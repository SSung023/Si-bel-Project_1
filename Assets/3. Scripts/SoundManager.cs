#pragma warning disable CS0649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public static SoundManager aud;

    // 필요 컴포넌트 선언
    AudioSource musicAud;        // BGM 용
    AudioSource sfxAud;          // 인게임 효과음용
    AudioSource uiAud;           // UI 효과음용
    AudioSource ambAud;          // 환경음용
    public AudioListener audioListener;

    // AudioListener 탐색 시 사용할 배열
    private AudioListener[] listeners;

    // 참조할 사운드 파일 선언
    [SerializeField] AudioClip[] musicClip;
    [SerializeField] AudioClip[] sfxClip;
    [SerializeField] AudioClip[] uiClip;
    //[SerializeField] AudioClip[] ambientClip; 나중에 사용할 환경음

    // 사운드 조정 변수 선언 (최솟값 0 ~ 최댓값 1)
    public float bgMusicVol     = 0.3f;    // 배경음 볼륨
    public float ingameSfxVol   = 1.0f;    // 인게임 효과음 볼륨
    public float uiSfxVol       = 1.0f;    // UI 효과음(메인메뉴 등) 볼륨
    public float ambientVol     = 0.8f;    // 환경음(개울소리 등) 볼륨

    /* 
     * 볼륨은 일단 1(최댓값)을 기본값으로 해놓기는 했는데,
     * 차후 게임 발매 시에는 플레이어가 매번 게임을 켤때마다 볼륨을 조절할 필요 없도록, 
     * 외부 파일에 저장할 수 있게 구현하면 좋을 듯 함.
     */

    private void Awake()
    {
        LoadSoundManager();

        FindAudioSource();
        VolumeChangeApply();

        // 다음 씬부터 로드할때마다 PlaySceneMusic이 대리자로 추가
        SceneManager.sceneLoaded += PlaySceneMusic;
        
    }


    // ------------- Public 메서드 -------------

    // AudioListener 컴포넌트 찾아서 할당해주는 메서드
    public void FindListener() {
        // AudioLisnter 컴포넌트 모두 검색함
        listeners = FindObjectsOfType(typeof(AudioListener)) as AudioListener[];

        // 검색해서 발견된 게 있으면
        if (listeners != null) {
            for (int i = 0; i < listeners.Length; ++i) {
                // 활성화된 하이어라키 내에 있는, 가장 먼저 발견된 audioListener 컴포넌트를 할당
                if(listeners[i] && listeners[i].enabled && listeners[i].gameObject.activeInHierarchy) {
                    audioListener = listeners[i];
                    break;
                }
            }
        }
        // AudioListener 컴포넌트가 없으면 Main Camera에 생성해줌
        if(audioListener == null) {
            Camera cam = Camera.main;
            if (cam == null) cam = FindObjectOfType(typeof(Camera)) as Camera;
            if (cam != null) audioListener = cam.gameObject.AddComponent<AudioListener>();
        }
    }
    
    // 씬 로드 때마다 해당 씬 넘버를 파악해서 브금을 재생해주는 메서드, 임시용
    public void PlaySceneMusic(Scene scene, LoadSceneMode mode)
    {
        FindAudioSource();
        int SceneNo = scene.buildIndex;
        switch (SceneNo)
        {
            case 0:
                PlaySnd(0, 0);
                break;
            case 1:
                PlaySnd(0, 1);
                break;
            case 7:
                //PlaySnd(0, 5);
                PlaySnd(0, 4,false);
                Invoke("TempBossLaugh", 8.0f);
                Invoke("TempBossBGMPlayer", 10.0f);
                break;
            case 11:
                
                break;
            default:
                PlaySnd(0, 2);
                break;
        }
    }

    // 모든 소리를 통합적으로 재생하도록 해주는 메서드
    // SoundType 0 : BGM / 1 : 인게임 효과음 / 2 : UI 효과음 / 3 : 환경음
    // loop : true 일 때 BGM 또는 환경음 루프, 효과음은 루프 없음
    public void PlaySnd(int soundType, int index) {
        switch (soundType) {
            case 0:
                // soundType = 0이면 BGM 재생
                PlayBGM(index);
                break;
            case 1:
                // soundType = 1이면 인게임 효과음 재생
                PlaySfx(index);
                break;
            case 2:
                // soundType = 2이면 UI 효과음 재생
                PlayUiSfx(index);
                break;
            case 3:
                // SoundType = 3이면 환경음 재생 - 경진대회 기준 미구현
                break;
        }
    }

    public void PlaySnd(int soundType, int index, bool loop)
    {
        switch (soundType)
        {
            case 0:
                // soundType = 0이면 BGM 재생, loop 이 true면 루프
                PlayBGM(index, loop);
                break;
            case 1:
                // soundType = 1이면 인게임 효과음 재생
                PlaySfx(index);
                break;
            case 2:
                // soundType = 2이면 UI 효과음 재생
                PlayUiSfx(index);
                break;
            case 3:
                // SoundType = 3이면 환경음 재생 - 경진대회 기준 미구현
                break;
        }
    }

    // 인게임 효과음을 재생할 때에는 해당 스크립트에서 이 메서드를 통해 오디오 클립을 가져가서 playoneshot으로 재생한다.
    public AudioClip TakeSndClip(int index)
    {
        if(index >= 0 && index < sfxClip.Length)
        {
            return sfxClip[index];
        }
        else
        {
            string methodInfo = "잘못된 index의 효과음입니다 : TakeSndClip(int " + index + ")";
            Debug.Log(methodInfo);
            return sfxClip[0];
        }
    }

    public void PlayUiSnd(int index)
    {
        if (index >= 0 && index < uiClip.Length)
        {
            uiAud.PlayOneShot(uiClip[index]);
        }
        else
        {
            string methodInfo = "잘못된 index의 효과음입니다 : PlayUiSnd(int " + index + ")";
            Debug.Log(methodInfo);
            uiAud.PlayOneShot(uiClip[0]);
        }
    }
    
    // 특정 소리를 즉시 정지시키는 메서드
    public void StopSnd(int soundType)
    {
        switch (soundType)
        {
            case 0:
                // soundType = 0이면 BGM 재생
                musicAud.Stop();
                break;
            case 1:
                // soundType = 1이면 인게임 효과음 재생
                // 그러나 아직 audioSource 할당 문제로 고민중.. 이부분은 프로퍼티로 구현해야 하지 않나 싶음
                sfxAud.Stop();
                break;
            case 2:
                // soundType = 2이면 UI 효과음 재생
                uiAud.Stop();
                break;
            case 3:
                // SoundType = 3이면 환경음 재생 - 경진대회 기준 미구현
                //ambientAud.Stop();
                break;
        }
    }

    // 모든 소리를 즉시 정지시키는 메서드
    public void StopSnd()
    {
        musicAud.Stop();
        sfxAud.Stop();
        uiAud.Stop();
        //ambientAud.Stop();
    }

    // fadeout 이 True 일 경우 페이드아웃하면서 특정 소리를 끄는 메서드
    public void StopSnd(int soundType, bool fadeout)
    {
        // 미구현
    }

    // fadeout 이 True 일 경우 페이드아웃하면서 모든 소리를 정지시키는 메서드
    public void StopSnd(bool fadeout)
    {
        // 미구현
    }

    // 볼륨 조절을 적용하는 메서드
    public void VolumeChangeApply()
    {
        musicAud.volume = bgMusicVol;
        sfxAud.volume = ingameSfxVol;
        uiAud.volume = uiSfxVol;
        ambAud.volume = ambientVol;
    }

    // --------------- Private 메서드 ---------------

    // SoundManager 게임오브젝트가 aud에 할당되어있는지 확인하고, 중복 방지
    void LoadSoundManager()
    {
        if(aud != null)
        {
            Destroy(gameObject);
            return;
        }
        aud = this;
        DontDestroyOnLoad(gameObject);
    }

    // 음악을 재생하는 메서드, 기본적으로 루프하게 되어있음
    void PlayBGM(int songNum){
        if(songNum >= 0 && songNum < musicClip.Length) {
            if(musicAud != null)
            {
                musicAud.clip = musicClip[songNum];
                musicAud.loop = true;
                musicAud.Play();
            }
        }
        else {
            Debug.Log("범위를 벗어난 배경음악 번호입니다.");
        }
    }
    // 음악을 재생하는 메서드 오버라이딩, True 일 경우 루프, False 일 경우 1회만 재생
    void PlayBGM(int songNum, bool loop) {
        if (songNum >= 0 && songNum < musicClip.Length) {
            musicAud.clip = musicClip[songNum];

            musicAud.loop = loop;
            musicAud.Play();
        }
        else {
            Debug.Log("범위를 벗어난 배경음악 번호입니다.");
        }
    }

    // 인게임 효과음을 재생하는 메서드 - PlayOneShot
    void PlaySfx(int ingameSfxNum)
    {
        if (ingameSfxNum >= 0 && ingameSfxNum < sfxClip.Length)
        {
            sfxAud.PlayOneShot(sfxClip[ingameSfxNum]);
        }
        else
        {
            Debug.Log("범위를 벗어난 UI 효과음 번호입니다.");
        }
    }

    // UI 효과음을 재생하는 메서드 - PlayOneShot
    void PlayUiSfx(int uiSfxNum) {
        if (uiSfxNum >= 0 && uiSfxNum < uiClip.Length) {
            uiAud.PlayOneShot(uiClip[uiSfxNum]);
        }
        else {
            Debug.Log("범위를 벗어난 인게임 효과음 번호입니다.");
        }
    }

    void FindAudioSource()
    {
        AudioSource[] audSources = GetComponents<AudioSource>();
        
        if(audSources.Length != 4)
        {
            print("audSources 수가 4개를 초과했습니다. 오류가 있는지 확인하십시오.");
        }

        musicAud    = audSources[0];
        sfxAud      = audSources[1];
        uiAud       = audSources[2];
        ambAud      = audSources[3];
    }

    // 임시로 보스전 웃음 소리 내는 메서드
    void TempBossLaugh()
    {
        if(SceneManager.GetActiveScene().buildIndex == 7)
        {
            sfxAud.PlayOneShot(aud.TakeSndClip(9));
        }
    }

    // 임시로 보스전 브금 인트로 후 루프돌리는 메서드
    void TempBossBGMPlayer()
    {
        if (SceneManager.GetActiveScene().buildIndex == 7)
        {
            musicAud.clip = musicClip[3];
            musicAud.loop = true;
            musicAud.Play();
        }
    }

}
