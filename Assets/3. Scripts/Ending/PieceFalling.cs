using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class PieceFalling : MonoBehaviour
{
    [SerializeField] private GameObject BGLight;
    private GameObject piece;
    private Transform target;
    private Transform pieceTransform;
    private Light2D light2D;
    
    
    private void Awake()
    {
        piece = transform.gameObject;
        pieceTransform = piece.GetComponent<Transform>();
        light2D = BGLight.GetComponent<Light2D>();
        target = piece.transform.GetChild(0);

        FallingProcess();
        
    }

    private void Update()
    {
        
    }
    
    private void FallingProcess()
    {
        Sequence seq = DOTween.Sequence()
            .Append(pieceTransform.DOLocalMove(target.position, 7f, false)).SetEase(Ease.OutQuint);
            //.Join()
    }

    IEnumerator Wait()
    {
        float i = 1f;
        while (i <= 10)
        {
            light2D.intensity += 0.25f;
            i++;
            yield return new WaitForSeconds(0.2f);
        }
    }


}
