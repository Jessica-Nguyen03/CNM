using SFB;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BrowseIMGEvent : MonoBehaviour
{

    public RawImage img;
    public Button BtnBrowse;



    // Start is called before the first frame update
    void Start()
    {
        BtnBrowse.onClick.AddListener(OnSelectImageButtonClick);

    }

    void OnSelectImageButtonClick()
    {
        var extensions = new[] {
            new ExtensionFilter("Image Files", "png", "jpg", "jpeg"),
            new ExtensionFilter("All Files", "*" ),
        };
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Chọn ảnh", "", extensions, false);

        if (paths.Length > 0 && !string.IsNullOrEmpty(paths[0]))
        {
            StartCoroutine(LoadImage(paths[0]));
        }
    }

    IEnumerator LoadImage(string path)
    {
        using (WWW www = new WWW("file://" + path))
        {
            yield return www;
            img.texture = www.texture;
        }
    }




}
