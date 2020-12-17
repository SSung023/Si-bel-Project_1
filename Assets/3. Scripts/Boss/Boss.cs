#pragma warning disable CS0649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
public class Boss : MonoBehaviour
{
    Animator animator;
    public int maxHealth = 22;
    public int curHealth = 22;
    [SerializeField] float breakTime = 5;
    [SerializeField] GameObject[] weaknessSpots = null;
    
    [SerializeField] Skill[] skills_1 = new Skill[2];
    [SerializeField] Skill[] skills_2 = new Skill[2];
    [SerializeField] Skill[] skills_3 = new Skill[2];
    [SerializeField] Skill[] passiveSkills = new Skill[0];
    [SerializeField] GameObject clearPoint;
    [SerializeField] Transform rewardPointT;
    [SerializeField] Transform[] edgeSpots = new Transform[2];
    [SerializeField] Transform deathPosition;
    public event System.Action OnBossDeath;

    private AudioSource bossAud; // 보스 게임오브젝트 오디오소스 선언
    
    void Start()
    {
        animator = GetComponent<Animator>();
        bossAud = GetComponent<AudioSource>(); // 보스 게임오브젝트 오디오소스 할당
        curHealth = maxHealth;
        StartCoroutine(SkillLoop());
        StartCoroutine(PassiveSkillLoop());
    }

    IEnumerator SkillLoop()
    {
        while (curHealth > 16)
        {
            foreach(Skill skill in skills_1)
            {
                if(!skill.isActivated)
                {
                    print("액티브 스킬: " + skill.skillName);
                    skill.Activate();
                    yield return new WaitForSeconds(skill.skillDuration + breakTime);
                }
            }
            yield return null;
        }
        bossAud.PlayOneShot(SoundManager.aud.TakeSndClip(10)); // 2패턴 때 howling 사운드 재생
        while (curHealth > 10)
        {
            foreach(Skill skill in skills_2)
            {
                if(!skill.isActivated)
                {
                    print("액티브 스킬: " + skill.skillName);

                    skill.Activate();
                    yield return new WaitForSeconds(skill.skillDuration + breakTime);
                }
            }
            yield return null;
        }
        bossAud.PlayOneShot(SoundManager.aud.TakeSndClip(11)); // 3패턴 때 yell 사운드 재생
        while (curHealth > 0)
        {
            foreach(Skill skill in skills_3)
            {
                print("asd");
                if(!skill.isActivated)
                {
                    print("액티브 스킬: " + skill.skillName);

                    skill.Activate();
                    yield return new WaitForSeconds(skill.skillDuration + breakTime);
                }
            }
            yield return null;
        }
    }
    IEnumerator PassiveSkillLoop() 
    {
        
        while(curHealth != 0)
        {
            foreach(Skill skill in passiveSkills)
            {
                if(!skill.isActivated)
                {
                    print("패시브 스킬: " + skill.skillName);
                    skill.Activate();
                }
            }
            yield return null;
        }
    }

    IEnumerator LookAt()
    {
        Player player = StageManager.instance.player;
        while(player != null)
        {
            Vector3 dir = player.transform.position - transform.position;
            float angle = Mathf.Atan2(dir.y,dir.x) * Mathf.Rad2Deg - 180;
            transform.DORotate(Quaternion.AngleAxis(angle, Vector3.forward).eulerAngles, 15).SetSpeedBased().SetEase(Ease.Linear);
            yield return new WaitForSeconds(2f);
        }
    }

    public void MoveTo(Transform destination, float duration)
    {
        Vector3 originPos = transform.position;
        Vector3 originRot = transform.rotation.eulerAngles;
        Sequence moveSeq = DOTween.Sequence()
        .Append(transform.DOMove(destination.position, duration))
        .Join(transform.DORotate(destination.rotation.eulerAngles, duration))
        .AppendInterval(1f)
        .Append(transform.DOMove(originPos, duration))
        .Join(transform.DORotate(originRot, duration));
    }
    public void MoveTo(int destination, float duration)
    {
        MoveTo(edgeSpots[destination], duration);
    }
    public void TakeHit(int amount)
    {
        curHealth -= amount;
        if(curHealth < 0)
        {
            curHealth = 0;
        }
        if(curHealth == 0)
        {
            Die();
        }
    }
    void Die()
    {
        print("Boss Died");
        if(OnBossDeath != null)
        {
            OnBossDeath.Invoke();
        }
        float dropTime = 1f;
        Sequence seq = DOTween.Sequence()
        .OnStart(() => bossAud.PlayOneShot(SoundManager.aud.TakeSndClip(12))) // 보스 사망 비명 지름
        .AppendInterval(1.0f)
        .AppendCallback(() => 
        {
            foreach(GameObject weaknessSpot in weaknessSpots)
            {
                weaknessSpot.SetActive(false);
            }
            foreach(Transform child in transform.GetComponentsInChildren<Transform>())
            {
                if(child == this.transform) {
                    continue;
                }
                child.gameObject.SetActive(false);
            }
            
            animator.SetTrigger("OnDeath");
        })
        .Append(transform.DOMoveY(deathPosition.position.y, dropTime).SetEase(Ease.InOutQuart))
        .Insert(1 + dropTime * 0.5f, transform.DORotate(deathPosition.rotation.eulerAngles, dropTime * 0.4f).SetEase(Ease.Linear))
        .InsertCallback(1 + dropTime * 0.5f, () => bossAud.PlayOneShot(SoundManager.aud.TakeSndClip(14))) // 보스 사망 후 낙하 시 뼈 부서지는 소리 (왜 두번나지..)
        .OnComplete(() => 
        {
            GameObject reward = Instantiate(clearPoint, rewardPointT.position ,Quaternion.Euler(Vector3.zero));
            SoundManager.aud.StopSnd(0); // 보스 사망 시퀀스 종료 후 브금 꺼짐
        });
    }
}
