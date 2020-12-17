using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameObject instance;
    
    
    void Awake() {
        #region 싱글톤 구현부분
        if(instance == null) {
            instance = this.gameObject;
        }
        else {
            Destroy(this.gameObject);
        }
        #endregion
        DontDestroyOnLoad(this.gameObject);
    }
    
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
