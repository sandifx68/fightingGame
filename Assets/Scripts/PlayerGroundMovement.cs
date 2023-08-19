using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerGroundMovement : MonoBehaviour
{
    [SerializeField]
    private Rigidbody rb;
    [SerializeField]
    private PlayerParkour parkour=null;
    [SerializeField]
    private Camera cam = null;
    [SerializeField]
    private GameObject camHolder = null;
    [SerializeField]
    private float cameraRotationLimit = 85f;
    [SerializeField]
    private GameObject PlayerModel = null;
    [SerializeField]
    private GameObject PlayerHolder = null;
    [SerializeField]
    private Animations_scr a_scr =null;
    [SerializeField]
    private GameObject skull = null;
    [SerializeField]
    private LayerMask mask;
    [SerializeField]
    public float jumpVelocity = 0f;
    private float jump = 0f;

    private Vector3 velocity = Vector3.zero;
    private float xVelocity = 0f;


    private float rotationY = 0f;
    private float rotationX = 0f;
    private float rotationuntil = 0;

    public float currentRotationX = 0f;
    public float currentRotationY = 0f;
    [HideInInspector]
    public bool isWalking = false;
    [HideInInspector]
    public bool isGrounded = true;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        camHolder.transform.position = skull.transform.position;
    }

    public void Move(Vector3 _velocity, float _jump)
    {
        velocity = _velocity;
        jump = _jump;
    }

    public void Shimmy(float _xVelocity)
    {
        xVelocity = _xVelocity;
    }

    public void Rotate(float _rotationX, float _rotationY)
    {
        rotationX = _rotationX;
        rotationY = _rotationY;
    }

    private void FollowPlayerHolderGround()
    {
        camHolder.transform.position = skull.transform.position + 0.1f * PlayerModel.transform.TransformDirection(Vector3.forward)+0.12f * PlayerModel.transform.TransformDirection(Vector3.up);
        PlayerModel.transform.position = transform.position;
    }

    private void CheckIfGrounded()
    {
        RaycastHit hit;
        if(Physics.Raycast(PlayerHolder.transform.position,new Vector3(0,-1,0), out hit, 0.1f))
        {
            isGrounded = true;    
        }
        else
        {
            isGrounded = false;
        }
    }

    private void CheckIfOvershootTurnAngle()
    {
        
        if(Mathf.Abs(Quaternion.Dot(PlayerModel.transform.rotation,transform.rotation)) > 90)
        {
            PlayerModel.transform.rotation = transform.rotation;
            rotationuntil = 0;
            Debug.Log("checkifovershoot");
        }
    }

    void FixedUpdate()
    {
        if(!parkour.isHanging && !parkour.isGettingUp)
        {
            PerformMovement();
            PerformRotation();
            CheckIfOvershootTurnAngle();
            FollowPlayerHolderGround();
        }
        CheckIfGrounded();
    }

    public void CarryOverRotation(float _currentRotationX, float _currentRotationY)
    {
        currentRotationX = _currentRotationX;
        currentRotationY = _currentRotationY;
    }

    public void Jumping(string type)
    {
        rb.velocity = new Vector3(0,jumpVelocity,0);
    }

#region  Perform movement and rotation
    private void PerformMovement()
    {
        if(jump>0 && isGrounded)
        {
            if(velocity==Vector3.zero)
            {
                Jumping("Jump");
            }
            else {
                Jumping("JumpRun");
            }
        }
        if(velocity!=Vector3.zero)
        {
            isWalking = true;

            rb.MovePosition(transform.position + velocity * Time.fixedDeltaTime);
            PlayerModel.transform.position = transform.position;
            PlayerModel.transform.rotation = transform.rotation;                    
        } 
        else {
            isWalking = false;
        }
    }

    

    private void PerformRotation()
    {
        currentRotationX -= rotationX;
        currentRotationY += rotationY;
        rotationuntil    += rotationY;
        currentRotationX = Mathf.Clamp(currentRotationX, -cameraRotationLimit, cameraRotationLimit);

        PlayerHolder.transform.localEulerAngles = new Vector3(0f, currentRotationY, 0f);
        cam.transform.localEulerAngles = new Vector3(currentRotationX, currentRotationY, 0f);

        float PH = PlayerHolder.transform.rotation.eulerAngles.y, PM = PlayerModel.transform.rotation.eulerAngles.y;
        if (Mathf.Abs(rotationuntil) > 90f && !isWalking)
        {
            if (PlayerController._yRot > 0)
            {
                a_scr.Turn("Turn Right");
            }
            else if (PlayerController._yRot < 0)
            {
                a_scr.Turn("Turn Left");
            }
            rotationuntil = 0;
        }
    }
#endregion

}
