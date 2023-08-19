using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerGroundMovement))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float speed=2f;
    [SerializeField]
    private float runspeed=6f;
    [SerializeField]
    private float lookSensitivity=3f;
    [SerializeField]
    private float crouchspeed=1f;
    [SerializeField]
    private Animations_scr a_scr = null;

    private PlayerParkour parkour;
    
    private PlayerGroundMovement groundMovement;

    public static float _yRot;

    public static bool isJumping = false;

    public static bool collidesWall = false;

    private bool _valueToBool(float value)
    {
        if(value>0)
            return true;
        else return false;
    }
    private float _currentSpeed(bool isRunning, bool isCrouching)
    {
        if(isRunning)
            return runspeed;
        if (isCrouching)
            return crouchspeed;
        return speed;
    }

    void Start()
    {
        groundMovement = GetComponent<PlayerGroundMovement>();
        parkour        = GetComponent<PlayerParkour>();
    }
    void Update()
    {
        //Calculate movement velocity as 3d vector
        float _xMov = Input.GetAxisRaw("Horizontal");Vector3 _movHorizontal = transform.right * _xMov;
        float _zMov = Input.GetAxisRaw("Vertical");  Vector3 _movVertical   = transform.forward * _zMov;
        float _jump = Input.GetAxis("Jump");
              _yRot = Input.GetAxis("Mouse X");      float   _rotationY     = _yRot * lookSensitivity;
        float _xRot = Input.GetAxis("Mouse Y");      float   _rotationX     = _xRot * lookSensitivity;
        float _run  = Input.GetAxis("Run");          bool     run           = _valueToBool(_run);
        float _ctrl = Input.GetAxis("Crouch");       bool     ctrl          = _valueToBool(_ctrl);        

        Vector3 _velocity = Vector3.zero;
        float _xWorldVelocity = _xMov * speed;//Unlike _movHorizontal, this is relative to the world, not holder
        
        _velocity = (_movHorizontal + _movVertical).normalized * _currentSpeed(run,ctrl);

        //Apply movement & rotation
        if(!parkour.isHanging)
        {
            groundMovement.Move(_velocity,_jump);
            groundMovement.Rotate(_rotationX, _rotationY);
        } else {
            parkour.Move(new Vector3(_xWorldVelocity,_jump,_zMov));
            parkour.Rotate(_rotationX,_rotationY);
        }
        a_scr.SetAnimationTriggers(_xMov,_zMov,_jump,run,ctrl);
    }
}
