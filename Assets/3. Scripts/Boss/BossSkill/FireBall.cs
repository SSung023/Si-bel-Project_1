using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Experimental.Rendering.Universal;

public class FireBall : Skill
{
    [SerializeField] GameObject ball = null;
    [SerializeField] float ballSpeed = 0;
    [SerializeField] int ballDamage = 0;

    Light2D chinLight;
    public override void Start()
    {
        chinLight = GetComponent<Light2D>();
        skillAud = GetComponent<AudioSource>();
        chinLight.intensity = 0;
    }
    protected override void Affect()
    {
        // 파이어볼 발사 시퀀스 시작할때 충전 사운드 재생
        skillAud.PlayOneShot(SoundManager.aud.TakeSndClip(15));
        Sequence seq = DOTween.Sequence()
        .OnStart(() => {print("턱 온");})
        .AppendCallback(() => {myBoss.MoveTo(Random.Range(0, 2), 3f);})
        .AppendInterval(1.5f)
        .Append(DOTween.To(()=>chinLight.intensity, x=> chinLight.intensity=x, 8, 1.5f))
        .Join(transform.DOLocalMoveY(-1, 1).SetRelative().SetEase(Ease.InCubic))
        .AppendCallback(() => 
        {
            // 파이어볼 발사 시 발사 사운드 재생
            skillAud.PlayOneShot(SoundManager.aud.TakeSndClip(16));
            Ball _ball = Instantiate(ball, transform).GetComponent<Ball>();
            _ball.speed = ballSpeed;
            _ball.damage = ballDamage;
            _ball.transform.SetParent(null);
        })
        .AppendInterval(1.5f)
        .Append(transform.DOLocalMoveY(1, 1).SetRelative().SetEase(Ease.InCubic))
        .Join(DOTween.To(()=>chinLight.intensity, x=> chinLight.intensity=x, 0, 1.5f))
        .OnComplete(() => 
        {
            isActivated = false;
            print("턱 오프");
        });
        
    }
}
