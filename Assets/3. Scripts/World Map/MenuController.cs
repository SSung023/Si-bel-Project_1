#pragma warning disable CS0649
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [SerializeField] private GameObject EffectContent;
    private GameObject menuPanel = null; //패널 150, 버튼 210
    private GameObject[] menuButtons; // 버튼 오브젝트
    private GameObject[] menuText; // 텍스트 오브젝트

    private SpriteRenderer menuRenderer;
    private SpriteRenderer[] buttonRenderer, textRenderer;
    private Color defaultColor, highlightColor;
    private float changeTime = 0.3f;
    private int menuIndex;
    public static bool isPossible = true;



    private void Awake()
    {
        menuButtons = new GameObject[3];
        buttonRenderer = new SpriteRenderer[3];
        
        menuText = new GameObject[4];
        textRenderer = new SpriteRenderer[4];

        menuPanel = this.gameObject;
        menuRenderer = menuPanel.GetComponent<SpriteRenderer>();
        
        for (int i = 0; i < menuButtons.Length; i++)
        {
            menuButtons[i] = menuPanel.transform.GetChild(i + 1).gameObject;
        }

        menuText[0] = menuPanel.transform.GetChild(0).gameObject;
        menuText[1] = menuButtons[0].transform.GetChild(0).gameObject;
        menuText[2] = menuButtons[1].transform.GetChild(0).gameObject;
        menuText[3] = menuButtons[2].transform.GetChild(0).gameObject;

        for (int i = 0; i < menuButtons.Length; i++)
        {
            buttonRenderer[i] = menuButtons[i].GetComponent<SpriteRenderer>();
        }
        for (int i = 0; i < menuText.Length; i++)
        {
            textRenderer[i] = menuText[i].GetComponent<SpriteRenderer>();
        }
    }

    private void Start()
    {
        defaultColor = new Color(131, 152,148);
        highlightColor = new Color(214,248,240);
        menuIndex = 0;
        menuButtons[menuIndex].GetComponent<SpriteRenderer>().DOColor(Color.blue, 0.5f);
        InitRenderer();
        
    }

    private void Update()
    {
        UpdateActivateMenu();
        UpdateMenuInput();
        UpdateShowMenu();
    }

    private void UpdateMenuInput()
    {
        if(!Blackboard.commonVariable.isMenuOff) // menu가 열려있을 때
            return;
        
        if (Input.GetKeyDown(KeyCode.DownArrow) && menuIndex + 1 < menuButtons.Length)
        {
            menuButtons[menuIndex].GetComponent<SpriteRenderer>().DOColor(Color.grey, 0.5f);
            menuIndex++;
            menuButtons[menuIndex].GetComponent<SpriteRenderer>().DOColor(Color.blue, 0.5f);
            SoundManager.aud.PlaySnd(2, 2); // Selector uiSfx 재생
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) && menuIndex - 1 >= 0)
        {
            menuButtons[menuIndex].GetComponent<SpriteRenderer>().DOColor(Color.grey, 0.5f);
            menuIndex--;
            menuButtons[menuIndex].GetComponent<SpriteRenderer>().DOColor(Color.blue, 0.5f);
            SoundManager.aud.PlaySnd(2, 2); // Selector uiSfx 재생
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            MenuFunction();
            SoundManager.aud.PlaySnd(2, 1); // Confirm uiSfx 재생
        }
    }

    private void MenuFunction()
    {
        switch (menuIndex)
        {
            case 0:
                Blackboard.commonVariable.isMenuOff = false;
                StartCoroutine(LoadContinueMenu());
                break;
            case 1:
                StartCoroutine(LoadMainMenuScene());
                break;
            case 2:
                Blackboard.ExitGame();
                break;
        }
    }
    
    private void SetAppearMenu()
    {
        Sequence seq = DOTween.Sequence()
            .Append(menuRenderer.DOFade(0.65f, changeTime))
            .Join(textRenderer[0].DOFade(1, changeTime))
            .Join(textRenderer[1].DOFade(1, changeTime))
            .Join(textRenderer[2].DOFade(1, changeTime))
            .Join(textRenderer[3].DOFade(1, changeTime))
            .Join(buttonRenderer[0].DOFade(0.75f, changeTime))
            .Join(buttonRenderer[1].DOFade(0.75f, changeTime))
            .Join(buttonRenderer[2].DOFade(0.75f, changeTime));
    }

    private void SetDisappearMenu()
    {
        Sequence seq = DOTween.Sequence()
            .Append(menuRenderer.DOFade(0, changeTime))
            .Join(textRenderer[0].DOFade(0, changeTime))
            .Join(textRenderer[1].DOFade(0, changeTime))
            .Join(textRenderer[2].DOFade(0, changeTime))
            .Join(textRenderer[3].DOFade(0, changeTime))
            .Join(buttonRenderer[0].DOFade(0, changeTime))
            .Join(buttonRenderer[1].DOFade(0, changeTime))
            .Join(buttonRenderer[2].DOFade(0, changeTime));
    }

    private void UpdateActivateMenu()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!Blackboard.commonVariable.isMenuOff)
            {
                Blackboard.commonVariable.isMenuOff = true;
            }
            else if (Blackboard.commonVariable.isMenuOff)
            {
                Blackboard.commonVariable.isMenuOff = false;
            }
        }
    }

    private void UpdateShowMenu()
    {
        if (!Blackboard.commonVariable.isMenuOff)
        {
            SetDisappearMenu();
        }
        else if (Blackboard.commonVariable.isMenuOff)
        {
            SetAppearMenu();
        }
    }
    
    private void InitRenderer()
    {
        Sequence seq = DOTween.Sequence()
            .Append(menuRenderer.DOFade(0, 0.01f))
            .Join(textRenderer[0].DOFade(0, 0.01f))
            .Join(textRenderer[1].DOFade(0, 0.01f))
            .Join(textRenderer[2].DOFade(0, 0.01f))
            .Join(textRenderer[3].DOFade(0, 0.01f))
            .Join(buttonRenderer[0].DOFade(0, 0.01f))
            .Join(buttonRenderer[1].DOFade(0, 0.01f))
            .Join(buttonRenderer[2].DOFade(0, 0.01f));
    }
    
    IEnumerator LoadContinueMenu()
    {
        isPossible = false;
        Blackboard.commonVariable.isMenuOff = false;
        yield return new WaitForSeconds(2.0f);
        isPossible = true;
    }

    IEnumerator LoadMainMenuScene()
    {
        Sequence seq = DOTween.Sequence()
            .PrependInterval(0.4f)
            .Append(EffectContent.transform.DOMoveX(25, 1.3f));
        
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("MainMenu");
        Blackboard.commonVariable.isMenuOff = false;
    }
}
