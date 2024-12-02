using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReserEvent : MonoBehaviour
{

    public RawImage rawImage; 
    public Button resetButton;

    void Start()
    {
        resetButton.onClick.AddListener(ResetRawImage);


    }
    public void ResetRawImage()
    {
         if (rawImage != null)
            {
                rawImage.texture = null;
                Debug.Log("reset!");
            }
            else
            {
                Debug.LogWarning("RawImage chưa được gán trong Inspector!");
            }
        }
       
    }


