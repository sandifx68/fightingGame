using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParkour : MonoBehaviour
{
    private Animations_scr a_scr;
    private Rigidbody rb;
    private PlayerGroundMovement groundMovement;

    [HideInInspector]
    public bool isHanging = false;
    [HideInInspector]
    public bool isGettingUp = false;
    [SerializeField]
    private GameObject PlayerModel=null;
    [SerializeField]
    private GameObject PlayerHolder=null;
    [SerializeField]
    private GameObject skull=null;
    [SerializeField]
    private GameObject camHolder=null;
    [SerializeField]
    private Camera cam=null;

    private float cameraRotationLimitX = 85f;
    private float cameraRotationLimitY = 110f;
    private float rotationY = 0f;
    private float rotationX = 0f;
    private float currentRotationX = 0f;
    private float currentRotationY = 0f;

    private Vector3 velocity;

    private Vector3 yHandsOffset = new Vector3(0f,1.75f,0f);
    private float xHandsDist = 0.5f;
    private Vector3 yWallCheck = new Vector3 (0f,1.3f,0f);
    private float xWallCheckDist = 0.5f;
    private RaycastHit hands, chest, chestPoint;
    private float objectTopEnd;
    private float playerToWallHangDistance = 0.25f;
    
    
    public void CarryOverRotation(float _currentRotationX, float _currentRotationY)
    {
        currentRotationX = _currentRotationX;
        currentRotationY = _currentRotationY;
    }

    void Start()
    {
        groundMovement = GetComponent<PlayerGroundMovement>();
        rb             = GetComponent<Rigidbody>();
        a_scr = PlayerModel.GetComponent<Animations_scr>();
    }

    public void Move(Vector3 _velocity)
    {
        velocity = _velocity;
    }
    public void Rotate(float _rotationX, float _rotationY)
    {
        rotationX = _rotationX;
        rotationY = _rotationY;
    }

    private void FollowPlayerHolderHanging()
    {
        camHolder.transform.position = skull.transform.position; //+ 0.1f * PlayerModel.transform.TransformDirection(Vector3.forward);
        if(!isGettingUp)
        {
            PlayerModel.transform.position = transform.position;
        }
        PlayerModel.transform.rotation = Quaternion.LookRotation(chestPoint.normal * -1f);

    }

    private void CheckIfAvailableClimb()
    {
        if(Physics.Raycast(PlayerHolder.transform.position+yWallCheck,transform.TransformDirection(Vector3.forward), out chest, xWallCheckDist) && //There is a wall in front
        !Physics.Raycast(PlayerHolder.transform.position+yHandsOffset,transform.TransformDirection(Vector3.forward), out hands, xHandsDist )  &&   //There is no wall at hand level
        !isHanging && !groundMovement.isGrounded)     
        {
            JumpToHang();
        }
    }
    
    private void CheckHangLevel()
    {
        if(PlayerHolder.transform.position.y+yHandsOffset.y-objectTopEnd < 0.001 && isHanging)
        {
            rb.constraints = RigidbodyConstraints.FreezePositionY;
        }
    }  

    void FixedUpdate()
    {
        if(isHanging)
        {
            FollowPlayerHolderHanging();
            PerformMovement();
            PerformRotation();
            CheckHangLevel();
        }
        else
            CheckIfAvailableClimb();
    }
    
    public void JumpToHang()
    {
        currentRotationX = Quaternion.LookRotation(chest.normal * -1f).eulerAngles.x;
        currentRotationY = Quaternion.LookRotation(chest.normal * -1f).eulerAngles.y;
        isHanging = true;
        chestPoint=chest;
        objectTopEnd = chest.transform.position.y + chest.transform.localScale.y/2;
        rb.velocity = Vector3.zero;
        Debug.Log(Quaternion.LookRotation(chest.normal * -1f).eulerAngles.y);
        PlayerHolder.transform.position = new Vector3(chest.point.x, PlayerHolder.transform.position.y, chest.point.z) 
                                          - PlayerModel.transform.TransformDirection(Vector3.forward) * playerToWallHangDistance;
        a_scr.JumpToHangAnimation();
    }

    private void LedgeClimb()
    {
        isGettingUp = true;
        velocity = Vector3.zero;
    }

    private void Shimmy(float angle, float dir)
    {
        transform.Translate(new Vector3(Mathf.Cos(angle*Mathf.Deg2Rad),0f,-Mathf.Sin(angle*Mathf.Deg2Rad)) * dir * Time.fixedDeltaTime,Space.World);
    }

    public void JumpDownFromHang()
    {
        isHanging = false;
        velocity = Vector3.zero;
        rb.constraints = RigidbodyConstraints.None | RigidbodyConstraints.FreezeRotation;
    }

    private void PerformMovement()
    {
        if(velocity!=Vector3.zero &&!isGettingUp)
        {
            if(velocity.z>0 && velocity.y>0)//w+space
            {
                LedgeClimb();
                a_scr.LedgeClimbAnimation();
            } else
            if(velocity.x!=0)//a or d
            {
                Shimmy(PlayerModel.transform.eulerAngles.y,velocity.x);
            } else
            if(velocity.z<0)//s
            {
                a_scr.JumpDownFromHang();
                JumpDownFromHang();
            }
        }
    }
    private void PerformRotation()
    {
        currentRotationX -= rotationX;
        currentRotationY += rotationY;
        currentRotationX = Mathf.Clamp(currentRotationX, -cameraRotationLimitX, cameraRotationLimitX);
        currentRotationY = Mathf.Clamp(currentRotationY, Quaternion.LookRotation(chest.normal * -1f).eulerAngles.y-cameraRotationLimitY, Quaternion.LookRotation(chest.normal * -1f).eulerAngles.y+cameraRotationLimitY);

        PlayerHolder.transform.localEulerAngles = new Vector3(0f, currentRotationY, 0f);
        cam.transform.localEulerAngles = new Vector3(currentRotationX, currentRotationY, 0f);
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(PlayerHolder.transform.position+yWallCheck, transform.TransformDirection(Vector3.forward) * xWallCheckDist);
        Gizmos.DrawRay(PlayerHolder.transform.position+yHandsOffset,transform.TransformDirection(Vector3.forward) * xHandsDist);
    }
}
