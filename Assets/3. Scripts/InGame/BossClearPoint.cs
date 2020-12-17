using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.SceneManagement;

public class BossClearPoint : MonoBehaviour
{
    Light2D[] lights = new Light2D[3];
    Light2D lastLight;
    void Start()
    {
        for(int i = 0; i < lights.Length; i++)
        {
            lights[i] = transform.GetChild(0).GetChild(i).GetComponent<Light2D>();
        }
        lastLight = transform.GetChild(0).GetChild(lights.Length).GetComponent<Light2D>();
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.CompareTag("Player"))
        {
            Player player = col.GetComponent<Player>();
            player.canAttack = true;
            StartCoroutine(AdjustLight());
        }
    }
    IEnumerator AdjustLight()
    {
        FindObjectOfType<StageManager>().OnClearBossStage();
        short CrystalSoundPitch = 0;
        foreach(Light2D light in lights)
        {
            light.intensity = 3;
            SoundManager.aud.PlaySnd(1, CrystalSoundPitch++ + 20); // 크리스탈 사운드 재생하는 명령어
            yield return new WaitForSeconds(1f);
        }
        SoundManager.aud.PlaySnd(1,25); // 홀리홀리한 아아아 소리 재생하는 명령어
        DOTween.To(()=>lastLight.pointLightOuterRadius, x=> lastLight.pointLightOuterRadius = x, 30, 7f)
        .OnComplete(() => SceneManager.LoadScene("Ending"));
        
    }
}
