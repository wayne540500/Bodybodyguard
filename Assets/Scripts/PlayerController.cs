using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int playerIndex;
    public int OnWhichController; // 0無 1船身 2砲台
    public Transform originalParent;
    public Transform respwanPoint;
    public Transform groudCheck;
    public float walkSpeed = 5f;
    public LayerMask shipGround;
    public float jumpPower = 0.03f;
    public float jumpTimeRange = 0.5f;
    [Range(0, 1)] public float groundCheckRadious = 0.02f;
    public Animator animator;
    public float fadeSpeed = 1f;
    public Transform teleportOtherSide;
    public PauseMenuController pauseMenuController;
    public MultiplayerEventSystem multiplayerEventSystem;

    private float horizontalMove = 0f;
    private Rigidbody2D rbody2D;
    private Transform Controller_temp;
    private bool freezePosition = false;
    private bool isTeleporting = false;
    private bool isTeleportingAdding = false;
    [SerializeField] private bool canJump = true;
    private PlayerInput playerInput;
    private InputAction m_Move;
    private SpriteRenderer playerSprite;
    private float fadeTimer = 1f;

    const float gravityScale = 9.8f;

    private void Awake()
    {
        originalParent = transform.parent;
        respwanPoint = originalParent;
        playerInput = GetComponent<PlayerInput>();
        //playerIndex = playerInput.user.index;
        // var playerUser = playerInput.user;
        // playerUser = GameDataManager.playerDatas[playerIndex].input.user;
        m_Move = playerInput.actions["Move"];
        rbody2D = GetComponent<Rigidbody2D>();
        rbody2D.gravityScale = 1f;
        pauseMenuController = GameObject.FindGameObjectWithTag("PauseMenu").GetComponent<PauseMenuController>();
        multiplayerEventSystem = GetComponentInChildren<MultiplayerEventSystem>();
        playerSprite = GetComponentInChildren<SpriteRenderer>();
    }


    void Update()
    {
        Collider2D[] collider2Ds = Physics2D.OverlapCircleAll(groudCheck.position, groundCheckRadious, shipGround);
        if (collider2Ds.Length > 0)
            canJump = true;
        else
            canJump = false;

        if (isTeleporting)
        {
            SetPlayerFreezed(1);

            if (fadeTimer == 1f)
                PlayTeleportSound();

            if (!isTeleportingAdding)
            {
                fadeTimer -= Time.deltaTime * fadeSpeed;
                if (fadeTimer <= 0f)
                {
                    fadeTimer = 0f;
                    isTeleportingAdding = true;
                    TeleportToOtherSide();
                }
                playerSprite.material.SetFloat("_Fade", fadeTimer);
            }
            else
            {
                fadeTimer += Time.deltaTime * fadeSpeed;
                if (fadeTimer >= 1f)
                {
                    fadeTimer = 1f;
                    isTeleportingAdding = false;
                    isTeleporting = false;
                    SetPlayerFreezed(0);
                }
                playerSprite.material.SetFloat("_Fade", fadeTimer);
            }
        }

        if (freezePosition || Time.timeScale == 0)
        {
            animator.SetBool("isWalking", false);
            return;
        }

        Move(m_Move);


    }

    public void SetPlayerFreezed(int onOrOff)
    {
        bool set = onOrOff == 1 ? true : false;
        freezePosition = set;
        if (set)
        {
            rbody2D.velocity = Vector2.zero;
            rbody2D.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        }
        else
            rbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    public void Move(InputAction context)
    {
        horizontalMove = Mathf.Abs(context.ReadValue<Vector2>().y) > 0.25f ? 0 : context.ReadValue<Vector2>().x;

        if (horizontalMove == 0)
            animator.SetBool("isWalking", false);
        else
            animator.SetBool("isWalking", true);


        if (horizontalMove > 0)
        {
            horizontalMove = 1;
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        if (horizontalMove < 0)
        {
            horizontalMove = -1;
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }

        rbody2D.velocity = new Vector2(horizontalMove * walkSpeed, rbody2D.velocity.y);
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (Time.timeScale == 0f)
            return;
        if (context.performed)
        {
            if (canJump)
            {
                rbody2D.AddForce(new Vector2(0f, jumpPower));

            }

            if (transform.parent.CompareTag("ShooterController"))
            {
                ExitShooterController();
            }

            if (transform.parent.CompareTag("ShipMoveController"))
            {
                ExitShipMoveController();
            }
        }


    }

    public void Back(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;
        Debug.Log("Back");

        if (transform.parent.CompareTag("ShooterController"))
        {
            ExitShooterController();
        }

        if (transform.parent.CompareTag("ShipMoveController"))
        {
            ExitShipMoveController();
        }
    }

    public void Pause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            bool isPaused = pauseMenuController.isPaused;
            if (isPaused)
                pauseMenuController.Resume(playerIndex + 1);
            else
                pauseMenuController.Pause(playerIndex + 1, multiplayerEventSystem);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {


        if (other.transform.CompareTag("ShooterController"))
        {
            if (other.transform.childCount > 1)
                return;
            Controller_temp = other.transform;
            EnterShooterController();
        }

        if (other.transform.CompareTag("ShipMoveController"))
        {
            if (other.transform.childCount > 1)
                return;
            Controller_temp = other.transform;
            EnterShipMoveController();
        }
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(groudCheck.position, groundCheckRadious);

    }

    public void ExitShooterController()
    {
        OnWhichController = 0;
        animator.SetBool("isOnControl", false);
        freezePosition = false;
        transform.parent = originalParent;
        SetPlayerFreezed(0);
        rbody2D.gravityScale = 1;
        Controller_temp.GetComponent<ShooterController>().isOnControl = false;
        Controller_temp.GetComponent<ShooterController>().isShootting = false;
    }

    private void EnterShooterController()
    {
        OnWhichController = 2;
        animator.SetBool("isOnControl", true);
        Controller_temp.GetComponent<ShooterController>().isOnControl = true;
        freezePosition = true;
        transform.parent = Controller_temp;
        rbody2D.gravityScale = 0;
        transform.localPosition = new Vector3(0.08f, 0.35f, 0f);
        transform.localEulerAngles = Vector3.zero;
        SetPlayerFreezed(1);
    }

    public void ExitShipMoveController()
    {
        OnWhichController = 0;
        animator.SetBool("isOnControl", false);
        freezePosition = false;
        transform.parent = originalParent;
        SetPlayerFreezed(0);
        rbody2D.gravityScale = 1;
        Controller_temp.GetComponent<ShipController>().isOnControl = false;
        SoundManager.Instance.StopPlaySound(SoundManager.SoundType.booster);
    }

    private void EnterShipMoveController()
    {
        OnWhichController = 1;
        animator.SetBool("isOnControl", true);
        Controller_temp.GetComponent<ShipController>().isOnControl = true;
        freezePosition = true;
        transform.parent = Controller_temp;
        rbody2D.gravityScale = 0;
        transform.localPosition = new Vector3(0.08f, 0.35f, 0f);
        transform.localEulerAngles = Vector3.zero;
        SetPlayerFreezed(1);
    }

    public void TeleportToOtherSide()
    {
        transform.position = teleportOtherSide.position;
    }

    public void PlayTeleportSound()
    {
        SoundManager.Instance.PlaySoundOneShot(SoundManager.SoundType.TeleportPipelineSound, false);
    }

    public void SetTeleport()
    {
        isTeleporting = true;
    }
}
