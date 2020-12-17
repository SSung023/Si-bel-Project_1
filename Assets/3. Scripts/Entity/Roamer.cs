using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Roamer : Entity
{
    public bool isInTheCamera;
    public enum RoamStyle {Linear, Recursive};
    public RoamStyle roamStyle = 0;
    [SerializeField] Transform[] roamingPoint = null;
    Animator animator;
    public float velocity = 0.5f;
    
    bool isRoaming = false;
    public int nextPointIndex = 0;
    public override void Start() {
        //StartCoroutine(Roam());
        animator = GetComponent<Animator>();
    }

    void Update() {
        animator.SetFloat("MoveSpeed", 0);
        if(!isRoaming) {
            isRoaming = true;
            //print("roaming");
            if(nextPointIndex == roamingPoint.Length) {
                nextPointIndex = 0;
            }
            MoveTo(roamingPoint[nextPointIndex++].position);
        }
    }
    /*
    IEnumerator Roam() {
        while(true) {
            switch(roamStyle) {
                case RoamStyle.Linear:
                    for(int i = 0; i < roamingPoint.Length; i++) {
                        MoveTo(roamingPoint[i].position);
                    }
                    for(int i = roamingPoint.Length - 1; i >= 0 ; i--) {
                        MoveTo(roamingPoint[i].position);
                    }
                    break;
                case RoamStyle.Recursive:
                    foreach(Transform point in roamingPoint) {
                        MoveTo(point.position);
                    }
                    break;
            }
            
        }
    }
    */
    void MoveTo(Vector3 pos) {
        float dist = (transform.position - pos).sqrMagnitude;
        if(pos.x - transform.position.x < 0) {
            transform.localScale = new Vector3(-1.5f, 1.5f, 1);
        }
        else {
            transform.localScale = new Vector3(1.5f, 1.5f, 1);
        }
        transform.
            DOMove(pos, dist / velocity).
            SetEase(Ease.InOutCubic).
            OnComplete(()=> {isRoaming = false;});
    }
    public override void TakeHit(int amount)
    {
        print("내가 죽다니..");
        base.TakeHit(amount);
    }
    public void OnCollisionStay2D (Collision2D col) {
        if (col.gameObject.CompareTag("Player")) {
            Player player = col.gameObject.GetComponent<Player>();
            if(player != null) {
                player.TakeHit(1);
            }
        }
    }
}
