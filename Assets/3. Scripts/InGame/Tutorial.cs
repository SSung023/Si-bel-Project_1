using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    private GameObject tutorial;
    private GameObject spacebar;
    private SpriteRenderer tutorialSpriteRenderer;
    private SpriteRenderer spaceSpriteRenderer;


    private void Start()
    {
        tutorial = transform.gameObject;
        spacebar = transform.GetChild(0).gameObject;
        tutorialSpriteRenderer = tutorial.GetComponent<SpriteRenderer>();
        spaceSpriteRenderer = spacebar.GetComponent<SpriteRenderer>();

        Blackboard.canMove = false;
    }

    
    private void Update()
    {
        ControlTutorialWindow();
    }

    private void ControlTutorialWindow()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Sequence sequence = DOTween.Sequence()
                .Append(tutorialSpriteRenderer.DOFade(0, 0.6f))
                .Join(spaceSpriteRenderer.DOFade(0, 0.6f))
                .OnComplete(() => Blackboard.canMove = true);
        }
        
    }
}
