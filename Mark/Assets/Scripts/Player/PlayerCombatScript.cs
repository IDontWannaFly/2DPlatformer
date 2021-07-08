using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatScript : MonoBehaviour
{
    public SurfaceSensorScript groundSensor;
    public SurfaceSensorScript wallSensor;
    public float attackDelay = 1f;
    public float attackRange = 1f;
    public LayerMask enemyLayers;
    public float damage = 20f;

    private PlayerState m_state;
    private Animator m_animator;
    private int m_comboState = 0;
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
        var attack = Input.GetButtonDown("Fire1");

        if(attack && m_comboState <= 1){
            Attack();
        }
    }

    private void Attack(){
        SimpleAttack();
        /*m_curAttackDelay = attackDelay;
        if(groundSensor.State())
            if(m_state.state != PlayerState.State.DASH)
                SimpleAttack();
            else
                DashAttack();
        else 
            else 
        else 
            else 
        else 
            AirAttack();*/
    }

    private void SimpleAttack(){
        StopCoroutine(ReserCombo(attackDelay));
        m_comboState += 1;
        m_animator.SetTrigger("Attack");
        m_animator.SetInteger("ComboState", m_comboState);
        m_state.state = PlayerState.State.ATTACK;
        StartCoroutine(ReserCombo(attackDelay));
    }

    private void AirAttack(){
        m_animator.SetTrigger("AttackJump");
        m_state.state = PlayerState.State.ATTACK;
        StartCoroutine(Hit(0.25f));
    }

    private void DashAttack(){
        m_animator.SetTrigger("AttackDash");
        m_state.state = PlayerState.State.ATTACK;
        StartCoroutine(Hit(0.3f));
    }

    private IEnumerator Hit(float timer){
        yield return new WaitForSeconds(timer);
        if(m_state.state != PlayerState.State.ATTACK)
            yield break;
        m_state.state = PlayerState.State.DEFAULT;
        //Debug.Log("Hit");
    }

    private IEnumerator ReserCombo(float timer){
        yield return new WaitForSeconds(timer);
        m_comboState = 0;
        if(m_state.state != PlayerState.State.ATTACK)
            yield break;
        m_state.state = PlayerState.State.DEFAULT;
    }
}
