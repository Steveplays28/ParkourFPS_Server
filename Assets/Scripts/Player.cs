using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Player")]
    public int id;
    public string username;
    public float health;
    public float maxHealth;
    public new Camera camera;

    private Rigidbody rb;
    private Vector3 localVelocity;

    [Header("Weapon")]
    public Weapon weapon;
    public Transform shootOrigin;

    [Header("Projectiles")]
    public int itemAmount;
    public int maxItemAmount;
    public float throwForce;

    [Header("Movement")]
    public int acceleration;
    public int counterAcceleration;
    public float crouchMaxSpeed;
    public float walkMaxSpeed;
    public float runMaxSpeed;
    [HideInInspector]
    public bool[] inputs;
    public bool isRunning;

    private float maxSpeed;
    private bool canMoveFAndB = true;
    private bool canMoveLAndR = true;

    [Header("Wallrunning")]
    public bool isWallrunning;

    private Vector3 wallRunDirection;
    private Vector3 wallRunNormal;

    [Header("Jumping")]
    public int jumpHeight;
    public int maxJumps;
    public int jumpsLeft;
    public bool canJump = true;

    private bool isGrounded = true;

    [Header("Crouching")]
    public bool canCrouch = true;

    private bool isCrouching;

    public void Initialize(int _id, string _username)
    {
        id = _id;
        username = _username;
        health = maxHealth;
        maxSpeed = walkMaxSpeed;
        inputs = new bool[5];

        rb = gameObject.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }

    /// <summary>Processes player input and moves the player.</summary>
    public void FixedUpdate()
    {
        //Convert world space rigidbody velocity to local velocity
        localVelocity = transform.InverseTransformDirection(rb.velocity);

        //Wallrunning
        if (Physics.Raycast(transform.position, transform.right, out RaycastHit hitR, 1))
        {
            if (!isWallrunning)
            {
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.useGravity = false;
                jumpsLeft = maxJumps;
                wallRunNormal = hitR.normal;
                isWallrunning = true;
            }

            wallRunDirection = Quaternion.AngleAxis(90, Vector3.up) * hitR.normal;
            Wallrun();
        }
        else if (Physics.Raycast(transform.position, -transform.right, out RaycastHit hitL, 1))
        {
            if (!isWallrunning)
            {
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.useGravity = false;
                jumpsLeft = maxJumps;
                wallRunNormal = hitL.normal;
                isWallrunning = true;
            }

            wallRunDirection = Quaternion.AngleAxis(-90, Vector3.up) * hitL.normal;
            Wallrun();
        }
        else
        {
            if (isWallrunning)
            {
                rb.useGravity = true;
                wallRunNormal = new Vector3(0, 0, 0);
                isWallrunning = false;
            }
        }

        Debug.DrawRay(transform.position, wallRunDirection, Color.cyan, 1);

        if (!isWallrunning)
        {
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
        }

        //Clamp speed
        rb.velocity = new Vector3(Mathf.Clamp(rb.velocity.x, -maxSpeed, maxSpeed), rb.velocity.y, Mathf.Clamp(rb.velocity.z, -maxSpeed, maxSpeed));

        ServerSend.PlayerPosition(this);
        ServerSend.PlayerRotation(this);
    }

    /// <summary>Updates the player input with newly received input.</summary>
    /// <param name="_inputs">The new key inputs.</param>
    /// <param name="_rotation">The new rotation.</param>
    public void SetInput(bool[] _inputs, Quaternion _playerRotation, Quaternion _cameraRotation)
    {
        inputs = _inputs;
        transform.rotation = _playerRotation;
        camera.transform.rotation = _cameraRotation;
    }

    private void OnCollisionEnter(Collision collision)
    {
        float slopeAngle = Vector3.Angle(collision.GetContact(0).normal, Vector3.up);

        if (slopeAngle < 45 && slopeAngle > -45)
        {
            isGrounded = true;
            jumpsLeft = maxJumps;
        }
    }

    public void Jump()
    {
        if (health <= 0f)
        {
            return;
        }

        if (jumpsLeft > 0 && canJump)
        {
            if (isWallrunning)
            {
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.AddForce(wallRunNormal * jumpHeight * 10, ForceMode.Impulse);
                rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
                jumpsLeft -= 1;
                isGrounded = false;

                rb.useGravity = true;
                wallRunNormal = new Vector3(0, 0, 0);
                isWallrunning = false;
            }
            else
            {
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
                jumpsLeft -= 1;
                isGrounded = false;
            }
        }
    }

    public void Run()
    {
        if (isCrouching)
        {
            return;
        }

        if (isRunning == false)
        {
            maxSpeed = runMaxSpeed;
            acceleration *= 2;
            isRunning = true;
        }
        else
        {
            maxSpeed = walkMaxSpeed;
            acceleration /= 2;
            isRunning = false;
        }
    }

    public void Crouch()
    {
        if (isRunning)
        {
            return;
        }

        //if (isCrouching == false)
        //{
        //    maxSpeed = crouchMaxSpeed;
        //    acceleration /= 2;
        //    isCrouching = true;
        //}
        //else
        //{
        //    maxSpeed = walkMaxSpeed;
        //    acceleration *= 2;
        //    isRunning = false;
        //}
    }

    public void Wallrun()
    {
        if (inputs[0])
        {
            rb.AddForce(wallRunDirection * acceleration, ForceMode.Force);
        }
        else if (inputs[1])
        {
            rb.AddForce(-wallRunDirection * acceleration, ForceMode.Force);
        }
        else
        {
            if (Vector3.Dot(rb.velocity, wallRunDirection) > 0)
            {
                rb.AddForce(wallRunDirection * -Vector3.Dot(rb.velocity, wallRunDirection) * counterAcceleration, ForceMode.Force);
            }
            else if (Vector3.Dot(rb.velocity, wallRunDirection) < 0)
            {
                rb.AddForce(wallRunDirection * -Vector3.Dot(rb.velocity, wallRunDirection) * counterAcceleration, ForceMode.Force);
            }
        }
    }

    public void ThrowItem()
    {
        if (health <= 0f)
        {
            return;
        }

        if (itemAmount > 0)
        {
            itemAmount--;
            NetworkManager.instance.InstantiateProjectile(shootOrigin).Initialize(camera.transform.forward, throwForce, id);
        }
    }

    public void TakeDamage(float _damage)
    {
        if (health <= 0f)
        {
            return;
        }

        health -= _damage;
        if (health <= 0f)
        {
            weapon.CancelReload();
            rb.isKinematic = true;
            health = 0f;
            canMoveFAndB = false;
            canMoveLAndR = false;
            transform.position = new Vector3(0f, 25f, 0f);
            StartCoroutine(Respawn());
        }

        ServerSend.PlayerHealth(this);
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(5f);

        rb.isKinematic = false;
        health = maxHealth;
        canMoveFAndB = true;
        canMoveLAndR = true;
        ServerSend.PlayerRespawned(this);
    }

    public bool AttemptPickupItem()
    {
        if (itemAmount >= maxItemAmount)
        {
            return false;
        }

        itemAmount++;
        return true;
    }

    public void EquipWeapon(int _weaponId)
    {
        if (weapon.isReloading)
        {
            weapon.CancelReload();
        }

        Weapon[] _weapons = GetComponentsInChildren<Weapon>(true);

        foreach (Weapon _weapon in _weapons)
        {
            _weapon.gameObject.SetActive(false);
        }

        weapon = _weapons[_weaponId];
        _weapons[_weaponId].gameObject.SetActive(true);
        ServerSend.PlayerEquipWeapon(id, this, _weaponId);
    }
}
