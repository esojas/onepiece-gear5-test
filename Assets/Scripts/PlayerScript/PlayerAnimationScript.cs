using Unity.VisualScripting;
using UnityEngine;

public class PlayerAnimationScript : MonoBehaviour
{
    private Rigidbody rb;
    private string currentAnimation;
    protected bool isAttacking = false;
    PlayerMovementScript playerMovementScript;
    AnimationScript animationScript;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animationScript = GetComponent<AnimationScript>();
        playerMovementScript = GetComponent<PlayerMovementScript>();
    }

    private void OnEnable()
    {
        animationScript.CurrentAnimationEvent += SetAnimation;
    }

    private void OnDisable()
    {
        animationScript.CurrentAnimationEvent -= SetAnimation;
    }

    public void SetAnimation(string animation)
    {
        currentAnimation = animation;
        if (animation == "luffy_idle" || animation == "luffy_walk")
        {
            isAttacking = false;
        }
    }

    private void CheckAnimation()
    {
        if (isAttacking) return;

        bool grounded = playerMovementScript.IsGrounded;
        bool jumping = playerMovementScript.IsJumping;
        bool sprinting = playerMovementScript.IsSprinting;
        bool flying = playerMovementScript.IsFlying;

        if (!grounded && !flying && !jumping)
        {
            animationScript.ChangeAnimation("luffy_jumpHold", .2f);
            return;
        }

        if (sprinting)
        {
            animationScript.ChangeAnimation("luffy_run", .1f);
        }
        else if (rb.linearVelocity.magnitude > 0.1f)
        {
            animationScript.ChangeAnimation("luffy_walk", .1f);
        }
        else
        {
            animationScript.ChangeAnimation("luffy_idle");
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckAnimation();
    }
}
