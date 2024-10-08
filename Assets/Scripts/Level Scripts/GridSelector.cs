using System.Collections;
using System.Collections.Generic;
using Interaction;
using UnityEngine;
using Utility;

namespace Level_Scripts
{
    public class GridSelector : MonoBehaviour
    {
        // The distance in front of the player that they can select/interact with objects
        [SerializeField] private float interactDistance = 4;
        
        // The highlight prefab and the time its takes to smoothly transition between cells
        [SerializeField] private GameObject highlighterPrefab;
        [Range(0,1)] [SerializeField] private float transitionTime = 0.1f;
        private Coroutine _transitionCoroutine;

        // The layers that the player can interface with
        [SerializeField] private LayerMask selectionMask;
        [SerializeField] private LayerMask interactableMask;
        
        // Variables to keep track of the selectors previous state
        [SerializeField] private Material transparentMaterial;
        private Material _opaqueMaterial;
        private OutlineHighlight _outlineHighlight;
        private GameObject _currHighlightMarker;
        private Renderer _currHighlightRenderer;
        private GameObject _prevObject;
        private Vector3 _prevCell;

        // The object the player is currently selecting and if they're holding an item
        public GameObject SelectedObject { get; private set; }
        public bool PlayerHoldingItem { private get; set; }

        private Dictionary<GameObject, Interactable> _cachedInteractables;

        private void Update()
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
                if (_prevObject)
                    TryHighlightInteractable(_prevObject, false);

                // Reset previously selected cell since there is no cell in front of the player
                ResetPreviousCell();
                return;
            }
            
            // If we're not looking at a new position, no need to alter the selection/highlight much
            var hitPosition = hit.transform.position;
            if (_currHighlightMarker) UpdateMarkerTransparency();
            if (hitPosition == _prevCell) return;
            
            // Select an object based on intractability and vertical height
            var blockAbove = GridManager.Instance.GetCellInColumn(hitPosition, 1);
            if (blockAbove)
            {
                // If the block above is not interactable, we don't need a marker and can
                // set the previous cell position so we don't do another calculation next call
                if (!CheckInteractable(blockAbove))
                {
                    SelectedObject = null;
                    DestroyMarker();
                    if (_prevObject)
                        TryHighlightInteractable(_prevObject, false);
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
                _currHighlightMarker = Instantiate(highlighterPrefab, highlightPosition, highlighterPrefab.transform.rotation);
                _currHighlightRenderer = _currHighlightMarker.GetComponent<Renderer>();
                _outlineHighlight = _currHighlightMarker.GetComponent<OutlineHighlight>();
                if (!_opaqueMaterial) _opaqueMaterial = _currHighlightRenderer.material;

            }
        }
        
        // Toggles the highlight on an interactable using its attached script to alter its emission value
        private void TryHighlightInteractable(GameObject target, bool isHighlighted)
        {
            _cachedInteractables ??= new Dictionary<GameObject, Interactable>();
            
            if (_cachedInteractables.TryGetValue(target, out var interactable))
            {
                interactable.OutlineHighlight.enabled = isHighlighted;
                if (interactable.DecalProjector)
                {
                    interactable.DecalProjector.enabled = isHighlighted;
                }
            }
            else
            {
                var interactableComponent = target.GetComponent<Interactable>();
                if (!interactableComponent) return;
                
                interactableComponent.OutlineHighlight.enabled = isHighlighted;
                if (interactableComponent.DecalProjector)
                {
                    interactableComponent.DecalProjector.enabled = isHighlighted;
                }
                _cachedInteractables.Add(target, interactableComponent);

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

        // Sets the marker to be slightly transparent if the player isn't holding anything
        private void UpdateMarkerTransparency()
        {
            if (!PlayerHoldingItem && _currHighlightRenderer.material != transparentMaterial)
            {
                _currHighlightRenderer.material = transparentMaterial;
                _outlineHighlight.enabled = false;
            }
            else
            {
                _currHighlightRenderer.material = _opaqueMaterial;
                _outlineHighlight.enabled = true;
            }
        }
        // Resets the location of the previously selected cell. Used both when we're not looking
        // at anything and also when we place something since we have to recalculate what we're selecting
        public void ResetPreviousCell()
        {
            _prevCell = Vector3.negativeInfinity;
        }
    }
}
