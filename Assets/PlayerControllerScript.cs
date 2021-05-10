using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerScript : MonoBehaviour
{
    public float speed;
    public float attackDelay;
    public float attackRange;
    public float jumpForce;
    public SurfaceSensorScript groundSensor;
    public LayerMask enemyLayers;
    public Transform attackPoint;
    public float damage = 20f;
    public float health = 100f;

    private Rigidbody2D m_body;
    private Animator m_animator;
    private float m_attackDelay;
    private bool m_isAttacking = false;
    private float m_curHealth;
    private bool m_isAbleToMove = true;
    // Start is called before the first frame update
    void Start()
    {
        m_body = GetComponent<Rigidbody2D>();
        m_animator = GetComponent<Animator>();
        m_curHealth = health;
    }

    // Update is called once per frame
    void Update()
    {
        if(m_attackDelay > 0f)
            m_attackDelay -= Time.deltaTime;

        var input = Input.GetAxis("Horizontal");

        if(input < 0 && transform.localScale.x > 0)
            Flip();
        else if(input > 0 && transform.localScale.x < 0)
            Flip();

        var jump = Input.GetButtonDown("Jump");
        if(groundSensor.State() && jump)
            Jump();

        if(Input.GetButtonDown("Fire1"))
            Attack();

        Move(input);

        UpdateAnimator();
    }

    private void Jump(){
        groundSensor.SetDelay(0.2f);
        m_animator.SetTrigger("Jump");
        m_body.velocity = new Vector2(m_body.velocity.x, jumpForce);
    }

    private void UpdateAnimator(){
        if(m_body.velocity.x != 0)
            m_animator.SetInteger("AnimState", 1);
        else
            m_animator.SetInteger("AnimState", 0);
        m_animator.SetBool("IsGrounded", groundSensor.State());
    }

    private void Move(float input){
        if(!m_isAbleToMove)
            return;
        m_body.velocity = new Vector2(input * speed, m_body.velocity.y);
    }

    private void Flip(){
        transform.localScale = new Vector2(transform.localScale.x * -1, 1);
    }

    private void Attack(){
        if(m_attackDelay > 0f)
            return;
        m_attackDelay = attackDelay;
        m_isAbleToMove = false;
        m_body.velocity = new Vector2(groundSensor.State() ? 0 : m_body.velocity.x, m_body.velocity.y);
        m_animator.SetTrigger("Attack");
        Invoke("Hit", attackDelay / 1.5f);
    }

    private void Hit(){
        m_isAbleToMove = true;

        Collider2D[] enemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach(var enemy in enemies){
            enemy.GetComponent<EnemyScript>().Hurt(damage);
        }
    }

    private void OnDrawGizmosSelected() {
        if(attackPoint != null)
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
