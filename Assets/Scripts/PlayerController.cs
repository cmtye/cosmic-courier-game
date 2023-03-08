using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Utility_Scripts;

namespace Character_Scripts
{
    [RequireComponent(typeof(CharacterMovement))]
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        // The players movement and input variables.
        private PlayerInputActions _controls;
        private CharacterMovement _characterMovement;
        private CharacterController _characterController;
        private Vector2 _moveDirectionInput;

        private GridSelector _gridSelector;
    
        private void Awake()
        {
            _characterMovement = GetComponent<CharacterMovement>();
            _characterController = GetComponent<CharacterController>();
            _gridSelector = GetComponent<GridSelector>();

            _controls = new PlayerInputActions();
            _controls.Player.Move.started += OnMovementInput;
            _controls.Player.Move.canceled += OnMovementInput;
            _controls.Player.Move.performed += OnMovementInput;
            _controls.Player.Interact.performed += OnInteractInput;
        }
        
        private void Update()
        {  
            _characterMovement.Move(_moveDirectionInput);
        }
    
        private void OnMovementInput(InputAction.CallbackContext context)
        {
            _moveDirectionInput = context.ReadValue<Vector2>();
        }

        private void OnInteractInput(InputAction.CallbackContext context)
        {
            AttemptInteract();
        }
    
        private void OnEnable()
        {
            _controls.Enable();
        }
        
        private void OnDisable()
        {
            _controls.Disable();
        }

        // TODO: Maybe move this to a separate script?
        private GameObject AttemptInteract()
        {
            // Here is a solution using the selector component since it already keeps track of what we're looking at
            // I've commented it out so you can test it, feel free to delete if it doesn't work for our use case
            /*if (_gridSelector)
            {
                var selectedObject = _gridSelector.SelectedObject;
                if (selectedObject == null) return null;
                
                var script = selectedObject.GetComponent<Interactable>();
                return script?.Interact();

            }
            else
            {
                Debug.LogError("No selector component attached on " + name);
                return null;
            }*/
            
            // Make a vector out in front of the character and slightly downward to get the tile in front of us
            float interactDistance = 2; 
            var fwd = transform.TransformDirection(Vector3.forward) * interactDistance;
            fwd.y -= 1;

            RaycastHit hit;
            Debug.DrawRay(transform.position, fwd, Color.blue);
            if (Physics.Raycast(transform.position, fwd, out hit, 2, -1))
            {
                GameObject hitObject = hit.transform.gameObject;
                Interactable script = hitObject.GetComponent<Interactable>();
                return script?.Interact();
            }

            return null;
        }
    }
}
