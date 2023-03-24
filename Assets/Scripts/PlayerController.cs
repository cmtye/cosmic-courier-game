using System;
using Enemy_Scripts;
using Level_Scripts.Grid;
using UnityEngine;
using UnityEngine.InputSystem;
using Utility.Interaction;

[RequireComponent(typeof(CharacterMovement))]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(GridSelector))]
public class PlayerController : MonoBehaviour
{
    public static event Action<GameObject> OnSlotChanged;
    
    // The players movement and input variables.
    private PlayerInputActions _controls;
    private CharacterMovement _characterMovement;
    private CharacterController _characterController;
    private Vector2 _moveDirectionInput;
    private ParticleController _particleController;

    private GridSelector _gridSelector;
    public GameObject currentlyHeld;

    private void Awake()
    {
        _characterMovement = GetComponent<CharacterMovement>();
        _characterController = GetComponent<CharacterController>();
        _gridSelector = GetComponent<GridSelector>();
        _particleController = GetComponent<ParticleController>();

        _controls = new PlayerInputActions();
        _controls.Player.Move.started += OnMovementInput;
        _controls.Player.Move.canceled += OnMovementInput;
        _controls.Player.Move.performed += OnMovementInput;
        _controls.Player.Interact.performed += OnInteractInput;
    }

    private void Start()
    {
        OnSlotChanged?.Invoke(currentlyHeld);
    }

    private void Update()
    {
        _characterMovement.UpdateFloorBehavior();
        _characterMovement.Move(_moveDirectionInput);
        
        _particleController.SetDustTrail(_moveDirectionInput.magnitude > 0);
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

        if (selectedObject.GetComponent<Interactable>())
        {
            return selectedObject.GetComponent<Interactable>()?.Interact();
        }

        if (currentlyHeld)
        {
            if (currentlyHeld.CompareTag("Item"))
            {
                currentlyHeld.transform.SetParent(null);
                currentlyHeld = null;
                OnSlotChanged?.Invoke(null);
                return null;
            }
            if (GridManager.Instance.TryPlaceObject(currentlyHeld, selectedObject.transform.position))
            {
                currentlyHeld = null;
                OnSlotChanged?.Invoke(null);
                _gridSelector.ResetPreviousCell();
            }
            else
            {
                Debug.Log("Cannot place currently held object here");
            }
        }

        return null;
    }

    public void FooButtonTest()
    {
        Debug.Log("The player controller knows about the foo button being pressed");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            if (!currentlyHeld)
            {
                currentlyHeld = other.gameObject;
                currentlyHeld.transform.SetParent(transform);
                OnSlotChanged?.Invoke(currentlyHeld);
            }
        }
    }
}