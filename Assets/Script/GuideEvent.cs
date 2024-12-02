using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GuideEvent : MonoBehaviour
{
    public Button btnGuide;

    void Start()
    {

        if (btnGuide != null)
        {
            btnGuide.onClick.AddListener(LoadNextScene);
        }
        else
        {
            Debug.LogWarning("btnGuide is not assigned in the Inspector!");
        }
    }

    public void LoadNextScene()
    {

        SceneManager.LoadScene("Scene_Guide");
    }
}
