#pragma warning disable CS0649
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainController : MonoBehaviour
{
    [SerializeField] private GameObject pressKey;
    [SerializeField] private GameObject effectContents;
    private GameObject menuSelecter;
    private GameObject[] menuContents; // 0 ~ 1
    private SpriteRenderer[] menuRenderer;
    private GameObject[] menuButton; // 0 ~ 3
    private SpriteRenderer[] buttonRenderer;
    private Animator animator;

    private bool isMenuUp;
    private int menuIndex;
    
    
    
    private void Awake()
    {
        menuContents = new GameObject[2];
        menuButton = new GameObject[3];
        menuSelecter = transform.GetChild(0).gameObject;

        for (int i = 0; i < 2; i++)
        {
            menuContents[i] = transform.GetChild(i + 1).gameObject;
        }

        for (int i = 0; i < 3; i++)
        {
            menuButton[i] = transform.GetChild(i + 3).gameObject;
        }

        animator = pressKey.GetComponent<Animator>();
    }

    private void Start()
    {
        isMenuUp = false;
        menuIndex = 0;
    }
    
    private void Update()
    {
        UpdateKeyPress();
        UpdateMenuSelecter();
    }
    private void UpdateKeyPress()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow) && menuIndex + 1 < 3)
        {
            menuIndex++;
            SoundManager.aud.PlaySnd(2,2);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) && menuIndex - 1 >= 0)
        {
            menuIndex--;
            SoundManager.aud.PlaySnd(2,2);
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            UpdateMenu();
            SoundManager.aud.PlaySnd(2,1);
        }
    }
    private void UpdateMenu()
    {
        if (!isMenuUp) // press space key 문구가 떠있을 때
        {
            Sequence seq = DOTween.Sequence()
                .OnStart(() => { animator.SetBool("isEnd", true); })
                .Append(pressKey.GetComponent<SpriteRenderer>().DOFade(0, 0.5f))
                .AppendInterval(0.4f)
                .Append(menuSelecter.GetComponent<SpriteRenderer>().DOFade(1, 0.2f))
                .Join(menuContents[0].GetComponent<SpriteRenderer>().DOFade(1, 0.2f))
                .Join(menuContents[1].GetComponent<SpriteRenderer>().DOFade(0.8f, 0.2f))
                .Join(menuButton[0].GetComponent<SpriteRenderer>().DOFade(1, 0.2f))
                .Join(menuButton[1].GetComponent<SpriteRenderer>().DOFade(1, 0.2f))
                .Join(menuButton[2].GetComponent<SpriteRenderer>().DOFade(1, 0.2f));
            isMenuUp = true;
            return;
        }

        switch (menuIndex)
        {
            case 0: // Continue
                StartCoroutine(OnWorldEnter());
                break;
            case 1: // New
                break;
            case 2: // Exit
                Blackboard.ExitGame();
                break;
        }
    }

    private void UpdateMenuSelecter()
    {
        Vector3 vector3 = new Vector3(menuButton[menuIndex].transform.position.x + 2.0f,
            menuButton[menuIndex].transform.position.y);

        menuSelecter.transform.DOMove(vector3, 0.65f, false);
    }
    
    IEnumerator OnWorldEnter()
    {
        Sequence seq = DOTween.Sequence()
            .PrependInterval(0.4f)
            .Append(effectContents.transform.DOMoveX(25, 1.3f));
        
        yield return new WaitForSeconds(2.0f);
        
        SceneManager.LoadScene("World Map");
    }
    
    

}
