using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartEvent : MonoBehaviour
{
    public Button btnStart;

    void Start()
    {

        if (btnStart != null)
        {
            btnStart.onClick.AddListener(LoadNextScene);
        }
        else
        {
            Debug.LogWarning("btnStart is not assigned in the Inspector!");
        }
    }

    public void LoadNextScene()
    {

        SceneManager.LoadScene("Scene_Demo");
    }
}
