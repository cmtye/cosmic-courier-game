using System;
using System.Collections;
using UnityEngine;

namespace Utility_Scripts.Grid
{
    public class GridSelector : MonoBehaviour
    {
        // The distance in front of the player that they can select/interact with objects
        [SerializeField] private float interactDistance = 4;
        
        // The highlight prefab and the time its takes to smoothly transition between cells
        [SerializeField] private GameObject highlighter;
        [Range(0,1)] [SerializeField] private float transitionTime = 0.1f;
        private Coroutine _transitionCoroutine;

        // The layers that the player can interface with
        [SerializeField] private LayerMask selectionMask;
        [SerializeField] private LayerMask interactableMask;
        
        // Variables to keep track of the selectors previous state
        private GameObject _currHighlightMarker;
        private GameObject _prevObject;
        private Vector3 _prevCell;

        // The object the player is currently selecting
        public GameObject SelectedObject { get; private set; }

        private void FixedUpdate()
        {
            FindTarget(transform);
        }

        // Shoots out a ray every call. What this ray hits/doesnt hit determines what the player
        // is currently targeting and what should be highlighted
        private void FindTarget(Transform playerTransform)
        {
            // Make a vector out in front of the character and slightly downward to get the tile in front of us
            var aheadVector = playerTransform.TransformDirection(Vector3.forward) * interactDistance;
            aheadVector.y -= 1;

            // Check layers for a hit, return if nothing (can change valid layers as need be)
            Debug.DrawRay(playerTransform.position, aheadVector, Color.red);
            if (!Physics.Raycast(playerTransform.position, aheadVector, out var hit, interactDistance, selectionMask))
            {
                // Player doesn't have a cell in front of them, destroy the highlight and update
                // the currently selected selected object to reflect it
                SelectedObject = null;
                DestroyMarker();

                // Reset previously selected cell since there is no cell in front of the player
                ResetPreviousCell();
                return;
            }
            
            // If we're not looking at a new position, no need to alter the selection/highlight
            var hitPosition = hit.transform.position;
            if (hitPosition == _prevCell) return;
            
            // Select an object based on intractability and vertical height
            var blockAbove = GridManager.Instance.FindBlockAbove(hitPosition);
            if (blockAbove)
            {
                // If the block above is not interactable, we don't need a marker and can
                // set the previous cell position so we don't do another calculation next call
                if (!CheckInteractable(blockAbove))
                {
                    DestroyMarker();
                    _prevCell = hitPosition;
                    return;
                }
                // Otherwise set the block as the selected object
                SelectedObject = blockAbove;
                
            }
            else
            {
                SelectedObject = hit.transform.gameObject;
            }

            // Turn off previous interactable highlight on last object if there is one
            if (_prevObject)
                TryHighlightInteractable(_prevObject, false);

            if (CheckInteractable(SelectedObject))
            {
                // Destroy highlight marker since we're highlighting the object now
                DestroyMarker();
                TryHighlightInteractable(SelectedObject, true);
            }
            else
            {
                // Move highlight to new cell, or instantiate a new one if no current cell is highlighted
                TryHighlightBlock(hitPosition);
            }
            _prevCell = hitPosition;
            _prevObject = SelectedObject;
        }

        // Highlight a block by placing a marker prefab over top of it. Re-instantiates 
        // the transition coroutine and marker when necessary
        private void TryHighlightBlock(Vector3 cell)
        {
            var highlightPosition = cell;
            highlightPosition.y += 0.6f;
            if (_currHighlightMarker)
            {
                if (_transitionCoroutine != null)
                {
                    StopCoroutine(_transitionCoroutine);
                    _transitionCoroutine = StartCoroutine(TransitionMarker(highlightPosition));
                }
                else
                {
                    _transitionCoroutine = StartCoroutine(TransitionMarker(highlightPosition));
                }
            }
            else
            {
                // Increase position by a bit over half a block to show marker over top of the target
                _currHighlightMarker = Instantiate(highlighter, highlightPosition, highlighter.transform.rotation);
            }
        }
        
        // Toggles the highlight on an interactable using its attached script to alter its emission value
        private void TryHighlightInteractable(GameObject target, bool highlight)
        {
            var interactableComponent = target.GetComponent<InteractableHighlight>();
            if (interactableComponent)
            {
                interactableComponent.ToggleHighlight(highlight);
            }
        }
        
        // Smoothly transition the marker over to a newly selected cell
        private IEnumerator TransitionMarker(Vector3 cell) {
            var position = Vector3.zero;

            while(_currHighlightMarker.transform.position != cell) {
                _currHighlightMarker.transform.position = Vector3.SmoothDamp(_currHighlightMarker.transform.position, 
                                                                    cell, ref position, transitionTime);
                yield return null;
            }
        }
        
        // Destroys the marker if it exists and stops any coroutine associated with it
        private void DestroyMarker()
        {
            if (!_currHighlightMarker) return;
            if (_transitionCoroutine != null)
            {
                StopCoroutine(_transitionCoroutine);
            }
            Destroy(_currHighlightMarker);
        }
        
        // Return whether or not the given block is an interactable object
        private bool CheckInteractable(GameObject block)
        {
            return interactableMask == (interactableMask | (1 << block.gameObject.layer));
        }
        
        // Resets the location of the previously selected cell. Used both when we're not looking
        // at anything and also when we place something since we have to recalculate what we're selecting
        public void ResetPreviousCell()
        {
            _prevCell = Vector3.negativeInfinity;
        }
    }
}
