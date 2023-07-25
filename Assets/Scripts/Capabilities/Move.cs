using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

[RequireComponent(typeof(Controller))]
public class Move : MonoBehaviour
{
    
    [SerializeField, Range(0f, 100f)] private float maxSpeed = 4f;
    [SerializeField, Range(0f, 100f)] private float maxAcceleration = 35f;
    [SerializeField, Range(0f, 100f)] private float maxAirAcceleration = 20f;
    [SerializeField, Range(0.05f, 0.5f)] private float wallStickTime = 0.25f;

    private Vector2 direction;
    private Vector2 desiredVelocity;
    private Vector2 velocity;
    private Rigidbody2D body;
    private CollisionDataRetriever collisionDataRetriever;
    private Controller controller;
    private WallInterector wallInterector;

    private float maxSpeedChange, acceleration, wallStickCounter;
    private bool onGround;


    // Start is called before the first frame update
    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        collisionDataRetriever = GetComponent<CollisionDataRetriever>();
        controller = GetComponent<Controller>();
        wallInterector = GetComponent<WallInterector>();
    }

    // Update is called once per frame
    void Update()
    {
        direction.x = controller.input.RetrieveMoveInput();
        desiredVelocity = new Vector2(direction.x, 0f) * Mathf.Max(maxSpeed - collisionDataRetriever.Friction, 0f);
    }

    private void FixedUpdate()
    {
        onGround = collisionDataRetriever.OnGround;
        velocity = body.velocity;

        acceleration = onGround ? maxAcceleration : maxAirAcceleration;
        maxSpeedChange = acceleration * Time.deltaTime;
        velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
        #region Wall Stick
        if(collisionDataRetriever.OnWall && !collisionDataRetriever.OnGround && !wallInterector.WallJumping)
        {
            if(wallStickCounter > 0)
            {
                velocity.x = 0;

                if(controller.input.RetrieveMoveInput() == collisionDataRetriever.ContactNormal.x)
                {
                    wallStickCounter -= Time.deltaTime;
                }
                else
                {
                    wallStickCounter = wallStickTime;
                }
            }
            else
            {
                wallStickCounter = wallStickTime;
            }
        } 
        #endregion
        body.velocity = velocity;
    }
}
