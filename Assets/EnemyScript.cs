using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public float health = 100f;

    private float m_curHealth;
    // Start is called before the first frame update
    void Start()
    {
        m_curHealth = health;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Hurt(float damage){
        m_curHealth -= damage;
        if(m_curHealth <= 0)
            Die();
    }

    private void Die(){
        
    }
}
