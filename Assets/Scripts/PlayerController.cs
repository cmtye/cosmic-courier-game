using System;
using Enemy_Scripts;
using Level_Scripts.Grid;
using Level_Scripts.Interaction;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using Utility.Interaction;
using UI.RadialMenu;

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
    [SerializeField] private Transform holdPoint;
    private Collider _lastHeld;
    private Coroutine _pickupCoroutine;

    public Transform respawnPoint;

    [SerializeField] private GameObject _playerRadial;

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
        _lastHeld = null;
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
        if (selectedObject == null)
        {
            if (currentlyHeld.CompareTag("Item"))
                ThrowItem(false);
            
            return null;
        }

        if (selectedObject.GetComponent<Interactable>())
        {
            return selectedObject.GetComponent<Interactable>()?.Interact(this);
        }

        if (!currentlyHeld) return null;
        
        if (currentlyHeld.CompareTag("Item"))
        {
            ThrowItem(true);
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
        return null;
        
    }

    public GameObject TakeHeldItem(GameObject caller)
    {
        var item = currentlyHeld;
        var targetPosition = caller.transform.position;
        var duration = .2f;
        currentlyHeld.transform.SetParent(caller.transform);
        StartCoroutine(MoveToPosition(currentlyHeld, targetPosition, duration));

        currentlyHeld = null;
        OnSlotChanged?.Invoke(null);
        return item;
    }


    private IEnumerator MoveToPosition(GameObject toMove, Vector3 position, float timeToMove)
    {
        var currentPosition = toMove.transform.position;
        var t = 0f;
        while(t < 1)
        {
            t += Time.deltaTime / timeToMove;
            toMove.transform.position = Vector3.Lerp(currentPosition, position, t);
            yield return null;
        }
    }

    public void FooButtonTest()
    {
        Debug.Log("The player controller knows about the foo button being pressed");
    }

    private void OnTriggerStay(Collider other)
    {
        CheckForItem(other);
    }

    private void ThrowItem(bool blockSelected)
    {
        currentlyHeld.transform.SetParent(null);
                
        var heldRigidbody = currentlyHeld.GetComponent<Rigidbody>();
        heldRigidbody.useGravity = true;
        heldRigidbody.constraints = RigidbodyConstraints.None;

        var currentPosition = transform.position;
        var hoverDirection = blockSelected ? 
            (_gridSelector.SelectedObject.transform.position - currentPosition).normalized 
            : transform.TransformDirection(Vector3.forward).normalized;
        
        var horizontalForce = hoverDirection.normalized * 5f;
        var verticalForce = Vector3.up * 3f;
        heldRigidbody.AddForce(horizontalForce + verticalForce, ForceMode.Impulse);

        currentlyHeld = null;
        if (_pickupCoroutine == null)
            _pickupCoroutine = StartCoroutine(PickupWait(0.75f));
        else
        {
            StopCoroutine(_pickupCoroutine);
            _pickupCoroutine = StartCoroutine(PickupWait(0.75f));
        }
        OnSlotChanged?.Invoke(null);
    }
    private void CheckForItem(Collider collided)
    {
        if (collided.gameObject.CompareTag("Item"))
        {
            if (currentlyHeld || collided == _lastHeld) return;

            _lastHeld = collided;
            currentlyHeld = collided.gameObject;
            currentlyHeld.transform.SetParent(transform);
                
            currentlyHeld.transform.position = holdPoint.position;
                
            var heldRigidbody = currentlyHeld.GetComponent<Rigidbody>();
            heldRigidbody.useGravity = false;
            heldRigidbody.constraints = RigidbodyConstraints.FreezeAll;
                
            OnSlotChanged?.Invoke(currentlyHeld);
        }
    }

    private IEnumerator PickupWait(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (currentlyHeld) yield break;
        
        StopCoroutine(_pickupCoroutine);
        _lastHeld = null;
    }

    public MenuController GetMenu()
    {
        return _playerRadial.GetComponent<MenuController>();
    }

}