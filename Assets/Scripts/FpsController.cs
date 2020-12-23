using System.Collections;
using UnityEngine;

public class FpsController : MonoBehaviour
{
    [Header("Player")]
    public new Camera camera;
    public float horizontalSensitivity;
    public float verticalSensitivity;

    private Vector3 rotation;

    private Rigidbody rb;
    private Player player;
    private Vector3 localVelocity;

    [Header("Movement")]
    public int acceleration;
    public int counterAcceleration;
    public int walkMaxSpeed;
    public int runMaxSpeed;

    private int maxSpeed;
    private Vector3 wallRunDirection;
    private bool isWallrunning;
    private bool canMoveFAndB = true;
    private bool canMoveLAndR = true;

    [Header("Jumping")]
    public int jumpHeight;
    public int maxJumps;

    private int jumpsLeft;
    private bool isGrounded = true;
    private bool canJump = true;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        rb = gameObject.GetComponent<Rigidbody>();
        player = gameObject.GetComponent<Player>();

        rb.isKinematic = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        maxSpeed = walkMaxSpeed;
    }

    //void OnEnable()
    //{
    //    Start();
    //}

    //// Update is called once per frame
    //private void Update()
    //{

    //}

    //void FixedUpdate()
    //{
    //    //Movement
    //    if (isWallrunning)
    //    {
    //        if (Input.GetKey(KeyCode.W))
    //        {
    //            rigidBody.AddForce(wallRunDirection * acceleration, ForceMode.Force);
    //        }
    //        if (Input.GetKey(KeyCode.S))
    //        {
    //            rigidBody.AddForce(wallRunDirection * -acceleration, ForceMode.Force);
    //        }
    //    }

    //    if (isWallrunning)
    //    {
    //        rigidBody.AddForce(Vector3.up * 1000, ForceMode.Force);
    //    }
    //}

    //void LateUpdate()
    //{
    //    //Set rotation
    //    camera.transform.Rotate(rotation);
    //    transform.Rotate(new Vector3(transform.rotation.x, rotation.y, transform.rotation.z));
    //}

    //private void OnCollisionExit(Collision collision)
    //{
    //    if (isWallrunning)
    //    {
    //        canMoveRAndL = true;
    //        isWallrunning = false;
    //    }
    //}

    [HideInInspector]
    public bool[] inputs = new bool[5];

    private void FixedUpdate()
    {
        //Convert world space rigidbody velocity to local velocity
        localVelocity = transform.InverseTransformDirection(rb.velocity);

        //Running
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            maxSpeed = runMaxSpeed;
            acceleration *= 2;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            maxSpeed = walkMaxSpeed;
            acceleration /= 2;
        }

        //Movement
        if (inputs[0] && canMoveFAndB)
        {
            rb.AddForce(transform.forward * acceleration, ForceMode.Force);
        }
        if (inputs[1] && canMoveFAndB)
        {
            rb.AddForce(transform.forward * -acceleration, ForceMode.Force);
        }
        if (inputs[2] && canMoveLAndR)
        {
            rb.AddForce(transform.right * -acceleration, ForceMode.Force);
        }
        if (inputs[3] && canMoveLAndR)
        {
            rb.AddForce(transform.right * acceleration, ForceMode.Force);
        }

        //Counter movement
        if ((!inputs[0]) && (!inputs[1]))
        {
            if (localVelocity.z > 0)
            {
                rb.AddRelativeForce(Vector3.back * localVelocity.z * counterAcceleration, ForceMode.Force);
            }
            else if (localVelocity.z < 0)
            {
                rb.AddRelativeForce(Vector3.forward * -localVelocity.z * counterAcceleration, ForceMode.Force);
            }
        }

        if ((!inputs[2]) && (!inputs[3]))
        {
            if (localVelocity.x > 0)
            {
                rb.AddRelativeForce(Vector3.left * localVelocity.x * counterAcceleration, ForceMode.Force);
            }
            else if (localVelocity.x < 0)
            {
                rb.AddRelativeForce(Vector3.right * -localVelocity.x * counterAcceleration, ForceMode.Force);
            }
        }

        //Jumping
        if (inputs[4] && jumpsLeft > 0 && canJump)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
            jumpsLeft -= 1;
            isGrounded = false;

            StartCoroutine(waitBetweenJumps());
        }

        //Clamp speed
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);

        ServerSend.PlayerPosition(player.id, transform.position);
        ServerSend.PlayerRotation(player.id, transform.rotation);
    }

    private IEnumerator waitBetweenJumps()
    {
        canJump = false;
        yield return new WaitForSeconds(0.1f);
        canJump = true;
    }

    void OnCollisionEnter(Collision collision)
    {
        Vector3 colNormal = collision.contacts[0].normal;
        float objectAngle = Mathf.Acos(Vector3.Dot(Vector3.up, colNormal)) * Mathf.Rad2Deg;

        if (objectAngle > -45f && objectAngle < 45f)
        {
            jumpsLeft = maxJumps;
            isGrounded = true;
        }

        //if (objectAngle > 45f && objectAngle < 135f)
        //{
        //    wallRunDirection = Vector3.Cross(Vector3.up, colNormal);
        //    canMoveLAndR = false;
        //    isWallrunning = true;
        //    Quaternion.Lerp(transform.rotation, new Quaternion(transform.rotation.x, transform.rotation.y, transform.rotation.z + 20, 0), 1);
        //}
    }
}