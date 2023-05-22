using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.InputSystem;

public class SelfPlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 dir;
    private Animator anim;
    private SpriteRenderer render;

    [SerializeField]
    private float maxSpeed;
    [SerializeField]
    private float speed;
    [SerializeField]
    private float jumpPower;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        render = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        if (dir.x < 0 && rb.velocity.x > - maxSpeed)
        {
            rb.AddForce(Vector2.right * dir.x * speed, ForceMode2D.Force);
        }

        else if (dir.x > 0 &&  rb.velocity.x < maxSpeed)
        {
            rb.AddForce(Vector2.right * dir.x * speed, ForceMode2D.Force);        
        }
    }

    private void Jump()
    {
        rb.AddForce(Vector2.up * speed, ForceMode2D.Impulse);
        anim.SetTrigger("Jump");
    }

    private void OnMove(InputValue value)
    {
        dir = value.Get<Vector2>();

        anim.SetFloat("MoveSpeed", Mathf.Abs(dir.x));

        if (dir.x > 0)
        {
            render.flipX = false;
        }

        else if (dir.x < 0)
        {
            render.flipX = true;
        }
    }

    private void OnJump(InputValue value)
    {
        Jump();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        anim.SetBool("IsGround", true);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        anim.SetBool("IsGround", false);
    }
}
