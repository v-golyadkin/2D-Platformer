using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(Controller))]
public class Jump : MonoBehaviour
{

    [SerializeField, Range(0f, 10f)] private float jumpHeight = 3f;
    [SerializeField, Range(0, 5)] private int maxAirJump = 0;
    [SerializeField, Range(0f, 5f)] private float downwardMovementMultiplier = 3f;
    [SerializeField, Range(0f, 5f)] private float upwardMovementMultiplier = 1.7f;
    [SerializeField, Range(0f, 0.3f)] private float coyoteTime = 0.2f;
    [SerializeField, Range(0f, 0.3f)] private float jumpBufferTime = 0.2f;

    private Controller controller;
    private Rigidbody2D body;
    private CollisionDataRetriever collisionDataRetriever;
    private Vector2 velocity;

    private int jumpPhase;
    private float defoultGravityScale, jumpSpeed, coyoteCounter, jumpBufferCounter;

    private bool desiredJump, onGround, isJumping;
    
    // Start is called before the first frame update
    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        collisionDataRetriever = GetComponent<CollisionDataRetriever>();
        controller = GetComponent<Controller>();

        defoultGravityScale = 1f;

    }

    // Update is called once per frame
    void Update()
    {
        desiredJump |= controller.input.RetrieveJumpInput();

    }

    private void FixedUpdate()
    {
        onGround = collisionDataRetriever.OnGround;
        velocity = body.velocity;

        if (onGround && body.velocity.y == 0)
        {
            coyoteCounter = coyoteTime; 
            jumpPhase = 0;
            isJumping = false;
        }
        else
        {
            coyoteCounter -= Time.deltaTime;
        }
        if (desiredJump)
        {
            desiredJump = false;
            jumpBufferCounter = jumpBufferTime;
        }
        else if(!desiredJump && jumpBufferCounter > 0)
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        if(jumpBufferCounter > 0)
        {
            JumpAction();
        }

        if(controller.input.RetrieveJumpHoldInput() && body.velocity.y > 0)
        {
            body.gravityScale = upwardMovementMultiplier;
        }
        else if (!controller.input.RetrieveJumpHoldInput() || body.velocity.y < 0)
        {
            body.gravityScale = downwardMovementMultiplier;
        }
        else if(body.velocity.y == 0)
        {
            body.gravityScale = defoultGravityScale;
        }

        body.velocity = velocity;
    }

    private void JumpAction()
    {
        if (coyoteCounter > 0f || (jumpPhase < maxAirJump && isJumping))
        {
            if (isJumping)
            {
                jumpPhase += 1;
            }

            jumpBufferCounter = 0;
            coyoteCounter = 0f;
            jumpSpeed = Mathf.Sqrt(-2f * Physics2D.gravity.y * jumpHeight);
            isJumping = true;

            if(velocity.y > 0f)
            {
                jumpSpeed = Mathf.Max(jumpSpeed - velocity.y, 0f);
            }
            else if (velocity.y < 0f)
            {
                jumpSpeed += Mathf.Abs(body.velocity.y);
            }
            velocity.y += jumpSpeed;
        }
        
    }
}
