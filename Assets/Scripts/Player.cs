using System;
using System.Collections;
using System.Collections.Generic;
using PlayerState;
using UnityEngine;

public class Player : MonoBehaviour
{
    private static Player instance;

    public static Player Instance
    {
        get
        {
            if (instance == null) instance = FindObjectOfType<Player>();
            return instance;
        }   
    }
    
    public float moveSpeed = 5f;
    public float rotationSpeed = 720f;
    public float jumpForce = 5f;
    
    public LayerMask groundlayer;
    private bool isGrounded = false;
    
    private Quaternion toRotation;
    [NonSerialized]
    public Animator animator;
    public Rigidbody rb;
    private float rotationCheck = 0;
    private Vector3 nowMove = Vector3.zero;
    private Vector3 prevPushedMove = Vector3.zero;
    private GameObject camera;
    private Vector3 cameraOffset = new Vector3(4.24f, 6f, -4.24f);
    private bool isAttacking = false; 

    private int comboStep = 0;
    private float lastAttackTime = 0f;
    private bool isAttackAble = true;
    
    public float comboResetTime = 1.0f;

    public PlayerFSM playerFsm;
    private AnimationEvent animationEvent;

    public PlayerState.PlayerState playerState;

    public float gravityScale = 1.0f;

    private float[] skillCool = new[] { 0f, 0f };

    public float Testvalue;
    
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        camera = Camera.main.gameObject;

        playerFsm = GetComponent<PlayerFSM>();
        playerFsm.ChangeState(new IdleState(this));

        animationEvent = GetComponent<AnimationEvent>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleRotation();
        HandleSkill();
        playerState = playerFsm.GetState();
    }

    private void FixedUpdate()
    {
        HandleCamera();
        Vector3 gravity = -Vector3.up * gravityScale;
        rb.AddForce(gravity, ForceMode.Acceleration);
    }

    public void HandleAttack(int attackCombo)
    {
        if (Input.GetKeyDown(KeyCode.X) && isAttackAble)
        {
            RotateToKey();

            int nextAttackCombo = attackCombo + 1;
            if (nextAttackCombo > 2) nextAttackCombo = 0;
            playerFsm.ChangeState(new AttackingState(this, nextAttackCombo));
        }
    }

    public void HandleSkill()
    {
        //스킬은 어떤 상황에서든 발동 된다(쿨 제외)
        if (Input.GetKeyDown(KeyCode.A))
        {
            playerFsm.ChangeState(new SkillState(this, 0));
        }
        
        
        if (Input.GetKeyDown(KeyCode.S))
        {
            playerFsm.ChangeState(new SkillState(this, 1));
        }
        
        if (Input.GetKeyDown(KeyCode.D))
        {
            playerFsm.ChangeState(new ShieldDashState(this));
        }

        
    }

    public void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (CheckGrounded())
            {
                playerFsm.ChangeState(new JumpState(this));
                
            }
        }
    }

    public void HandleDash()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //방향 전환
            
            
            playerFsm.ChangeState(new ShieldDashState(this));
        }
    }

    public void ChangeRotationImmediatelyByKey()
    {
        transform.rotation = toRotation;
        Vector3 move = Vector3.zero;
        
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        
        move = new Vector3(horizontal, 0f, vertical);

        if (move != Vector3.zero)
        {
            move = Quaternion.AngleAxis(-45, Vector3.up) * move.normalized;
            toRotation = Quaternion.LookRotation(move, Vector3.up);
            transform.rotation = toRotation;
        }
    }

    public void Jump()
    {
        rb.AddForce(Vector3.up*jumpForce, ForceMode.Impulse);
    }

    public void HandleMovement()
    {
        Vector3 move = Vector3.zero;
        
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        
        move = new Vector3(horizontal, 0f, vertical);
        move = Quaternion.AngleAxis(-45, Vector3.up) * move.normalized;
            
        if (nowMove != move)
        {
            if (prevPushedMove != move)
            {
                prevPushedMove = move;
                rotationCheck = 0f;
            }
            else
            {
                if (rotationCheck <0.05f && playerFsm.GetState() == PlayerState.PlayerState.Moving)
                {
                    rotationCheck += Time.deltaTime;
                }
                else
                {
                    rotationCheck = 0f;
                    nowMove = move;
                }
            }
        }
        
        if (nowMove.magnitude > 0.1f)
        {
            Vector3 newPosition = rb.position + nowMove * moveSpeed * Time.deltaTime;
            rb.MovePosition(newPosition);
            
            //방향 바꾸기
            toRotation = Quaternion.LookRotation(nowMove, Vector3.up);

            if (playerFsm.GetState() == PlayerState.PlayerState.Idle)
            {
                playerFsm.ChangeState(new MovingState(this));
            }
            
        }
        else
        {
            if (playerFsm.GetState() == PlayerState.PlayerState.Moving)
            {
                playerFsm.ChangeState(new IdleState(this));
            }
        }
    }

    void HandleCamera()
    {
        camera.transform.position = transform.position + cameraOffset;
    }

    public bool CheckGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down,
            0.1f, groundlayer);
    }

    public bool IsAnimationEnd(int animationHash)
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.shortNameHash == animationHash && stateInfo.normalizedTime >= 0.98f)
        {
            return true;
        }

        return false;
    }

    public void SetFreezePositionY(bool enable)
    {
        if (enable)
        {
            rb.constraints |= RigidbodyConstraints.FreezePositionY;
        }
        else
        {
            rb.constraints &= ~RigidbodyConstraints.FreezePositionY;
        }
    }

    public void SetAttackAble()
    {
        isAttackAble = true;
    }
    
    public void SetAttackAble(bool enable)
    {
        isAttackAble = enable;
    }

    public void EndAttack()
    {
        playerFsm.ChangeState(new IdleState(this));
        nowMove = Vector3.zero;
        prevPushedMove = Vector3.zero;
    }

    public void HandleRotation()
    {
        //방향 전환
        if (transform.rotation != toRotation)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, 
                toRotation, rotationSpeed * Time.deltaTime);
        }
    }

    
    private void RotateToKey()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        
        Vector3 move = new Vector3(horizontal, 0f, vertical);
        if (move.magnitude > 0.1f)
        {
            move = Quaternion.AngleAxis(-45, Vector3.up) * move.normalized;
        
            toRotation = Quaternion.LookRotation(move, Vector3.up);
            transform.rotation = toRotation;
        }
    }

    public void ChangeState(BaseState state)
    {
        playerFsm.ChangeState(state);
    }

    public void Stop()
    {
        nowMove = Vector3.zero;
        prevPushedMove = Vector3.zero;
    }

    public void TrailOff()
    {
        animationEvent.DisableTrail();
    }
}
