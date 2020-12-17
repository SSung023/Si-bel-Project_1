using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    [SerializeField] protected string _name;
    [SerializeField] protected Boss myBoss;
    [SerializeField] protected AudioSource skillAud;

    public string skillName {
        get {
            return _name;
        }
    }
    [HideInInspector] public bool isActivated = false;
    public int skillDuration;

    public virtual void Start()
    {
        skillAud = GetComponent<AudioSource>();
        isActivated = false;
    }

    public void Activate()
    {
        isActivated = true;
        Affect();
    }

    protected virtual void Affect()
    {

    }

    public virtual void OnTriggerEnter2D(Collider2D col)
    {
        
    }
    public virtual void OnLightCollisionEnter2D(Collider2D col)
    {
        
    }
}
