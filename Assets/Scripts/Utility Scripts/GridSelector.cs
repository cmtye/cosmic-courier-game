using System.Collections;
using UnityEngine;

namespace Utility_Scripts
{
    public class GridSelector : MonoBehaviour
    {
        // The distance in front of the player that they can select/interact with objects
        [SerializeField] private float interactDistance;
        
        // The highlight prefab and the time its takes to smoothly transition between cells
        [SerializeField] private GameObject highlighter;
        [Range(0,1)] [SerializeField] private float transitionTime;
        private Coroutine _transitionCoroutine;
        
        // The layers that the player can interact with
        [SerializeField] private LayerMask selectionMask;
        
        // Variables to keep track of the selectors previous state
        private GameObject _currHighlighted;
        private Vector3 _prevCell;

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

            // Check all layers for a hit, return if nothing (can change valid layers as need be)
            Debug.DrawRay(playerTransform.position, fwd, Color.red);
            if (!Physics.Raycast(playerTransform.position, fwd, out var hit, interactDistance, selectionMask))
            {
                // Player doesn't have a cell in front of them, destroy the highlight and update current selected
                SelectedObject = null;
                if (!_currHighlighted) return;
                Destroy(_currHighlighted);
                
                // End the current highlight transition if there is one
                StopCoroutine(_transitionCoroutine);

                // Reset previous cell since there is no cell in front of them
                _prevCell = Vector3.negativeInfinity;
                return;
            }

            // Raise target position slightly so instantiated object shows above the selected tile
            var cell = hit.transform.position;
            cell.y += 0.6f;

            if (cell == _prevCell) return;
            
            // Move highlight to new cell, or instantiate a new one if no current cell is highlighted
            if (_currHighlighted)
            {
                if (_transitionCoroutine != null)
                {
                    StopCoroutine(_transitionCoroutine);
                    _transitionCoroutine = StartCoroutine(TransitionCell(cell));
                }
                else
                {
                    _transitionCoroutine = StartCoroutine(TransitionCell(cell));
                }
            }
            else
            {
                _currHighlighted = Instantiate(highlighter, cell, highlighter.transform.rotation);
            }
            _prevCell = cell;
            SelectedObject = hit.transform.gameObject;
        }
        
        private IEnumerator TransitionCell(Vector3 cell) {
            var position = Vector3.zero;

            while(_currHighlighted.transform.position != cell) {
                _currHighlighted.transform.position = Vector3.SmoothDamp(_currHighlighted.transform.position, 
                                                                    cell, ref position, transitionTime);
                yield return null;
            }
        }
    }
}
