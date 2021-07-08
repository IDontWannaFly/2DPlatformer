using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class EnemyBandit : Enemy, EnemyDetector.EnemyDetectorCallback
{
    public EnemyDetector detector;
    public float lookTime;
    public float attackRange;
    public Transform attackPoint;

    private State m_state = State.IDLE;
    private Rigidbody2D m_body;
    private Transform m_objectToChase;
    private float m_flipDelay;

    protected override void StartChild()
    {
        m_body = GetComponent<Rigidbody2D>();
        detector.AttachCallback(this);
        m_flipDelay = lookTime;
        m_animator.SetInteger("AnimState", 1);
    }

    public void OnDetected(Collider2D collider){
        m_objectToChase = collider.transform;
        ChaseEnemy();
    }
    public void OnLost(Collider2D collider){
        m_state = State.IDLE;
        m_animator.SetInteger("AnimState", 1);
        m_objectToChase = null;
    }

    protected override void UpdateChild()
    {
        if(m_flipDelay > 0)
            m_flipDelay -= Time.deltaTime;
        switch(m_state){
            case State.IDLE:
                if(m_flipDelay <= 0){
                    m_flipDelay = lookTime;
                    Flip();
                }
                break;
            case State.CHASE:
                ToChase();
                break;
            case State.COMBAT:
                if(Mathf.Sqrt(Mathf.Pow(transform.position.x - m_objectToChase.position.x, 2)) > attackRange * 6)
                    ChaseEnemy();
                break;
        }
    }

    protected override void ChaseEnemy()
    {
        m_animator.SetInteger("AnimState", 2);
        if((m_curDir > 0 && transform.position.x < m_objectToChase.position.x) || (m_curDir < 0 && transform.position.x > m_objectToChase.position.x))
            Flip();
        m_state = State.CHASE;
    }

    private void ToChase(){
        if(m_objectToChase != null){
            m_body.velocity = new Vector2(speed * m_curDir * -1, m_body.velocity.y);
            if(Mathf.Sqrt(Mathf.Pow(transform.position.x - m_objectToChase.position.x, 2)) < attackRange * 3){
                Attack();
            }
        }
    }

    protected override void LookForEnemies()
    {
        m_state = State.IDLE;
    }

    protected override void Attack()
    {
        m_body.velocity = Vector2.zero;
        m_state = State.COMBAT;
        m_animator.SetInteger("AnimState", 1);
        StartCoroutine(Fight());
    }

    private IEnumerator Fight(){
        while(m_state == State.COMBAT){
            m_animator.SetTrigger("Attack");
            yield return new WaitForSeconds(0.5f);
            DealDamage(damage);
            yield return new WaitForSeconds(attackDelay);
        }
    }

    private void DealDamage(float damage){
        var enemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        foreach(var enemy in enemies){
            enemy.GetComponent<PlayerControllerScript>().Hurt(damage);
        }
    }

    protected override void Die()
    {
        m_animator.SetTrigger("Death");
        enabled = false;
        detector.enabled = false;
        m_body.simulated = false;
        //GetComponent<BoxCollider2D>().enabled = false;
    }

    public override void Hurt(float damage)
    {
        m_animator.SetTrigger("Hurt");
        base.Hurt(damage);
    }

    protected override void UpdateAnimator()
    {
        switch(m_state){
            case State.IDLE:
            case State.COMBAT:
                m_animator.SetInteger("AnimState", 1);
                break;
            case State.CHASE:
                m_animator.SetInteger("AnimState", 2);
                break;
        }
    }

    private enum State{
        IDLE,
        CHASE,
        COMBAT
    }
}
