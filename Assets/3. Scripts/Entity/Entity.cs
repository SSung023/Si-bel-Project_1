using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public int maxHealth;
    [HideInInspector] public int curHealth;
    public float dieTime;

    public virtual void Start() {
        curHealth = maxHealth;
    }

    void Update() {
        
    }

    // -1: 원킬, 정수: 그만큼
    public virtual void TakeHit(int amount) {
        curHealth -= (amount == -1) ? curHealth : amount;
        if(curHealth <= 0) {
            Die();
        }
    }

    public virtual void Die() {
        Destroy(this.gameObject, dieTime);
    }
}
