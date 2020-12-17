using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Experimental.Rendering.Universal;

public class FearOfFinite : Skill
{
    [SerializeField] Light2D darknessPanel = null;
    [SerializeField] GameObject lightOnPlayer = null;
    [SerializeField] int damage = 0;
    [SerializeField] Light2D[] eyeLight = new Light2D[2];
    [SerializeField] float chargeTime = 0;
    [SerializeField] float coolTime = 0;

    [SerializeField] float limit = 0;
    IEnumerator attack = null;
    bool isHit = false;

    public override void Start()
    {
        skillAud = GetComponent<AudioSource>();
        eyeLight[0].intensity = 0;
        eyeLight[1].intensity = 0;
        darknessPanel.color = Color.white;
        attack = ExplodeOnPlayerPos();
        myBoss.OnBossDeath += this.OnBossDeath;
        isHit = false;
        isActivated = false;
    }
    protected override void Affect()
    {
        Sequence first = DOTween.Sequence()
        .OnStart(() => {
            StopCoroutine(attack);
            attack = ExplodeOnPlayerPos();
        })
        .Append(DOTween.To(()=>darknessPanel.intensity, x=>darknessPanel.intensity = x, 0, 5f))
        .Join(DOTween.To(()=>eyeLight[0].intensity, x=>eyeLight[0].intensity=x, 30, 5f))
        .Join(DOTween.To(()=>eyeLight[1].intensity, x=>eyeLight[1].intensity=x, 30, 5f))
        .OnComplete(() => StartCoroutine(attack));
    }

    IEnumerator ExplodeOnPlayerPos()
    {
        Light2D targetLight = lightOnPlayer.GetComponent<Light2D>();
        for(int i =0; i < 5; i++)
        {
            while (isHit) 
            {
                isHit = false;
                yield return new WaitForSeconds(3f);
            }
            Sequence mySeq = DOTween.Sequence()
            .AppendCallback(() =>
            {
                // 폭발 시퀀스 시작할때 충전 사운드 재생
                skillAud.PlayOneShot(SoundManager.aud.TakeSndClip(17));
                lightOnPlayer.transform.position = StageManager.instance.player.transform.position;
            })
            .Append(DOTween.To(()=>targetLight.intensity, x=>targetLight.intensity = x, 3, chargeTime))
            .Join(DOTween.To(()=>eyeLight[0].intensity, x=>eyeLight[0].intensity=x, 80, chargeTime))
            .Join(DOTween.To(()=>eyeLight[1].intensity, x=>eyeLight[1].intensity=x, 80, chargeTime))
            .AppendCallback(() => 
            {
                StageManager.instance.cam.VibrateForTime(0.1f, 0.15f);
                // 폭발 할때 폭발 사운드 재생
                SoundManager.aud.PlaySnd(1, 13);
                AttackPlayer();
            })
            .Append(DOTween.To(()=>targetLight.intensity, x=>targetLight.intensity = x, 0, coolTime))
            .Join(DOTween.To(()=>eyeLight[0].intensity, x=>eyeLight[0].intensity=x, 30, coolTime))
            .Join(DOTween.To(()=>eyeLight[1].intensity, x=>eyeLight[1].intensity=x, 30, coolTime));
            yield return new WaitForSeconds(chargeTime + coolTime);
            if(myBoss.curHealth == 0)
            {
                break;
            }
        }
        int intensity = 30;
        if(myBoss.curHealth == 0)
        {
            intensity = 0;
        }
        Sequence last = DOTween.Sequence()
        .Append(DOTween.To(()=>darknessPanel.intensity, x=>darknessPanel.intensity = x, 1, 1f))
        .Join(DOTween.To(()=>eyeLight[0].intensity, x=>eyeLight[0].intensity=x, intensity, 1f))
        .Join(DOTween.To(()=>eyeLight[1].intensity, x=>eyeLight[1].intensity=x, intensity, 1f))
        .AppendCallback(() => {isActivated = false;});
    }

    public void OnBossDeath()
    {
        Sequence last = DOTween.Sequence()
        .OnStart(() => StopCoroutine(attack))
        .Append(DOTween.To(()=>darknessPanel.intensity, x=>darknessPanel.intensity = x, 1, 1f))
        .Join(DOTween.To(()=>eyeLight[0].intensity, x=>eyeLight[0].intensity=x, 0, 1f))
        .Join(DOTween.To(()=>eyeLight[1].intensity, x=>eyeLight[1].intensity=x, 0, 1f))
        .AppendCallback(() => {isActivated = false;});
    }

    void AttackPlayer()
    {
        Vector2 distVec = (lightOnPlayer.transform.position - StageManager.instance.player.transform.position);
        float dist = Mathf.Sqrt(Mathf.Pow(distVec.x, 2) + Mathf.Pow(distVec.y, 2));
        print("Distance : " + dist);
        if(dist < limit)
        {
            if(!StageManager.instance.player.isLightOn && isActivated) {
                StageManager.instance.player.TakeHit(damage);
                isHit = true;
            }
        }
    }
}
