using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    public float health = 100f;
    public float attackDelay = 3f;
    public float speed = 4f;
    public float hurtDelay = 0.5f;
    public LayerMask enemyLayers;
    public float damage;

    protected Animator m_animator;
    protected float m_curHealth;
    protected float m_curHurtDelay;
    protected int m_curDir = 1;
    // Start is called before the first frame update
    void Start()
    {
        m_animator = GetComponent<Animator>();
        m_curHealth = health;
        StartChild();
    }

    void Update(){
        if(m_curHurtDelay > 0)
            m_curHurtDelay -= Time.deltaTime;
        UpdateChild();
    }

    public virtual void Hurt(float damage){
        Debug.Log("Hurt");
        m_curHurtDelay = hurtDelay;
        m_curHealth -= damage;
        if(m_curHealth <= 0)
            Die();
    }

    protected void Flip(){
        m_curDir *= -1;
        transform.localScale = new Vector3(m_curDir, 1, 1);
    }

    protected abstract void StartChild();
    protected abstract void UpdateChild();
    protected abstract void LookForEnemies();
    protected abstract void ChaseEnemy();
    protected abstract void Attack();
    protected abstract void Die();
    protected abstract void UpdateAnimator();
}
