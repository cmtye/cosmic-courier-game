using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
    
        private void Awake()
        {
            _characterMovement = GetComponent<CharacterMovement>();
            _characterController = GetComponent<CharacterController>();

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
            Debug.Log("Interact!");
        }
    
        private void OnEnable()
        {
            _controls.Enable();
        }
        
        private void OnDisable()
        {
            _controls.Disable();
        }
    }
}
