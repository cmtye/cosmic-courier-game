using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    // The characters movement related variables.
    [Range(0, 1.0f)] [SerializeField] private float moveSmoothing = 0.5f;
    [SerializeField] private float speed = 10f;

    [SerializeField] private float rotateSpeed;

    private CharacterController _characterController;
    private Vector3 _currentVelocity;
    private Vector3 _input;
    private Vector3 _smoothInput;

    // The characters variables related to calculated gravity.
    [SerializeField] private float gravityMultiplier = 1f;
    private float _gravityVelocity;
    private float _gravity;

    // Reference to camera for euler angle calculation
    private Camera _camera;

    // Determine behaviors for different floor types
    // The Vector2s store <speed, moveSmoothing>
    [SerializeField] private Vector2 normalFloorBehavior = new Vector2(5f, 0.1f);
    [SerializeField] private Vector2 slippyFloorBehavior = new Vector2(7f, 0.5f);
    [SerializeField] private Vector2 stickyFloorBehavior = new Vector2(3f, 0.05f);

    private Dictionary<string, Vector2> _floorBehaviorLookup;

        
    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();

        _camera = Camera.main;

        _gravity = 9.81f;

        _floorBehaviorLookup = new Dictionary<string, Vector2>{
            {"Slippy", slippyFloorBehavior},
            {"Sticky", stickyFloorBehavior}};
    }


    public void UpdateFloorBehavior()
    {
        // Make a vector downward to get the tile below
        var currTransform = transform;
        var fwd = (currTransform).TransformDirection(Vector3.down) * 1;

        //Default to normal floor behavior if tag is not found

        // Check layers for a hit, return if nothing (can change valid layers as need be) 0 == Default, 7 == Unstackable
        Debug.DrawRay(currTransform.position, fwd, Color.green);
        if (Physics.Raycast(transform.position, fwd, out var hit, 1, (1 << 0 | 1 << 7)))
        {
            if(_floorBehaviorLookup.TryGetValue(hit.transform.tag, out var floorBehavior))
            {
                speed = floorBehavior.x;
                moveSmoothing = floorBehavior.y;
            }
            else
            {
                // Altering this code so that tags that aren't specified as special default to normal behavior
                speed = normalFloorBehavior.x;
                moveSmoothing = normalFloorBehavior.y;
            }
        }
    }

    // Takes in a 2D vector representing up and down movement, and
    // translates it to 3D velocity on a GameObjects rigidbody.
    public void Move(Vector2 moveDirection2D)
    {
        // Translate Vec2 to Vec3
        var moveDirection = new Vector3(moveDirection2D.x, 0, moveDirection2D.y);

        moveDirection = TransformMoveDirection(moveDirection);

        RotateTowardDirection(moveDirection);

        MoveInDirection(moveDirection);
    
    }

    private Vector3 TransformMoveDirection(Vector3 direction)
    {
        return Quaternion.Euler(0, _camera.gameObject.transform.eulerAngles.y, 0) * direction;
    }

    private void RotateTowardDirection(Vector3 direction)
    {
        if(direction.magnitude == 0) return;
        var rotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotateSpeed * Time.deltaTime);
    }

    private void MoveInDirection(Vector3 direction)
    {
        // Smoothly interpolate to our desired direction.
        _input = Vector3.SmoothDamp(_input, 
            direction, ref _smoothInput, moveSmoothing);

        // Give the player small downward velocity to keep them grounded.
        if (_characterController.isGrounded && _gravityVelocity < 0.0f)
        {
            _gravityVelocity = -1.0f;
        }

        // Downward velocity continues to grow as fall to simulate terminal velocity.
        _gravityVelocity -= _gravity * gravityMultiplier * Time.deltaTime;
        
        // Translate our 2D movement vector into a 3D vector with gravity.
        var moveVector = new Vector3(_input.x * speed, _gravityVelocity, _input.z * speed);
        _currentVelocity = moveVector;

        Physics.SyncTransforms();
        // Use built in character controller to move the character in our desired direction.
        _characterController.Move(_currentVelocity * Time.deltaTime);
    }
}