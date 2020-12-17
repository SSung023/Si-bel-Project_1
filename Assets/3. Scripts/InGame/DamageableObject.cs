using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageableObject : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    public void OnCollisionEnter2D(Collision2D col)
    {
        GameObject colObj = col.gameObject;
        if(colObj.CompareTag("Player"))
        {
            colObj.GetComponent<Player>().TakeHit(damage);
        }
    }
}
