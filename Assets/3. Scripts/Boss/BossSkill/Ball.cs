using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Ball : MonoBehaviour
{
    [HideInInspector] public float speed = 0;
    [HideInInspector] public int damage = 0;
    void Start()
    {
        Destroy(this.gameObject, 10f);
    }

    void Update() 
    {
        transform.Translate(Vector3.left * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.CompareTag("Player")) {
            print("충돌!");
            col.GetComponent<Player>().TakeHit(damage);
            Destroy(this.gameObject, 0.1f);
        }
    }
}
