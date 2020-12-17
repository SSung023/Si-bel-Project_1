#pragma warning disable CS0649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraController : MonoBehaviour
{
    public enum CameraMode {Fixed, HoldOn};
    public CameraMode cameraMode = CameraMode.HoldOn;
    [SerializeField] Player player = null;
    [SerializeField] float cameraShakeAmount = 0;
    [SerializeField] Transform bgContentRoot = null;
    [SerializeField] float backgroundSpeed;
    [SerializeField] float backgroundLayerMag;
    float[] bgContentMags;
    float shakeTime;
    Vector3 initialPosition;
    float boundaryLimit;
    void Start() {
        bgContentMags = new float[bgContentRoot.childCount];
        for(int i = 0; i < bgContentMags.Length; i++) {
            bgContentMags[i] = i * backgroundLayerMag;
        }
    }
    void Update() {
        CheckShakeCamera();
        if(player == null)
        {
            player = StageManager.instance.player;
        }
        switch(cameraMode)
        {
            case CameraMode.Fixed:
            if(shakeTime == 0)
            {
                initialPosition = transform.position;
            }
            break;
            case CameraMode.HoldOn:
            initialPosition = new Vector3(player.transform.position.x, 0, transform.position.z);
            MoveToPlayer();
            break;
        }
    }
    void MoveToPlayer() {
        Vector3 targetPos = player.transform.position;
        float amount = targetPos.x - transform.position.x;

        this.transform.DOMoveX(targetPos.x, .5f).SetEase(Ease.OutCubic)
        .OnPlay(() => {AdjustBGLayers(amount);});
    }
    void AdjustBGLayers(float amount) {
        for(int i = 0; i < bgContentMags.Length; i++) {
            bgContentRoot.GetChild(i).DOMoveX(amount * bgContentMags[i], 0.5f).SetEase(Ease.OutCubic);
        }
    }
    void CheckShakeCamera() {
        if (shakeTime > 0)
        {
            Vector3 tmp = Random.insideUnitSphere * cameraShakeAmount + initialPosition;
            transform.position = new Vector3(tmp.x, tmp.y, transform.position.z);
            shakeTime -= Time.deltaTime;
            if(shakeTime <= 0) 
            {
                shakeTime = 0.0f;
                transform.position = initialPosition;
            }
        }
    }
    public void VibrateForTime(float time, float power)
    {
        shakeTime = time;
        cameraShakeAmount = power;
    }
    public void OnTriggerEnter2D(Collider2D col)
    {
        if(col.CompareTag("Enemy"))
        {
            Roamer r = col.GetComponent<Roamer>();
            if(r == null)
            {
                return;
            }
            else if(r.isInTheCamera)
            {
                r.TakeHit(-1);
            }
            else
            {
                r.isInTheCamera = true;
            }
        }
    }
}
