using UnityEngine;

public class ImpactReceiver : MonoBehaviour
{
    [SerializeField] private float mass = 3f; // defines the character mass
    private Vector3 _impact = Vector3.zero;
    private CharacterController _characterController;
    //private float _gravityVelocity;
    //private float _gravity;
 
    private void Start()
    {
        _characterController = GetComponent<CharacterController>();

        //_gravity = 2.81f;
    }
        
    public void AddImpact(Vector3 direction, float force){
        direction.Normalize();
        if (direction.y < 0) 
        {
            direction.y = -direction.y; // reflect down force on the ground
        }
        _impact += direction.normalized * force / mass;
    }
 
    private void Update()
    {
            
        if (_impact.magnitude > 0.2) _characterController.Move(_impact * Time.deltaTime);
        _impact = Vector3.Lerp(_impact, Vector3.zero, 5*Time.deltaTime);
    }
}