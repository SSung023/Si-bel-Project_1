using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaknessSpot : MonoBehaviour
{
    public Roamer parent;

    public void OnTriggerEnter2D (Collider2D col) 
    {
        if (col.gameObject.CompareTag("Player")) 
        {
            if(col.GetComponent<Player>().canAttack) 
            {
                parent.TakeHit(-1);
            }
        }
    }
}
