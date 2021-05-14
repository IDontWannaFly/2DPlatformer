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
    public SurfaceSensorScript wallSensor;
    public LayerMask enemyLayers;
    public Transform attackPoint;
    public float damage = 20f;
    public float health = 100f;
    public float dashDelay = 2f;

    private Rigidbody2D m_body;
    private Animator m_animator;
    private float m_attackDelay;
    private bool m_isAttacking = false;
    private float m_curHealth;
    private bool m_isAbleToMove = true;
    private float m_dashDelay;
    private bool m_isBackground = false;

    
    private float m_wallWalkAxis = 0f;
    private bool m_isWallWalking = false;


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

        UpdateDelays();

        var input = Input.GetAxis("Horizontal");

        if(input < 0 && transform.localScale.x > 0)
            Flip();
        else if(input > 0 && transform.localScale.x < 0)
            Flip();
        else if(input == 0)
            StopWallWalking();

        var jump = Input.GetButtonDown("Jump");
        if(jump)
            Jump();

        if(Input.GetButtonDown("Fire1"))
            Attack();

        if(Input.GetButtonDown("Dash"))
            Dash();

        Move(input);

        if(wallSensor.State() && !groundSensor.State() && input == transform.localScale.x)
            m_body.velocity = new Vector2(m_body.velocity.x, m_body.velocity.y > 0 ? m_body.velocity.y : 0);

        UpdateAnimator();
    }

    public IEnumerator ChangeWalkAxis(float oldValue, float newValue, float duration) {
        for (float t = 0f; t < duration; t += Time.deltaTime) {
            m_wallWalkAxis = Mathf.Lerp(oldValue, newValue, t / duration);
            yield return null;
        }
        m_wallWalkAxis = newValue;
    }

    private void UpdateDelays(){
        if(m_dashDelay > 0)
            m_dashDelay -= Time.deltaTime;
        if(m_attackDelay > 0)
            m_attackDelay -= Time.deltaTime;
    }

    private void Jump(){
        if(wallSensor.State() && !groundSensor.State()){
            m_isAbleToMove = false;
            m_animator.SetTrigger("Jump");
            m_body.velocity = new Vector2(transform.localScale.x * -1 * speed, jumpForce);
            Invoke("SetAbleToMove", 0.2f);

        } else if(groundSensor.State()) {
            groundSensor.SetDelay(0.2f);
            m_animator.SetTrigger("Jump");
            m_body.velocity = new Vector2(m_body.velocity.x, jumpForce);
        } else if(m_isBackground){
            StartWallWalking();
        }
    }

    private void StartWallWalking(){
        m_isWallWalking = true;
        StartCoroutine(ChangeWalkAxis(0, 360, 2));
        Invoke("StopWallWalking", 2);
    }

    private void StopWallWalking(){
        m_isWallWalking = false;
        StopCoroutine(ChangeWalkAxis(0, 360, 2));
        m_wallWalkAxis = 0f;
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
        m_body.velocity = new Vector2(input * speed, m_isWallWalking && input != 0 ? speed * Mathf.Sin(m_wallWalkAxis * Mathf.Deg2Rad) / 2  : m_body.velocity.y);
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

    private void Dash(){
        if(m_dashDelay > 0 || !m_isAbleToMove || !groundSensor.State())
            return;
        m_dashDelay = dashDelay;
        m_animator.SetTrigger("Dash");
        m_isAbleToMove = false;
        m_body.velocity = new Vector2(speed * 3 * transform.localScale.x, m_body.velocity.y);
        Invoke("SetAbleToMove", 0.375f);
    }

    private void SetAbleToMove(){
        m_isAbleToMove = true;
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

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.layer == LayerMask.NameToLayer("Background"))
            m_isBackground = true;
    }

    private void OnTriggerExit2D(Collider2D other) {
        if(other.gameObject.layer == LayerMask.NameToLayer("Background")){
            m_isBackground = false;
            StopWallWalking();
        }
    }
}