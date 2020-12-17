﻿#pragma warning disable CS0649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Experimental.Rendering.Universal;

public class HandForVanishment : Skill
{
    [SerializeField] GameObject hand;
    [SerializeField] float rotationSpeed;
    [SerializeField] int damage;

    Light2D eyeLight;
    IEnumerator lookAt;

    public override void Start()
    {
        eyeLight = GetComponent<Light2D>();
        eyeLight.intensity = 0;
        lookAt = LookAt();
        hand.SetActive(false);
        base.Start();
    }
    protected override void Affect()
    {
        Light2D handLight = hand.GetComponent<Light2D>();
        Collider2D handCollider = hand.GetComponent<Collider2D>();
        Sequence seq = DOTween.Sequence()
        .OnStart(() => 
        {
            hand.SetActive(true);
            print("눈알 온");
            handLight.intensity = 0;
            handCollider.enabled = false;
        })
        .Append(DOTween.To(()=>eyeLight.intensity, x=> eyeLight.intensity=x, 30, 1.5f))
        .Join(DOTween.To(()=>handLight.intensity, x=> handLight.intensity=x, 30, 1.5f))
        .AppendCallback(() => 
        {
            handCollider.enabled = true;
            StartCoroutine(lookAt);
        })
        .AppendInterval(skillDuration)
        .AppendCallback(() => 
        {
            StopCoroutine(lookAt);
        })
        .Join(DOTween.To(()=>eyeLight.intensity, x=> eyeLight.intensity=x, 0, 1.5f))
        .Join(DOTween.To(()=>handLight.intensity, x=> handLight.intensity=x, 0, 1.5f))
        .InsertCallback(2 + skillDuration, () => {handCollider.enabled = false;} )
        .AppendCallback(() => 
        {
            hand.SetActive(false);
        })
        .OnComplete(() => 
        {
            print("눈알 오프");
            isActivated = false;
        });
    }

    IEnumerator LookAt()
    {
        Player player = StageManager.instance.player;
        while (player != null)
        {
            Vector3 dir = player.transform.position - transform.position;
            float angle = Mathf.Atan2(dir.y,dir.x) * Mathf.Rad2Deg - 180;
            transform.DORotate(Quaternion.AngleAxis(angle, Vector3.forward).eulerAngles, rotationSpeed).SetSpeedBased().SetEase(Ease.Linear);
            yield return new WaitForSeconds(2f);
        }
    }

    public override void OnTriggerEnter2D(Collider2D col)
    {
        if(col.CompareTag("Player")) {
            print("충돌!");
            if(col.GetComponent<Player>().isLightOn) {
                return;
            }
            col.GetComponent<Player>().TakeHit(damage);
        }
    }
}
