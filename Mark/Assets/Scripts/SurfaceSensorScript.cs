using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SurfaceSensorScript : MonoBehaviour
{
    [Serializable]
    private class StateEvent : UnityEvent<Collider2D> {}

    public LayerMask surfaceLayers;

    private int m_colCount = 0;
    private float m_delay = 0f;
    private StateEvent m_enterEvent = new StateEvent();
    private StateEvent m_exitEvent = new StateEvent();
    
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

    public void AttachListener(UnityAction<Collider2D> action, bool isEnterListener){
        if(isEnterListener)
            m_enterEvent.AddListener(action);
        else
            m_exitEvent.AddListener(action);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if((surfaceLayers.value & 1 << other.gameObject.layer) != 0){
            m_colCount += 1;
            m_enterEvent.Invoke(other);
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if((surfaceLayers.value & 1 << other.gameObject.layer) != 0){
            m_colCount -= 1;
            m_exitEvent.Invoke(other);
        }
    }
}
