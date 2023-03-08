using System.Collections;
using UnityEngine;

namespace Character_Scripts
{
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
        
        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();

            _camera = (Camera)FindObjectOfType(typeof(Camera));

            _gravity = 9.81f;
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
            
            // Use built in character controller to move the character in our desired direction.
            _characterController.Move(_currentVelocity * Time.deltaTime);
        }
    }
    
}
