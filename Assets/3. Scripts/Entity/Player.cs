#pragma warning disable CS0649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using DG.Tweening;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.SceneManagement;

public class Player : Entity
{
    [Header ("Light")]
    [SerializeField] float visionDuration = 0;
    [SerializeField] float visionRadius = 0;
    Animator animator;
    Light2D torch;
    Rigidbody2D myRigid;
    AudioSource playerAud;
    public int speed;
    
    float nextHitTime;
    public float hitTerm;
    [HideInInspector] public bool isLightOn;
    [HideInInspector] public bool canAttack;
    public float maxFuel = 5;
    [HideInInspector] public float curFuel = 5;
    float fuelPerUse = 1;
    public int currentLevelIndex;
    int jumpCount;
    public event System.Action<int> OnPlayerHit;
    public event System.Action OnPlayerDeath;
    public event System.Action OnLand;
    public event System.Action OnPlayerLight;
    
    public Ease lightEase;
    public Color dieColor;
    
    private void Awake()
    {
        animator = GetComponent<Animator>();
        torch = transform.GetChild(0).GetComponent<Light2D>();
        myRigid = GetComponent<Rigidbody2D>();
        playerAud = GetComponent<AudioSource>();
        jumpCount = 0;
        isLightOn = false;
        canAttack = true;
        curFuel = maxFuel;
        hitTerm = dieTime + 0.1f;
    }

    public override void Start()
    {
        DOTween.To(()=> torch.pointLightOuterRadius, x=> torch.pointLightOuterRadius = x, 0, 0.5f);
        base.Start();
    }
    public void OnEnable() 
    {
        torch.color = Color.white;
        torch.pointLightInnerRadius = 0;
        torch.pointLightOuterRadius = 0;
        canAttack = true;
    }
    public void OnDisable() 
    {
        isLightOn = false;
    }
    private void Update()
    {
        if(canAttack) 
        {
            Move();
            CheckInteraction();
        }
    }
    private void Move()
    {
        if(!Blackboard.canMove)
            return;
        
        if(Input.GetKeyDown(KeyCode.Space) && jumpCount < 2) 
        {
            jumpCount++;

            // 점프 사운드 재생부
            if (SoundManager.aud != null) // 사운드매니저가 있으면 점프할때 woosh(playerjump) 사운드 재생
            {
                var defaultPitch = playerAud.pitch;
                var randomPitch = Random.Range(-3, 4); // 랜덤한 피치(음높이)로 재생
                playerAud.pitch = randomPitch;
                //print(playerAud.pitch);
                playerAud.PlayOneShot(SoundManager.aud.TakeSndClip(5));
                playerAud.pitch = defaultPitch; //다시 원래 피치로 복구
            }

            Jump();
        }
        animator.SetFloat("MoveSpeed", Input.GetAxis("Horizontal"));
        Vector3 horizontal = new Vector3(Input.GetAxis("Horizontal"), 0.0f, 0.0f);
        transform.position = transform.position + horizontal * speed * Time.deltaTime;
    }

    void Jump()
    {
        animator.SetBool("isJump", true);
        myRigid.velocity = new Vector2(myRigid.velocity.x, 0);
        myRigid.AddForce(new Vector2(0, 300));
    }

    void CheckInteraction() 
    {
        if(Input.GetKeyDown(KeyCode.Z) && !isLightOn && fuelPerUse <= curFuel) 
        {
            TurnOnLight();
        }
    }
    void TurnOnLight() {    
        if(OnPlayerLight != null) {
            OnPlayerLight.Invoke();
        }
        Sequence torchSeq = DOTween.Sequence()
        .OnStart(() => 
        {
            playerAud.PlayOneShot(SoundManager.aud.TakeSndClip(0)); // 횃불 스킬 쓸때 횃불 사운드 재생
            curFuel -= fuelPerUse;
            isLightOn = true;
            torch.pointLightOuterRadius = visionRadius;
            torch.pointLightInnerRadius = visionRadius - 0.3f;
        })
        .AppendInterval(visionDuration)
        .Append(DOTween.To(() => torch.pointLightInnerRadius, x => torch.pointLightInnerRadius = x, 0, 0.6f).SetEase(lightEase))
        .Insert(visionDuration + 0.2f, DOTween.To(() => torch.pointLightOuterRadius, x => torch.pointLightOuterRadius = x, 0, 0.4f).SetEase(lightEase))
        .OnComplete(() => {isLightOn = false;});
    }
    public override void TakeHit(int amount)
    {
        if(nextHitTime > Time.time)
        {
            return;
        }
        nextHitTime = Time.time + hitTerm;
        if(amount == -1) 
        {
            amount = curHealth;
        }
        if(OnPlayerHit != null) 
        {
            OnPlayerHit.Invoke((amount > curHealth) ? curHealth : amount);
        }
        print("공격받았다! 현재 체력 비율: " + (curHealth - amount).ToString() + " / " + maxHealth);
        if(SoundManager.aud != null) // 죽었을때 sizzle 사운드 재생
        {
            playerAud.PlayOneShot(SoundManager.aud.TakeSndClip(Random.Range(1, 3)));
        }
        canAttack = false;
        animator.SetTrigger("onDied");
        base.TakeHit(amount);
    }
    public override void Die()
    {
        if (SoundManager.aud != null)
        {
            StartCoroutine(PlayDieSound());
        }
        StartCoroutine(DieProcess());

        Sequence dieSeq = DOTween.Sequence()
        .OnStart(() => {torch.color = dieColor;})
        .Append(DOTween.To(()=> torch.pointLightOuterRadius, x=> torch.pointLightOuterRadius = x, 80, 5f))
        .Join(DOTween.To(()=>torch.intensity, x=>torch.intensity=x, 0, 3f));
    }

    // 사망 효과음 재생부
    IEnumerator PlayDieSound()
    {
        yield return new WaitForFixedUpdate();
        SoundManager.aud.StopSnd(0);
        SoundManager.aud.PlaySnd(1, 3);
    }

    IEnumerator DieProcess() {
        yield return new WaitForSeconds(0.01f);
        torch.transform.SetParent(null);
        if(OnPlayerDeath != null)  OnPlayerDeath.Invoke();
        base.Die();
    }

    IEnumerator ClearProcess(Collider2D collision)
    {
        SoundManager.aud.PlaySnd(1, 19); // 클리어 포인트 도달 시 완료 소리 재생 
        Blackboard.W1StageClearInfo[currentLevelIndex] = true;
        collision.GetComponent<Collider2D>().enabled = false;
        collision.GetComponent<Animator>().SetTrigger("clear");
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("World Map");
    }
    
    // check point object와 충돌 시 clear 체크
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 클리어 부분 (되도록 StageManager에서 관리하도록 하자)
        if (collision.CompareTag("Clear Point"))
        {
            StartCoroutine(ClearProcess(collision));
        }

        // 공격 부분
        if (collision.CompareTag("WeaknessSpot"))
        {
            // 타격 사운드 재생
            if (SoundManager.aud != null)
            {
                playerAud.PlayOneShot(SoundManager.aud.TakeSndClip(Random.Range(6, 9)));
            }
            Jump();
        }
    }
    
    // 점프 초기화 부분
    private void OnCollisionEnter2D(Collision2D col) {
        if(col.gameObject.CompareTag("Ground")) {
            animator.SetBool("isJump", false);
            if(jumpCount != 0) {
                jumpCount = 0;
                if(OnLand != null) {
                    OnLand.Invoke();
                }
            }
        }
    }
}
