using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Utility_Scripts;

namespace Character_Scripts
{
    [RequireComponent(typeof(CharacterMovement))]
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(GridSelector))]
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

        private GameObject AttemptInteract()
        {
            var selectedObject = _gridSelector.SelectedObject;
            if (selectedObject == null) return null;
            
            return selectedObject.GetComponent<Interactable>()?.Interact();
        }
    }
}
