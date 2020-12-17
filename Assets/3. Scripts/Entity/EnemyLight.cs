using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLight : MonoBehaviour
{
    public int damage;
    public void OnTriggerStay2D (Collider2D col) {
        if (col.gameObject.CompareTag("Player")) {
            Player player = col.GetComponent<Player>();
            if(player != null) {
                if(player.isLightOn) {
                    print("방어했다!");
                    return;
                }
                player.TakeHit(damage);
            }
        }
    }
}
