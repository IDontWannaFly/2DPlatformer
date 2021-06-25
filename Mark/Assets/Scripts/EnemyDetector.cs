using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemyDetector : MonoBehaviour
{
    public LayerMask enemyLayer;
    public Transform player;

    private EnemyDetectorCallback callback;

    public void AttachCallback(EnemyDetectorCallback callback){
        this.callback = callback;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        var hit = Physics2D.Raycast(
            transform.position, 
            new Vector3(
                other.transform.position.x,
                other.transform.position.y + other.transform.lossyScale.y / 2
            ) - transform.position,
            Vector2.Distance(transform.position, other.transform.position),
            enemyLayer | LayerMask.GetMask("Ground")
        );
        if(hit.collider != null && (enemyLayer.value & 1 << hit.collider.gameObject.layer) != 0)
            if(callback != null)
                callback.OnDetected(other);
    }

    private void OnTriggerExit2D(Collider2D other) {
        if((enemyLayer.value & 1<<other.gameObject.layer) != 0){
            if(callback != null)
                callback.OnLost(other);
        }
    }

    private void OnDrawGizmosSelected() {
        if(player != null)
            Debug.DrawRay(transform.position, new Vector3(
                player.position.x, 
                player.position.y + player.lossyScale.y / 2) - transform.position, Color.green);
    }

    public interface EnemyDetectorCallback{
        void OnDetected(Collider2D collider);
        void OnLost(Collider2D collider);
    }
}
