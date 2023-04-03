using System;
using Level_Scripts.Grid;
using Level_Scripts.Interaction;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using Tower_Scripts;
using UI.RadialMenu;
using Utility;

[RequireComponent(typeof(CharacterMovement))]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(GridSelector))]
public class PlayerController : MonoBehaviour
{
    public static event Action<GameObject> OnSlotChanged;
    
    // The players movement and input variables.
    private PlayerInputActions _controls;
    private CharacterMovement _characterMovement;
    private Vector2 _moveDirectionInput;
    private ParticleController _particleController;
    private Animator _animator;

    // The players item/tower holding variables
    [SerializeField] private Transform holdTransform;
    public GameObject currentlyHeld;

    // The players variables that interact with the world or towers
    [SerializeField] private GameObject playerRadial;
    public Transform respawnPoint;
    private GridSelector _gridSelector;
    private static readonly int Running = Animator.StringToHash("Running");

    private void Awake()
    {
        _characterMovement = GetComponent<CharacterMovement>();
        _gridSelector = GetComponent<GridSelector>();
        _particleController = GetComponent<ParticleController>();
        _animator = GetComponentInChildren<Animator>();

        _controls = new PlayerInputActions();
        _controls.Player.Move.started += OnMovementInput;
        _controls.Player.Move.canceled += OnMovementInput;
        _controls.Player.Move.performed += OnMovementInput;
        _controls.Player.Interact.performed += OnInteractInput;
    }

    private void Start()
    {
        InvokeSlotChange(currentlyHeld);
    }

    private void Update()
    {
        if (_animator) _animator.SetBool(Running, _moveDirectionInput.magnitude > 0);
        _characterMovement.UpdateFloorBehavior();
        _characterMovement.Move(_moveDirectionInput);

        _particleController.SetDustTrail(_moveDirectionInput.magnitude > 0);
        _gridSelector.PlayerHoldingItem = currentlyHeld;
    }
    
    private void AttemptInteract()
    {
        var selectedObject = _gridSelector.SelectedObject;
        
        // If there is no selected object and nothing is currently being held, there is nothing to interact with.
        if (!selectedObject && !currentlyHeld) return;

        // If there is a selected object, try to interact with it.
        if (selectedObject)
        {
            // We try to interact with interactable objects first
            var interactable = selectedObject.GetComponent<Interactable>();
            if (interactable)
            {
                interactable.Interact(this);
                return;
            }
            
            // If there was no interactable, we'll check what we're holding
            if (currentlyHeld)
            {
                // Items get thrown if held before we check placement
                if (currentlyHeld.CompareTag("Item"))
                {
                    ReleaseHeldItem(true);
                    return;
                }

                // If we're holding something and its not an item, we'll try placing it instead
                if (GridManager.Instance.TryPlaceObject(currentlyHeld, selectedObject.transform.position))
                {
                    // Check if we've placed a tower, if so re-enable its components
                    var towerComponent = currentlyHeld.GetComponent<BaseTower>();
                    if (towerComponent) currentlyHeld.GetComponent<BaseTower>().IsDisabled = false;

                    currentlyHeld = null;
                    InvokeSlotChange(currentlyHeld);
                    _gridSelector.ResetPreviousCell();
                    return;
                }
                Debug.LogError("Cannot place object in this position");
            }
            return;

        }
        // If there is nothing selected but we have an item, throw it without a target
        if (currentlyHeld && currentlyHeld.CompareTag("Item")) ReleaseHeldItem(false);

    }

    private void ReleaseHeldItem(bool blockSelected)
    {
        // Enable gravity and release constraints on the held object
        currentlyHeld.transform.SetParent(null);
        
        // Enable gravity and release constraints on the held object
        var heldRigidbody = currentlyHeld.GetComponent<Rigidbody>();
        heldRigidbody.useGravity = true;
        heldRigidbody.constraints = RigidbodyConstraints.None;

        // Calculate the direction of the throw
        var currentPosition = transform.position;
        var hoverDirection = blockSelected ? 
            (_gridSelector.SelectedObject.transform.position - currentPosition).normalized 
            : transform.TransformDirection(Vector3.forward).normalized;
        
        // Apply horizontal and vertical forces to the object
        var horizontalForce = hoverDirection.normalized * 5f;
        var verticalForce = Vector3.up * 3f;
        heldRigidbody.AddForce(horizontalForce + verticalForce, ForceMode.Impulse);

        // Reset currentlyHeld to null and start the coroutine to wait before allowing the player to pick up
        // the same object
        var item = currentlyHeld.GetComponent<ItemController>();
        item.canPickup = false;
        currentlyHeld = null;
        StartCoroutine(PickupWaitCoroutine(item, 0.75f));
        
        // Invoke event to signal that the slot has changed
        InvokeSlotChange(currentlyHeld);
    }
    
