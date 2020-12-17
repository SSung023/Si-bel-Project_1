using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
    public Player player = null;
    public CameraController cam = null;
    [SerializeField] Light2D bgLight = null;
    [SerializeField] Transform respawnPointT = null;
    public Transform cameraPointT = null;
    [SerializeField] GameObject gameOverContent = null; 
    int healthPoint;
    int fuelPoint;
    public static StageManager instance;
    void Awake() {
        #region 싱글톤 구현부분
        if(instance == null) {
            instance = this;
        }
        else {
            Destroy(this.gameObject);
        }
        #endregion
    }
    void Start()
    {
        //bgLight.intensity = 1;
        //DOTween.To(()=> bgLight.intensity, x=> bgLight.intensity = x, 0, 2.5f);

        player.OnPlayerHit += this.OnPlayerHit;
        player.OnPlayerLight += this.OnPlayerLight;
        player.OnPlayerDeath += this.OnPlayerDeath;
        healthPoint = 0;
        fuelPoint = 0;

        for(int i = 0; i < player.maxHealth; i++) 
        {
            transform.GetChild(2).GetChild(i).gameObject.SetActive(player.curHealth > i);
        }
        for(int i = 0; i < player.maxFuel; i++) 
        {
            transform.GetChild(3).GetChild(i).gameObject.SetActive(player.curFuel > i);
        }
    }
    // x 버튼을 누르면 world map으로 이동
    public void BackToWorldMap()
    {
        SceneManager.LoadScene("World Map");
    }
    public void OnPlayerHit(int amount) 
    {
        for(int i = healthPoint; i < amount + healthPoint; i++)
        {
            StartCoroutine(LeaveHealthPoint(i));
        }
        healthPoint += amount;
        if(player.curHealth > 1 && amount != 0) 
        {
            StartCoroutine(RespawnPlayer());
        }
    }
    public void OnPlayerLight() 
    {
        StartCoroutine(LeaveFuelPoint(fuelPoint++));
    }
    public void OnPlayerDeath() 
    {
        Text gameOverTx = gameOverContent.transform.GetChild(0).GetComponent<Text>();
        Sequence seq = DOTween.Sequence()
            .OnStart(() => 
            {
                gameOverTx.color = new Color(1,0,0,0);
                gameOverContent.SetActive(true);
            })
            .Append(DOTween.To(()=> bgLight.intensity, x=> bgLight.intensity = x, 0, 2.5f))
            .Append(DOTween.ToAlpha(()=>gameOverTx.color, x=>gameOverTx.color = x, 1, 1.0f))
            .OnComplete(()=>{Invoke("ReLoad", 2.0f);});
    }
    public void ReLoad()
    {
        Debug.Log("리셋 true");
        Blackboard.isReset = true;
        SceneManager.LoadScene("MainMenu");
    }
    IEnumerator RespawnPlayer() 
    {
        player.enabled = false;
        yield return new WaitForSeconds(player.dieTime);
        player.gameObject.SetActive(false);
        yield return new WaitForSeconds(1f);
        player.transform.position = respawnPointT.position;
        player.enabled = true;
        player.gameObject.SetActive(true);
        SoundManager.aud.PlaySnd(1, 4); // 부활할때 불 지펴지는 소리 재생
    }
    IEnumerator LeaveHealthPoint(int index) 
    {
        transform.GetChild(2).GetChild(index).GetComponent<Animator>().SetTrigger("onLeave");
        yield return new WaitForSeconds(0.5f);
        transform.GetChild(2).GetChild(index).gameObject.SetActive(false);
    }  
    IEnumerator LeaveFuelPoint(int index) 
    {
        transform.GetChild(3).GetChild(index).GetComponent<Animator>().SetTrigger("onLeave");
        yield return new WaitForSeconds(0.2f);
        transform.GetChild(3).GetChild(index).gameObject.SetActive(false);
    }
    public void UpdateRespawnPoint(Transform nextSpawnPoint)
    {
        respawnPointT = nextSpawnPoint;
    }
    public void UpdateCameraPoint(Transform nextCameraPoint)
    {
        cameraPointT = nextCameraPoint;
        if(cam.cameraMode == CameraController.CameraMode.Fixed)
        {
            cam.transform.DOMove(cameraPointT.position, 2f).SetEase(Ease.OutCubic);
        }
    }
    public void OnClearBossStage()
    {
        this.transform.GetChild(1).gameObject.SetActive(false);
        this.transform.GetChild(2).gameObject.SetActive(false);
        this.transform.GetChild(3).gameObject.SetActive(false);
    }
}
