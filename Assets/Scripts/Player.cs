using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float jumpForce = 5f;
    [SerializeField] float groundCheckDistance = .5f;
    [SerializeField] float wallCheckDistance = .5f;
    [SerializeField] LayerMask groundLayer;

    // Animation states
    bool isRunning = false;
    bool isFacingRight = true;
    bool isGrounded = false;
    bool isFacingWall = false;
    bool isSliding = false;

    Rigidbody2D rb2d;
    Animator animator;
    float xInput;
    float yInput;
    bool isJumpPressed;

    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        isFacingRight = transform.rotation.eulerAngles.y == 0;
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
        HandleMovement();  
        UpdateAnimation();
    }

    private void HandleInput()
    {
        xInput = Input.GetAxis("Horizontal");
        isJumpPressed = Input.GetKeyDown(KeyCode.Space);
    }

    private void HandleMovement()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);
        isFacingWall = Physics2D.Raycast(transform.position, Vector2.right * (isFacingRight ? 1 : -1), wallCheckDistance, groundLayer);
        isSliding = isFacingWall && rb2d.velocity.y < 0;

        Vector2 vec = rb2d.velocity;

        if (!isFacingWall)
        {
            vec.x = xInput * moveSpeed;
        }
        
        if (isGrounded && isJumpPressed)
        {
            vec.y = jumpForce;
        }

        if (isSliding)
        {
            vec.y = vec.y * 0.5f;
        }

        rb2d.velocity = vec;
    }

    void UpdateAnimation()
    {
        animator.SetBool("isGrounded", isGrounded);
        animator.SetBool("isSliding", isSliding);
        animator.SetFloat("xVelocity", rb2d.velocity.x);
        animator.SetFloat("yVelocity", rb2d.velocity.y);

        if (xInput != 0)
        {
            if ((xInput > 0 && !isFacingRight) || (xInput < 0 && isFacingRight))
            {
                isFacingRight = !isFacingRight;
                Flip();
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - groundCheckDistance));
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x + (isFacingRight ? wallCheckDistance : -wallCheckDistance), transform.position.y));
    }

    void Flip()
    {
        transform.Rotate(0, 180, 0);
    }
}