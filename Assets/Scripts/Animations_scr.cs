using UnityEngine;

public class Animations_scr : MonoBehaviour
{
    [SerializeField]
    private GameObject PlayerHolder = null;
    [SerializeField]
    private Transform Holder = null;

    private Animator anim;
    private Rigidbody rb;
    private PlayerGroundMovement groundMovement;
    private PlayerParkour parkour;


    void Start()
    {
        anim           = GetComponent<Animator>();
        groundMovement = PlayerHolder.GetComponent<PlayerGroundMovement>();
        parkour        = PlayerHolder.GetComponent<PlayerParkour>();
        rb             = PlayerHolder.GetComponent<Rigidbody>();
    }

    private void HolderToModel()
    {
        PlayerHolder.transform.position = transform.position;
        parkour.isHanging=false;
        parkour.isGettingUp = false;
        rb.constraints = RigidbodyConstraints.None | RigidbodyConstraints.FreezeRotation;
    }

    public void SetAnimationTriggers(float inputH, float inputV, float jump, bool run,bool ctrl)
    {
        anim.SetFloat("inputH",inputH);
        anim.SetFloat("inputV",inputV);
        anim.SetFloat("jump", jump);
        anim.SetBool("run", run);
        anim.SetBool("ctrl", ctrl);
        anim.SetBool("isGrounded", groundMovement.isGrounded);
    }

    public void JumpToHangAnimation()
    {
        anim.SetTrigger("JumpToHang");   
    }
    public void LedgeClimbAnimation()
    {
        anim.SetTrigger("ClimbLedge");
    }
    public void JumpDownFromHang()
    {
        anim.SetTrigger("JumpDownFromHang");
    }

    public void Turn(string rightleft)
    {
        transform.rotation = Holder.rotation;
        anim.SetTrigger(rightleft);
    }
}