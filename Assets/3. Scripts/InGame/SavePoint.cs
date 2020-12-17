using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour
{
    [SerializeField] Transform nextCameraPoint;
    bool isActivated;

    void Start()
    {
        isActivated = true;
    }
    public void OnTriggerEnter2D(Collider2D col)
    {
        if(isActivated && col.CompareTag("Player"))
        {
            isActivated = false;
            SoundManager.aud.PlaySnd(1, 18);
            StageManager.instance.UpdateRespawnPoint(transform);
            if(nextCameraPoint != null)
            {
                StageManager.instance.UpdateCameraPoint(nextCameraPoint);
            }
            
        }
    }
}