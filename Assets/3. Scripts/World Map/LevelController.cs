#pragma warning disable CS0649
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//
public class LevelController : MonoBehaviour
{
    [SerializeField] private GameObject girl = null;
    [SerializeField] private GameObject selecter = null;
    [SerializeField] private GameObject EffectContent = null;
    [SerializeField] private Animator animator;

    private float dir;
    private Vector3 selecterPos;
    private Vector3 girlPos;
    
    
    private GameObject[] stageSpots; // child 오브젝트 적용 : 단위 1
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        stageSpots = new GameObject[7];
        for (int i = 0; i < 7; i++)
        {
            stageSpots[i] = transform.GetChild(i).gameObject;
        }
        
        EffectContent.transform.position = new Vector3(20,0);
    }

    private void Start()
    {
        if (Blackboard.isReset)
        {
            ResetStageInfo();
        }
        else
        {
            LoadStageInfo();
        }
        
        OnLoadEffect();
    }

    private void Update()
    {
        UpdateActivateSpot(); // key를 입력받아 처리 - selecter 이미지 업데이트
    }
    
    private void UpdateActivateSpot()
    {
        if (!Blackboard.commonVariable.isInputAcceptable || Blackboard.commonVariable.isMenuOff)
        {
            return;
        }
        // up: 0, down: 1, left: 2, right: 3
        SingleStageSpot spot = stageSpots[Blackboard.W1StageProgress].GetComponent<SingleStageSpot>();
        girlPos = new Vector3(spot.transform.position.x, spot.transform.position.y + 0.5f);

        if (Input.GetKeyDown(KeyCode.RightArrow) && spot.ableSpots[3] != null && spot.ableSpots[3].GetComponent<SingleStageSpot>().isUnlocked)
        {
             //spot.isActived = false;
             Blackboard.W1StageProgress--;
             dir = 3f;
            SoundManager.aud.PlaySnd(2, 2);
        }
         
        else if (Input.GetKeyDown(KeyCode.LeftArrow) && spot.ableSpots[2] != null && spot.ableSpots[2].GetComponent<SingleStageSpot>().isUnlocked)
        {
             //spot.isActived = false;
             Blackboard.W1StageProgress++;
             dir = 2f;
            SoundManager.aud.PlaySnd(2, 2);
        }
         
        else if (Input.GetKeyDown(KeyCode.UpArrow) && spot.ableSpots[0] != null && spot.ableSpots[0].GetComponent<SingleStageSpot>().isUnlocked)
        {
             //spot.isActived = false;
             Blackboard.W1StageProgress += 3;
             dir = 0f;
            SoundManager.aud.PlaySnd(2, 2);
        }
         
        else if (Input.GetKeyDown(KeyCode.DownArrow) && spot.ableSpots[1] != null && spot.ableSpots[1].GetComponent<SingleStageSpot>().isUnlocked)
        {
             //spot.isActived = false;
             Blackboard.W1StageProgress -= 3;
             dir = 1f;
            SoundManager.aud.PlaySnd(2, 2);
        }
         
        else if (Input.GetKeyDown(KeyCode.Space) && MenuController.isPossible)
        {
             Blackboard.WGirlPos = Blackboard.W1StageProgress;
             //scene change effect - change scene
             StartCoroutine(ChangeProcess());
        }
        
        spot = stageSpots[Blackboard.W1StageProgress].GetComponent<SingleStageSpot>(); //값 갱신
        //spot.isActived = true;
        
        //Move selecter's position
        SelecterMoveProcess();
  
    }
    private void LoadStageInfo() // 저장된 stage clear 정보를 가져와서 loading
    {
        // clear된 stage 세팅
        StartCoroutine(WaitLoad());
        for (int i = 0; i < Blackboard.W1StageClearInfo.Length; i++)
        {
            if (Blackboard.W1StageClearInfo[i]) //만약 clear라면 
            {
                stageSpots[i].GetComponent<SingleStageSpot>().isCleared = true;
                stageSpots[i].GetComponent<SingleStageSpot>().isUnlocked = true;
            }
        }
        // 소녀의 위치 세팅
        girlPos = new Vector3(stageSpots[Blackboard.WGirlPos].transform.position.x, stageSpots[Blackboard.WGirlPos].transform.position.y + 0.5f);
        girl.transform.position = girlPos;

        // selecter의 위치 세팅
        selecterPos = new Vector3(stageSpots[Blackboard.W1StageProgress].transform.position.x, stageSpots[Blackboard.W1StageProgress].transform.position.y + 1f);
        selecter.transform.position = selecterPos;
    }

    private void ResetStageInfo()
    {
        StartCoroutine(WaitLoad());

        for (int i = 0; i < Blackboard.W1StageClearInfo.Length; i++)
        {
            Blackboard.W1StageClearInfo[i] = false;
            stageSpots[i].GetComponent<SingleStageSpot>().isCleared = false;
            stageSpots[i].GetComponent<SingleStageSpot>().isUnlocked = false;
        }
        stageSpots[0].GetComponent<SingleStageSpot>().isUnlocked = true;
        
        girlPos = new Vector3(stageSpots[0].transform.position.x, stageSpots[0].transform.position.y + 0.5f);
        girl.transform.position = girlPos;
        Blackboard.WGirlPos = 0;
        
        selecterPos = new Vector3(stageSpots[0].transform.position.x, stageSpots[0].transform.position.y + 1f);
        selecter.transform.position = selecterPos;

        Blackboard.W1StageProgress = 0;
        Blackboard.isReset = false;
        Debug.Log("리셋 false");
    }

    private void OnPressStage()
    {
        Sequence seq = DOTween.Sequence()
            .PrependInterval(0.4f)
            .Append(EffectContent.transform.DOMoveX(25, 1.3f));
    }

    private void OnLoadEffect()
    {
        Sequence seq = DOTween.Sequence()
            .Append(EffectContent.transform.DOMoveX(0, 1.3f))
            .OnComplete(() =>
            {
                EffectContent.transform.position = new Vector3(0,0);
            });
    }

    private void SelecterMoveProcess()
    {
        // Move selecter's position
        selecterPos = new Vector3(stageSpots[Blackboard.W1StageProgress].transform.position.x, stageSpots[Blackboard.W1StageProgress].transform.position.y + 1f);

        if (Blackboard.WGirlPos == Blackboard.W1StageProgress)
        {
            selecter.GetComponent<SpriteRenderer>().DOFade(0, 0.7f);
        }
        else
        {
            selecter.GetComponent<SpriteRenderer>().DOFade(1, 0.7f);
        }

        selecter.transform.DOLocalMove(selecterPos, 0.7f, false);

    }

    private void girlMoveProcess()
    {
        SoundManager.aud.PlaySnd(2, 0);
        Sequence seq = DOTween.Sequence()
            .OnStart(() =>
            {
                SingleStageSpot spot = stageSpots[Blackboard.W1StageProgress].GetComponent<SingleStageSpot>();
                animator.SetFloat("MoveDir", dir);
                animator.SetBool("isWalking", true);
            })
            .Append(girl.transform.DOLocalMove(girlPos, 1.5f, false));
    }
    IEnumerator ChangeProcess()
    {
        Blackboard.commonVariable.isInputAcceptable = false;
        girlMoveProcess();
        yield return new WaitForSeconds(1.5f);
        OnPressStage();
        yield return new WaitForSeconds(1.4f);
        EnterStage(stageSpots[Blackboard.W1StageProgress].GetComponent<SingleStageSpot>());
        animator.SetBool("isWalking", false);
        Blackboard.commonVariable.isInputAcceptable = true;
    }
     
    IEnumerator WaitLoad()
    {
        yield return new WaitForSeconds(1.0f);
    }
    
    private void EnterStage(SingleStageSpot singleStageSpot)
    {
        string tmp = singleStageSpot.SceneToMove;
        SceneManager.LoadScene(tmp);
    }
    
    
    
}
