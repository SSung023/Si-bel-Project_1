using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Blackboard
{
    public static CommonVariable commonVariable = new CommonVariable();

    // 월드맵 리셋 여부
    public static bool isReset = false;
    // 플레이어의 월드 진행도
    public static int WorldProgress = 1;
    
    // 플레이어의 스테이지 진행도
    public static int W1StageProgress = 0; //world 1 stage progress
    public static bool[] W1StageClearInfo = {false, false, false, false, false, false, false}; //world 1 stage clear information: clear -> true
    
    //플레이어 월드 내에서의 위치(Index): 0 ~ 6
    public static int WGirlPos = 0;
    public static int WSelecterPos = 0;
    
    
    // 전역 변수
    
    // Game Exit
    public static void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); // 어플리케이션 종료
#endif

    }
}
