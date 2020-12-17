using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Experimental.Rendering.Universal;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class EndingManager : MonoBehaviour
{
    [SerializeField] private float delayTime;
    [Header ("Piece Control")]
    [SerializeField] private GameObject piece;
    [SerializeField] Transform pieceTargetT;
    [SerializeField] private float pieceFallingTime;
    [Header ("Fade")]
    [SerializeField] private Light2D bgLight;
    [SerializeField] private float bgLightOnTime;
    [SerializeField] private float bgLightOffTime;
    [SerializeField] private Image bgPanel;
    [Header ("Text Control")]
    [SerializeField] private GameObject textContent;
    bool isScriptingEnd;
    void Start()
    {
        bgPanel.color = Color.white;
        isScriptingEnd = false;
        DOTween.ToAlpha(() => bgPanel.color, x=> bgPanel.color = x, 0, 1).OnComplete(() => {bgPanel.color = new Color(0, 0, 0, 0);});
        ActPiece();
    }
    void Update()
    {
        if(isScriptingEnd && Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
    void ActPiece()
    {
        Sequence pieceSeq = DOTween.Sequence()
        .SetDelay(delayTime)
        .Append(piece.transform.DOMoveY(pieceTargetT.position.y, pieceFallingTime))
        .Append(DOTween.To(() => bgLight.intensity, x=> bgLight.intensity = x, 10, bgLightOnTime))
        .Append(DOTween.ToAlpha(() => bgPanel.color, x=> bgPanel.color = x, 1, bgLightOffTime))
        .OnComplete(() => StartCoroutine(ActTexts()));
    }
    
    IEnumerator ActTexts()
    {
        Text title = textContent.transform.GetChild(0).GetComponent<Text>();
        DOTween.ToAlpha(() => title.color, x=> title.color = x, 1, 2f);
        SoundManager.aud.PlaySnd(0, 7); // 브금틀기
        yield return new WaitForSeconds(2f);
        print("맨 위 시작");
        SoundManager.aud.PlaySnd(1, 24); // 타자기 소리넣기
        foreach (Text t in textContent.transform.GetChild(1).GetComponentsInChildren<Text>())
        {
            DOTween.ToAlpha(() => t.color, x=> t.color = x, 1, 0.5f);
            yield return new WaitForSeconds(0.5f);
        }
        SoundManager.aud.StopSnd(1); // 타자기 중단
        print("맨 위 끝");
        yield return new WaitForSeconds(2.5f);
        print("중간 시작");
        SoundManager.aud.PlaySnd(1, 24); // 타자기 소리넣기
        foreach (Text t in textContent.transform.GetChild(2).GetComponentsInChildren<Text>())
        {
            if(t.transform.childCount == 1)
            {
                Text nameTx = t.transform.GetChild(0).GetComponent<Text>();
                SoundManager.aud.StopSnd(1); // 타자기 소리 중단
                Sequence seq = DOTween.Sequence()
                .Append(DOTween.ToAlpha(() => t.color, x => t.color = x, 1, 0.5f))
                .Join(nameTx.DOText("???????????", 3600f, true, ScrambleMode.Custom, "!@#$%^&*()_+=-;:,./?"));
                SoundManager.aud.PlaySnd(1, 26); // 취이익 소리 내리
            }
            else
            {
                DOTween.ToAlpha(() => t.color, x=> t.color = x, 1, 0.5f);
            }
            yield return new WaitForSeconds(1.5f);
        }
        print("중간 끝");
        yield return new WaitForSeconds(2.5f);
        print("끝 시작");
        for(int i = 0; i < 2; i++)
        {
            Transform root = textContent.transform.GetChild(3).GetChild(i);
            print("끝 페이즈 시작");
            foreach(Text t in root.transform.GetComponentsInChildren<Text>())
            {
                DOTween.ToAlpha(() => t.color, x=> t.color = x, 1, 0.5f);
                yield return new WaitForSeconds(0.5f);
            }
            print("끝 페이즈 끝");
            yield return new WaitForSeconds(1.5f);
        }
        print("끝 끝");
        isScriptingEnd = true;
    }
    
}
