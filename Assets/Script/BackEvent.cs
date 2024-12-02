using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BackEvent : MonoBehaviour
{
    public Button btnBack;

    void Start()
    {
       
        if (btnBack != null)
        {
            btnBack.onClick.AddListener(LoadNextScene);
        }
        else
        {
            Debug.LogWarning("btnBack is not assigned in the Inspector!");
        }
    }

    public void LoadNextScene()
    {

        SceneManager.LoadScene("Scene_Intro");
    }
}