    public GameObject TransferHeldItem(GameObject caller)
    {
        if (!currentlyHeld.CompareTag("Item")) return null;
        
        // Store a reference to the currently held object and make it so we can't pick it up again
        var item = currentlyHeld;
        item.GetComponent<ItemController>().canPickup = false;
        item.GetComponent<OutlineHighlight>().enabled = false;
        
        // Set the held object's parent to the target object and start moving the object towards the target position
        var targetPosition = caller.transform.position;
        var duration = .2f;
        currentlyHeld.transform.SetParent(caller.transform);
        StartCoroutine(MoveToPosition(currentlyHeld, targetPosition, duration));

        // Reset the currently held object and invoke the OnSlotChanged event
        currentlyHeld = null;
        InvokeSlotChange(currentlyHeld);
        
        // Return the previously held object
        return item;
    }

    private void PickupTower(PlayerController player, InteractionHandler handler)
    {
        // If the player is not the target player or an object is already being held, we can't pickup
        if (player != this || currentlyHeld) return;

        // Try to remove the object from the grid at the object's position. If unsuccessful, exit the method
        if(!GridManager.Instance.TryRemoveObject(handler.gameObject.transform.position)) return;
        _gridSelector.ResetPreviousCell();
            
        // Set the currently held object to the tower and update its position, rotation, and parent
        var towerHoldPoint = holdTransform.position + Vector3.up * 0.5f;
        currentlyHeld = handler.gameObject;
        currentlyHeld.transform.SetPositionAndRotation(towerHoldPoint, holdTransform.rotation);
        currentlyHeld.transform.localScale = Vector3.one;
        currentlyHeld.transform.SetParent(holdTransform);
        
        // Turn the towers abilities off while being held
        currentlyHeld.GetComponent<BaseTower>().IsDisabled = true;
        
        // Invoke event to signal that the slot has changed
        InvokeSlotChange(currentlyHeld);
    }

    private void ReceiveTower(PlayerController player, InteractionHandler handler, GameObject received)
    {
        // If the player is not the target player or an object is already being held, we can't pickup
        if (player != this || currentlyHeld) return;

        // If we cannot spend the cost of the tower, we can't craft
        if (!GameManager.Instance.Spend(received.GetComponent<BaseTower>().Cost)) return;

        // Instantiate a new tower
        var tower = Instantiate(received);

        // Set the currently held object to the tower and update its position, rotation, and parent
        var towerHoldPoint = holdTransform.position + Vector3.up * 0.5f;
        currentlyHeld = tower;
        currentlyHeld.transform.SetPositionAndRotation(towerHoldPoint, holdTransform.rotation);
        currentlyHeld.transform.localScale = Vector3.one;
        currentlyHeld.transform.SetParent(holdTransform);

        // Turn the towers abilities off while being held
        currentlyHeld.GetComponent<BaseTower>().IsDisabled = true;
        
        // Invoke event to signal that the slot has changed
        InvokeSlotChange(currentlyHeld);
    }
    
    private void HandleItemCollision(Collider collided)
    {
        // If the collision isn't an item, ignore it
        if (!collided.gameObject.CompareTag("Item")) return;
        
        // If an object is already being held or the collided object is the last held object, we can't pickup
        if (currentlyHeld) return;
        if (!collided.GetComponent<ItemController>().canPickup) return;
        
        // Set the currently held object to the collided object and update its position and constraints
        currentlyHeld = collided.gameObject;
        currentlyHeld.transform.SetParent(transform);
        currentlyHeld.transform.position = holdTransform.position;
                
        var heldRigidbody = currentlyHeld.GetComponent<Rigidbody>();
        heldRigidbody.useGravity = false;
        heldRigidbody.constraints = RigidbodyConstraints.FreezeAll;
            
        // Invoke event to signal that the slot has changed
        InvokeSlotChange(currentlyHeld);
    }
    
    private IEnumerator PickupWaitCoroutine(ItemController item, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        item.canPickup = true;
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
    
    public void InvokeSlotChange(GameObject held)
    {
        OnSlotChanged?.Invoke(held);
    }
    
    public void FooButtonTest()
    {
        Debug.Log("The player controller knows about the foo button being pressed");
    }
    
    private void OnTriggerStay(Collider other)
    {
        HandleItemCollision(other);
    }
    
    private void OnEnable()
    {
        _controls.Enable();
        PickupEvent.OnTowerPickup += PickupTower;
        CraftEvent.OnTowerCraft += ReceiveTower;
    }
        
    private void OnDisable()
    {
        _controls.Disable();
        PickupEvent.OnTowerPickup -= PickupTower;
        CraftEvent.OnTowerCraft -= ReceiveTower;
    }
    
    public MenuController GetMenu() { return playerRadial.GetComponent<MenuController>(); }
    private void OnMovementInput(InputAction.CallbackContext context) { _moveDirectionInput = context.ReadValue<Vector2>(); }
    private void OnInteractInput(InputAction.CallbackContext context) { AttemptInteract(); }

}