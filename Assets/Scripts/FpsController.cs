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
    private bool isWallrunning = false;
    private bool canMoveFAndB = true;
    private bool canMoveLAndR = true;

    [Header("Jumping")]
    public int jumpHeight;
    public int maxJumps;

    private int jumpsLeft;

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
    //    //Jumping
    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        if (jumpsLeft > 0)
    //        {
    //            rigidBody.velocity = new Vector3(rigidBody.velocity.x, 0, rigidBody.velocity.z);
    //            rigidBody.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
    //            jumpsLeft -= 1;
    //        }
    //    }

    //    //Running
    //    if (Input.GetKeyDown(KeyCode.LeftShift))
    //    {
    //        maxSpeed = runMaxSpeed;
    //        acceleration *= 2;
    //    }
    //    if (Input.GetKeyUp(KeyCode.LeftShift))
    //    {
    //        maxSpeed = walkMaxSpeed;
    //        acceleration /= 2;
    //    }

    //    //Convert world space rigidbody velocity to local velocity
    //    localVelocity = transform.InverseTransformDirection(rigidBody.velocity);
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

    //    if (Input.GetKey(KeyCode.A) && canMoveRAndL)
    //    {
    //        rigidBody.AddForce(transform.right * -acceleration, ForceMode.Force);
    //    }
    //    if (Input.GetKey(KeyCode.D) && canMoveRAndL)
    //    {
    //        rigidBody.AddForce(transform.right * acceleration, ForceMode.Force);
    //    }

    //    //Counter movement
    //    if ((!Input.GetKey(KeyCode.W)) && (!Input.GetKey(KeyCode.S)))
    //    {
    //        if (localVelocity.z > 0)
    //        {
    //            rigidBody.AddRelativeForce(Vector3.back * localVelocity.z * counterAcceleration, ForceMode.Force);
    //        }
    //        else if (localVelocity.z < 0)
    //        {
    //            rigidBody.AddRelativeForce(Vector3.forward * -localVelocity.z * counterAcceleration, ForceMode.Force);
    //        }
    //    }

    //    if ((!Input.GetKey(KeyCode.D)) && (!Input.GetKey(KeyCode.A)))
    //    {
    //        if (localVelocity.x > 0)
    //        {
    //            rigidBody.AddRelativeForce(Vector3.left * localVelocity.x * counterAcceleration, ForceMode.Force);
    //        }
    //        else if (localVelocity.x < 0)
    //        {
    //            rigidBody.AddRelativeForce(Vector3.right * -localVelocity.x * counterAcceleration, ForceMode.Force);
    //        }
    //    }

    //    //Clamp speed
    //    rigidBody.velocity = Vector3.ClampMagnitude(rigidBody.velocity, maxSpeed);

    //    if (isWallrunning)
    //    {
    //        rigidBody.AddForce(Vector3.up * 1000, ForceMode.Force);
    //    }

    //    //Get mouse rotation inputs
    //    rotation.y += Input.GetAxis("Mouse X") * horizontalSensitivity;
    //    rotation.x += -Input.GetAxis("Mouse Y") * verticalSensitivity;
    //}

    //void LateUpdate()
    //{
    //    //Set rotation
    //    camera.transform.Rotate(rotation);
    //    transform.Rotate(new Vector3(transform.rotation.x, rotation.y, transform.rotation.z));
    //}

    //void OnCollisionEnter(Collision collision)
    //{
    //    Vector3 colNormal = collision.contacts[0].normal;
    //    float objectAngle = Mathf.Acos(Vector3.Dot(Vector3.up, colNormal)) * Mathf.Rad2Deg;

    //    if (objectAngle > -45f && objectAngle < 45f)
    //    {
    //        jumpsLeft = maxJumps;
    //    }

    //    if (objectAngle > 45f && objectAngle < 135f)
    //    {
    //        wallRunDirection = Vector3.Cross(Vector3.up, colNormal);
    //        canMoveRAndL = false;
    //        isWallrunning = true;
    //        Quaternion.Lerp(transform.rotation, new Quaternion(transform.rotation.x, transform.rotation.y, transform.rotation.z + 20, 0), 1);
    //    }
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
            rb.AddForce(transform.right * acceleration, ForceMode.Force);
        }
        if (inputs[3] && canMoveLAndR)
        {
            rb.AddForce(transform.right * -acceleration, ForceMode.Force);
        }

        ServerSend.PlayerPosition(player.id, transform.position);
        ServerSend.PlayerRotation(player.id, transform.rotation);
    }
}