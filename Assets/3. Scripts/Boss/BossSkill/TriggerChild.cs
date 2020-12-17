using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerChild : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col) {
        transform.parent.GetComponent<Skill>().OnLightCollisionEnter2D(col);
    }
}
