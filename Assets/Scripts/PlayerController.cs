using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 inputDir;
    private Animator anim;
    private SpriteRenderer render;

    [SerializeField]
    private float maxSpeed;
    [SerializeField]
    private float movePower;
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

    public void Move()
    {
        if (inputDir.x < 0 && rb.velocity.x > -maxSpeed)
            // ���� �Է��� �������� ���� ����
            rb.AddForce(Vector2.right * inputDir.x * movePower, ForceMode2D.Force);

        else if (inputDir.x > 0 && rb.velocity.x < maxSpeed)
            rb.AddForce(Vector2.right * inputDir.x * movePower, ForceMode2D.Force);
    }

    private void Jump()
    {
            rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);

        anim.SetTrigger("Jump");
    }

    // �̵��Լ�
    private void OnMove(InputValue value)
    {
        inputDir = value.Get<Vector2>();
        // ���ʰ��� ������ �����⶧���� �������� ����
        anim.SetFloat("MoveSpeed", Mathf.Abs(inputDir.x));

        if(inputDir.x > 0)
        {
            render.flipX = false;
        }

        else if (inputDir.x < 0)
        {
            render.flipX = true;
        }
    }

    // �����Լ�
    private void OnJump(InputValue value)
    {
        Jump();
    }

    // ���� �浹 �Լ�
    private void OnCollisionEnter2D(Collision2D collision)
    {
        anim.SetBool("IsGround", true);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        anim.SetBool("IsGround", false);
    }
}
