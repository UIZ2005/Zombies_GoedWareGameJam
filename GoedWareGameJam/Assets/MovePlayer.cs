using UnityEngine;
using UnityEngine.InputSystem;

public class MovePlayer : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 5f;
    public bool quieto = false;

    private Rigidbody2D rb;
    private Vector2 movement;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (animator == null)
            animator = GetComponent<Animator>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }


    public void OnMove(InputValue value)
    {
        movement = value.Get<Vector2>();

        if (quieto)
        {
            movement = Vector2.zero;
            return;
        }

        if (movement != Vector2.zero)
        {
            if (movement.x < 0)
            {
                //transform.rotation = Quaternion.Euler(0f, 180f, 0f);
                //spriteRenderer.flipX = true;
            }
            else
            {
                //transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                //spriteRenderer.flipX = false;
            }

            //animator.SetFloat("XInput", movement.x);
            //animator.SetFloat("YInput", movement.y);
        }
    }


    private void FixedUpdate()
    {
        /*
        if (quieto)
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("IsWalking", false);
            return;
        }*/

        Vector2 direction = movement.normalized;

        rb.linearVelocity = direction * speed;

        /*if (movement != Vector2.zero)
        {
            animator.SetBool("IsWalking", true);
        }
        else
        {
            animator.SetBool("IsWalking", false);
        }*/
    }

    public void ActivarQuieto()
    {
        quieto = true;
        movement = Vector2.zero;
        rb.linearVelocity = Vector2.zero;
        animator.SetBool("IsWalking", false);
    }

    public void DesactivarQuieto()
    {
        quieto = false;
        movement = Vector2.zero;
        rb.linearVelocity = Vector2.zero;
    }


}
