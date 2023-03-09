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

        // The layers that the player can interact with
        [SerializeField] private LayerMask selectionMask;
        [SerializeField] private LayerMask interactableMask;
        
        // Variables to keep track of the selectors previous state
        private GameObject _currHighlightMarker;
        private GameObject _prevObject;
        private Vector3 _prevCell;
        private readonly Collider[] _collidersAbove = new Collider[10];

        public GameObject SelectedObject { get; private set; }

        private void Update()
        {
            FindTarget(transform);
        }

        private void FindTarget(Transform playerTransform)
        {
            // Make a vector out in front of the character and slightly downward to get the tile in front of us
            var fwd = playerTransform.TransformDirection(Vector3.forward) * interactDistance;
            fwd.y -= 1;

            // Check layers for a hit, return if nothing (can change valid layers as need be)
            Debug.DrawRay(playerTransform.position, fwd, Color.red);
            if (!Physics.Raycast(playerTransform.position, fwd, out var hit, interactDistance, selectionMask))
            {
                // Player doesn't have a cell in front of them, destroy the highlight and update current selected
                SelectedObject = null;
                if (!_currHighlightMarker) return;
                Destroy(_currHighlightMarker);
                
                // End the current highlight transition if there is one
                StopCoroutine(_transitionCoroutine);

                // Reset previous cell since there is no cell in front of them
                _prevCell = Vector3.negativeInfinity;
                return;
            }
            SelectedObject = hit.transform.gameObject;

            var cellPosition = SelectedObject.transform.position;

            // If there is no valid placement at this position, return
            if (!CheckValidPlacement(cellPosition)) return;
            
            // Store selected cell and its position
            SelectedObject = hit.transform.gameObject;
            cellPosition = SelectedObject.transform.position;
            if (cellPosition == _prevCell) return;

            // Turn off previous interactable highlight on object if there was one
            if (_prevObject && !_currHighlightMarker)
            {
                if (_prevObject.transform.position == cellPosition) return;
                TryHighlightInteractable(_prevObject, false);
            }
                
            if (interactableMask == (interactableMask | (1 << SelectedObject.layer)))
            {
                // Destroy highlight marker since we're highlighting the object now
                if (_currHighlightMarker)
                {
                    Destroy(_currHighlightMarker);
                    StopCoroutine(_transitionCoroutine);
                }
                TryHighlightInteractable(SelectedObject, true);
            }
            else
            {
                // Move highlight to new cell, or instantiate a new one if no current cell is highlighted
                TryHighlightBlock(cellPosition);
            }
            _prevCell = cellPosition;
            _prevObject = SelectedObject;
        }

        private bool CheckValidPlacement(Vector3 cellPosition)
        {
            return CheckCellAbove(cellPosition);
        }
        private bool CheckCellAbove(Vector3 cellPosition)
        {
            var cellAbove = cellPosition;
            cellAbove.y += 1;
            var amountHit = Physics.OverlapSphereNonAlloc(cellAbove, 0.01f, _collidersAbove);
            if (amountHit == 0)
            {
                return true;
            }
            for (var i = 0; i < amountHit; i++)
            {
                if (interactableMask == (interactableMask | (1 << _collidersAbove[i].gameObject.layer)))
                {
                    if (_prevObject && !_currHighlightMarker)
                    {
                        SelectedObject = _collidersAbove[i].gameObject;
                        if (_prevObject.transform.position == cellAbove) return false;
                        TryHighlightInteractable(_prevObject, false);
                    }
                    SelectedObject = _collidersAbove[i].gameObject;
                    TryHighlightInteractable(SelectedObject, true);
                    _prevObject = SelectedObject;

                    if (!_currHighlightMarker) return false;
                    Destroy(_currHighlightMarker);
                    StopCoroutine(_transitionCoroutine);
                    return false;
                }
            }

            if (!_currHighlightMarker) return false;
            Destroy(_currHighlightMarker);
            StopCoroutine(_transitionCoroutine);
            return false;
        }

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

        // TODO: Maybe theres a cleaner/more efficient way to handle highlighting?
        // I feel like this may be better if integrated closely with the grid system.
        // Least expensive solution I could currently come up with since we are changing renderers
        // Other highlighting solutions potentially? Events?
        private void TryHighlightInteractable(GameObject target, bool highlight)
        {
            var interactableComponent = target.GetComponent<InteractableHighlight>();
            if (interactableComponent)
            {
                interactableComponent.ToggleHighlight(highlight);
            }
        }
        
        private IEnumerator TransitionMarker(Vector3 cell) {
            var position = Vector3.zero;

            while(_currHighlightMarker.transform.position != cell) {
                _currHighlightMarker.transform.position = Vector3.SmoothDamp(_currHighlightMarker.transform.position, 
                                                                    cell, ref position, transitionTime);
                yield return null;
            }
        }
    }
}
