using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : MonoBehaviour
{
    [SerializeField] private InputController input = null;
    [SerializeField, Range(0f, 10f)] private float jumpHeight = 5.5f;
    [SerializeField, Range(0, 5)] private int maxAirJump = 0;
    [SerializeField, Range(0f, 5f)] private float downwardMovementMultiplier = 4f;
    [SerializeField, Range(0f, 5f)] private float upwardMovementMultiplier = 2f;

    private Rigidbody2D body;
    private Ground ground;
    private Vector2 velocity;

    private int jumpPhase;
    private float defoultGravityScale;

    private bool desiredJump;
    private bool onGround;
    // Start is called before the first frame update
    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        ground = GetComponent<Ground>();

        defoultGravityScale = 1f;

    }

    // Update is called once per frame
    void Update()
    {
        desiredJump |= input.RetrieveJumpInput();

    }

    private void FixedUpdate()
    {
        onGround = ground.GetOnGroun();
        velocity = body.velocity;

        if (onGround)
        {
            jumpPhase = 0;
        }

        if (desiredJump)
        {
            desiredJump = false;
            JumpAction();
        }

        if(body.velocity.y > 0)
        {
            body.gravityScale = upwardMovementMultiplier;
        }
        else if (body.velocity.y < 0)
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
        if (onGround || jumpPhase < maxAirJump)
        {
            jumpPhase = 1;
            float jumpSpeed = Mathf.Sqrt(-2f * Physics2D.gravity.y * jumpHeight); 
            if(velocity.y > 0f)
            {
                jumpSpeed = Mathf.Max(jumpSpeed - velocity.y, 0f);
            }
            velocity.y += jumpSpeed;
        }
        
    }
}
