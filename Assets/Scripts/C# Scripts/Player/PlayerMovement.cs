using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    private Vector2 movement;
    private Animator animator;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Get input from Horizontal and Vertical axes
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Normalize the movement vector to prevent faster diagonal movement
        if (movement.magnitude > 1)
        {
            movement.Normalize();
        }

        // Set animator parameters
        animator.SetFloat("Horizontal", Mathf.Abs(movement.x));
        animator.SetFloat("Vertical", movement.y);
        animator.SetBool("isWalking", movement.sqrMagnitude > 0);

        // Flip the sprite based on the horizontal movement
        if (movement.x < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (movement.x > 0)
        {
            spriteRenderer.flipX = false;
        }
    }

    void FixedUpdate()
    {
        // Move the player
        rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
    }
}
