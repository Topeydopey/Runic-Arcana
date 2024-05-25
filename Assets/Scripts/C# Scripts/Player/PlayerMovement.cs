using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public AudioClip[] walkingSounds; // Array to hold walking sound variants
    private Vector2 movement;
    private Animator animator;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;
    private bool isWalking = false;
    private AudioClip lastPlayedClip;
    private Coroutine walkingSoundCoroutine;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = false; // We will handle looping manually
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

        // Handle walking sound based on movement
        if (movement.sqrMagnitude > 0 && !isWalking)
        {
            isWalking = true;
            walkingSoundCoroutine = StartCoroutine(PlayWalkingSounds());
        }
        else if (movement.sqrMagnitude == 0 && isWalking)
        {
            isWalking = false;
            StopCoroutine(walkingSoundCoroutine);
            audioSource.Stop();
        }
    }

    void FixedUpdate()
    {
        // Move the player
        rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
    }

    private IEnumerator PlayWalkingSounds()
    {
        while (isWalking)
        {
            PlayRandomWalkingSound();
            yield return new WaitForSeconds(GetWalkingSoundInterval());
        }
    }

    private void PlayRandomWalkingSound()
    {
        if (walkingSounds.Length > 0)
        {
            AudioClip newClip;
            do
            {
                newClip = walkingSounds[Random.Range(0, walkingSounds.Length)];
            } while (newClip == lastPlayedClip);

            lastPlayedClip = newClip;
            audioSource.clip = newClip;
            audioSource.Play();
        }
    }

    private float GetWalkingSoundInterval()
    {
        // Calculate the interval based on the player's speed
        return audioSource.clip.length / (speed * movement.magnitude);
    }
}
