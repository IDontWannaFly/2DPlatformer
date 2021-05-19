using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerScript : MonoBehaviour
{
    private PlayerMovementScript m_movement;
    private PlayerCombatScript m_combat;
    private PlayerState m_state = new PlayerState();

    private void Start() {
        m_movement = GetComponent<PlayerMovementScript>();
        m_combat = GetComponent<PlayerCombatScript>();
        m_movement.BindState(m_state);
        m_combat.BindState(m_state);
    }

    private void Update() {
        
    }

    public void Hurt(float damage){
        
    }
}
