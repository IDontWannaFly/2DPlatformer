using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerScript : MonoBehaviour
{
    public float health;


    private float m_curHealth;
    private PlayerMovementScript m_movement;
    private PlayerCombatScript m_combat;
    private PlayerState m_state = new PlayerState();
    private Animator m_animator;

    private void Start() {
        m_animator = GetComponent<Animator>();
        m_movement = GetComponent<PlayerMovementScript>();
        m_combat = GetComponent<PlayerCombatScript>();
        m_movement.BindState(m_state);
        m_combat.BindState(m_state);
        m_curHealth = health;
    }

    private void Update() {
        
    }

    public void Hurt(float damage){
        m_curHealth -= damage;
        if(m_curHealth <= 0)
            Die();
        else
            m_animator.SetTrigger("Hurt");
    }

    private void Die(){
        m_animator.SetTrigger("Death");
        enabled = false;
        m_movement.enabled = false;
        m_combat.enabled = false;
        GetComponent<Rigidbody2D>().simulated = false;
        GetComponent<CapsuleCollider2D>().enabled = false;
    }
}
