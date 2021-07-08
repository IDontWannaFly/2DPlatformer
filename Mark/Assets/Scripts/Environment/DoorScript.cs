using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{

    private Animator m_animator;
    // Start is called before the first frame update
    void Start()
    {
        m_animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator SetBool(bool value, float delayInSec){
        yield return new WaitForSeconds(delayInSec);
        m_animator.SetBool("IsOpen", true);
    }

    private void OnTriggerEnter2D(Collider2D other){
        if((LayerMask.GetMask("Player") & 1 << other.gameObject.layer) != 0){
            m_animator.SetTrigger("Open");
            StartCoroutine(SetBool(true, 0.2f));
        }
    }
}
