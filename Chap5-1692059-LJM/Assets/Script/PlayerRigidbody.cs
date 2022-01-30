using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRigidbody : MonoBehaviour
{
    private Animator m_animator;

    private Rigidbody m_rigidBody;
    private bool m_wasGrounded;
    private bool m_isGrounded;
    private List<Collider> m_collisions = new List<Collider>();

    public float m_moveSpeed = 2.0f;
    public float m_jumpForce = 5.0f;
    private float m_jumpTimeStamp = 0;
    private float m_minJumpInterval = 0.25f;

    void Start()
    {
        m_rigidBody = GetComponent<Rigidbody>();
        m_animator = GetComponent<Animator>();
    }

    void Update()
    {
        m_animator.SetBool("Grounded", m_isGrounded);

        PlayerMove();
        JumpingAndLanding();

        m_wasGrounded = m_isGrounded;
    }

    private void PlayerMove()
    {
        //사용자로부터 키 입력값을 받아옴
        //project setting의 inputManager설정값에따라
        //-1, +1값이 리턴됨.
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 moveHorizontal = Vector3.right * h;
        Vector3 moveVertical = Vector3.forward * v;
        Vector3 velocity = (moveHorizontal + moveVertical).normalized;
        //normalized주어진 벡터의 길이를 1로 만들어줌 -> 방향
        transform.LookAt(transform.position + velocity);

        if (Input.GetKey(KeyCode.LeftShift))//대쉬일때 속도를 높임
        {
            velocity *= 2.0f;
        }
        //Vector3는 월드중심
        //transform는 오브젝트 중심.- > Space.World 월드중심으로 이동.
        transform.Translate(velocity * m_moveSpeed * Time.deltaTime, Space.World);
        //m_rigidBody.MovePosition(transform.position + velocity * m_moveSpeed * Time.deltaTime);
        //MovePosition는 목적지를 쓴다.
        //Translate는 방향을 쓴다.
        m_animator.SetFloat("MoveSpeed", velocity.magnitude);
    }

    private void JumpingAndLanding()
    {
        //m_minJumpInterval는 이중점프를 막을 쿨타임
        bool jumpCooldownOver = (Time.time - m_jumpTimeStamp) >= m_minJumpInterval;

        if (jumpCooldownOver && m_isGrounded && Input.GetKey(KeyCode.Space))
        {
            m_jumpTimeStamp = Time.time;
            m_rigidBody.AddForce(Vector3.up * m_jumpForce, ForceMode.Impulse);
            // ForceMode.Impulse는 순간적으로 힘을 준다.
        }

        //애니메이션 바뀌는 순간
        if (!m_wasGrounded && m_isGrounded)
        {
            m_animator.SetTrigger("Land");
        }

        if (!m_isGrounded && m_wasGrounded)
        {
            m_animator.SetTrigger("Jump");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint[] contactPoints = collision.contacts;
        for (int i = 0; i < contactPoints.Length; i++)
        {
            if (Vector3.Dot(contactPoints[i].normal, Vector3.up) > 0.5f)
            {
                if (!m_collisions.Contains(collision.collider))
                {
                    m_collisions.Add(collision.collider);
                }
                m_isGrounded = true;
            }
        }
    }


    private void OnCollisionExit(Collision collision)
    {
        if (m_collisions.Contains(collision.collider))
        {
            m_collisions.Remove(collision.collider);
        }
        if (m_collisions.Count == 0) { m_isGrounded = false; }
    }

    private void OnCollisionStay(Collision collision)
    {
        ContactPoint[] contactPoints = collision.contacts;
        bool validSurfaceNormal = false;
        for (int i = 0; i < contactPoints.Length; i++)
        {
            if (Vector3.Dot(contactPoints[i].normal, Vector3.up) > 0.5f)
            {
                validSurfaceNormal = true; break;
            }
        }

        if (validSurfaceNormal)
        {
            m_isGrounded = true;
            if (!m_collisions.Contains(collision.collider))
            {
                m_collisions.Add(collision.collider);
            }
        }
        else
        {
            if (m_collisions.Contains(collision.collider))
            {
                m_collisions.Remove(collision.collider);
            }
            if (m_collisions.Count == 0) { m_isGrounded = false; }
        }
    }

}
