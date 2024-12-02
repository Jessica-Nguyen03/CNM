using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;

public class SelectOptionsEvent : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public RawImage rawImage;
    private WebCamTexture webCam;
    public Button btnChoice;

    // Start is called before the first frame update
    void Start()
    {
        dropdown.onValueChanged.AddListener(OnDropdownValChange);
        checkSelected();
    }

    private void OnDropdownValChange(int arg0)
    {
        checkSelected();
    }

    private void checkSelected()
    {
        int select_index = dropdown.value;
        string select_item = dropdown.options[select_index].text;

        Debug.Log(select_item);

        if (select_item.Equals("Camera"))
        {
            StartCamera();

            btnChoice.gameObject.name = "Take a photo";
        }
        else if (select_item.Equals("File"))
        {
            StopCamera();
            btnChoice.gameObject.name = "BrowseIMG";
    }
        }

    private void StartCamera()
    {
        WebCamDevice[] devices = WebCamTexture.devices;

        if (devices.Length > 0)
        {
            if (webCam != null && webCam.isPlaying)
            {
                webCam.Stop();
            }

            webCam = new WebCamTexture(devices[0].name);
            rawImage.texture = webCam;
            //rawImage.material.mainTexture = webCam;
            webCam.Play();
        }
        else
        {
            Debug.Log("Không có thiết bị camera.");
        }
    }

    private void StopCamera()
    {
        if (webCam != null && webCam.isPlaying)
        {
            webCam.Stop();
           // rawImage.material.mainTexture = null;
            rawImage.texture = null;
        }
        webCam = null;
    }

    /*private void LoadImageFromFile()
    {
        if (!string.IsNullOrEmpty(path))
        {
            byte[] imageData = System.IO.File.ReadAllBytes(path);
            Texture2D texture = new Texture2D(2, 2);
            if (texture.LoadImage(imageData))
            {
                rawImage.texture = texture;
                rawImage.material.mainTexture = texture;
            }
            else
            {
                Debug.LogError("Không thể tải hình ảnh.");
            }
        }
        else
        {
            Debug.Log("Người dùng đã hủy chọn tệp.");
        }
    }
    */
    private void OnDestroy()
    {
        StopCamera();
    }
}
