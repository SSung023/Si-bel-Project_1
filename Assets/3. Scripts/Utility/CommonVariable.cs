using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonVariable
{
    public enum PlayerType {Fire, Blood, Star};
    public PlayerType playerType;

    public bool isInputAcceptable; // input을 받을 수 있는가의 여부
    public bool isMenuOff;
    

    public CommonVariable () {
        playerType = PlayerType.Fire;
        isInputAcceptable = true;
        isMenuOff = false;
    }
    
    
}
