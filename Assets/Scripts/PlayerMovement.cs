using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float forceMagnitude;
    [SerializeField] private float maxVelocity;
    [SerializeField] private float rotationSpeed;

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
        TakeInput();

        MoveToOtherSide();

        RotateIntoDirection();
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

    private void TakeInput()
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

    private void MoveToOtherSide()
    {
        Vector3 newPos = transform.position;
        //Store where player is on camera
        Vector3 viewportPos = mainCamera.WorldToViewportPoint(transform.position);
        //Teleport to other side if we are outside of viewport box
        if(viewportPos.x > 1)
        {
            newPos.x = -newPos.x + 0.1f;
        } else if(viewportPos.y > 1) {
            newPos.y = -newPos.y + 0.1f;
        } else if(viewportPos.y < 0) {
            newPos.y = -newPos.y - 0.1f;
        } else if(viewportPos.x < 0) {
            newPos.x = -newPos.x - 0.1f;
        }
        transform.position = newPos;
    }

    private void RotateIntoDirection()
    {
        //Default rotation at the start
        if(rb.velocity == Vector3.zero) {return; }
        //Rotate into flying direction
        Quaternion targetRotation = Quaternion.LookRotation(rb.velocity, Vector3.back);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}
