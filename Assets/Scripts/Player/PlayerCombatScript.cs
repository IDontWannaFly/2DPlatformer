using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatScript : MonoBehaviour
{
    public SurfaceSensorScript groundSensor;
    public SurfaceSensorScript wallSensor;
    public float attackDelay = 0.6f;

    private PlayerState m_state;
    private Animator m_animator;
    private float m_curAttackDelay = 0f;
    // Start is called before the first frame update
    void Start()
    {
        m_animator = GetComponent<Animator>();
    }

    public void BindState(PlayerState state){
        m_state = state;
    }

    // Update is called once per frame
    void Update()
    {
        if(m_curAttackDelay > 0)
            m_curAttackDelay -= Time.deltaTime;
        var attack = Input.GetButtonDown("Fire1");

        if(attack && m_curAttackDelay <= 0){
            m_curAttackDelay = attackDelay;
            if(groundSensor.State())
                if(m_state.state != PlayerState.State.DASH)
                    SimpleAttack();
                else
                    DashAttack();
            else 
                AirAttack();
        }
    }

    private void SimpleAttack(){
        m_animator.SetTrigger("Attack");
        m_state.state = PlayerState.State.ATTACK;
        StartCoroutine(Hit(0.25f));
    }

    private void AirAttack(){
        m_animator.SetTrigger("AttackJump");
        m_state.state = PlayerState.State.ATTACK;
        StartCoroutine(JumpHit(0.25f));
    }

    private void DashAttack(){
        m_animator.SetTrigger("AttackDash");
        m_state.state = PlayerState.State.ATTACK;
        StartCoroutine(Hit(0.3f));
    }

    private IEnumerator JumpHit(float timer){
        yield return new WaitForSeconds(timer);
        while(!groundSensor.State()){
            yield return null;
        }

        m_state.state = PlayerState.State.DEFAULT;
    }

    private IEnumerator Hit(float timer){
        yield return new WaitForSeconds(timer);
        if(m_state.state != PlayerState.State.ATTACK)
            yield break;

        m_state.state = PlayerState.State.DEFAULT;
        Debug.Log("Hit");
    }
}
