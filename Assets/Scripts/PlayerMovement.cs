using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float forceMagnitude;
    [SerializeField] private float maxVelocity;

    private Camera mainCamera;
    private Rigidbody rb;
    private Vector3 movementDirection;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //Check if screen was touched
        if(Touchscreen.current.primaryTouch.press.isPressed)
        {
            Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();

            //Convert from screen position to world position
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(touchPosition);

            //Move player away from the finger touch
            movementDirection = transform.position - worldPosition;
            movementDirection.z = 0f;
            //Make sure spaceship doesnt go faster if player's finger is further away
            movementDirection.Normalize();
        } else 
        {
            //Dont move if player is not pressing anything
            movementDirection = Vector3.zero;
        }
    }
    //FixedUpdate is called every time physics system updates
    void FixedUpdate()
    {
        //Dont push player if screen is not pressed
        if(movementDirection == Vector3.zero) { return; }
        //You dont need to multiply the movement by Time.deltaTime because fixed update is called fixed amount of times and is not dependent on framerate
        rb.AddForce(movementDirection * forceMagnitude * Time.deltaTime, ForceMode.Force);
        //Limit velocity of the spaceship so it wont' go too fast
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxVelocity);

    }
}
