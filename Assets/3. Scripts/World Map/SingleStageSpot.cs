using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SingleStageSpot : MonoBehaviour
{
    [SerializeField] public bool isUnlocked; // 해금 시 해당 stage 접근 가능
    [SerializeField] private bool isBossStage; // boss stage인지의 여부, 해당 stage clear 시, 다음 world map 해금
    public bool isCleared; // 해당 stage 클리어 시, 재진입 불가능
    //public bool isActived; // 활성화된 stage인지의 여부
    
    private GameObject unlockImage;
    
    public GameObject[] lines;
    public GameObject[] ableSpots = new GameObject[4]; // 해당 stage spot에서 이동 가능한 오브젝트
    public string SceneToMove;
    
    
    
    private void Awake()
    {
        unlockImage = transform.GetChild(0).gameObject;
    }
    
    private void Update()
    {
        UpdateLevelStatus();
        UpdateStageImage();
        UpdateStageColor();
    }
    
    private void UpdateStageImage()
    {
        if (!isCleared) //해당 stage 미 clear 시
        {
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i].gameObject.SetActive(false);
            }
        }
        else if(isCleared) //해당 stage clear 시
        {
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i].gameObject.SetActive(true);
            }
        }
    }

    private void UpdateStageColor()
    {
        if (isUnlocked) //해금 O
        {
            unlockImage.SetActive(true);
        }
        else // 해금 X
        {
            unlockImage.SetActive(false);
        }

        // if (isActived)
        // {
        //     
        // }

    }
    
    private void UpdateLevelStatus()
    {
        int curLevel = int.Parse(gameObject.name);

        if (Blackboard.W1StageClearInfo[curLevel]) //해당 레벨이 isCleared = true 라면 
        {    
            // 인접한 stage spot을 unlock 처리
            for (int i = 0; i < ableSpots.Length; i++)
            {
                if(ableSpots[i] != null)
                    ableSpots[i].GetComponent<SingleStageSpot>().isUnlocked = true;
            }
            isUnlocked = true;
            isCleared = true;
        }
        
    }
    
}
