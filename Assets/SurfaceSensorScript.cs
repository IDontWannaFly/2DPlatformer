using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceSensorScript : MonoBehaviour
{
    private int m_colCount = 0;
    private float m_delay = 0f;
    
    private void Update() {
        if(m_delay > 0)
            m_delay -= Time.deltaTime;
    }

    public bool State(){
        return m_colCount > 0 && m_delay <= 0;
    }

    public void SetDelay(float value){
        m_delay = value;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        m_colCount += 1;
    }

    private void OnTriggerExit2D(Collider2D other) {
        m_colCount -= 1;
    }
}
