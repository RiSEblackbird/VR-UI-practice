using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIMenu : MonoBehaviour
{
    public Canvas mainCanvas;
    public Canvas quitCanvas;
    public GameObject uiHelper;
    //public Button retryButton;

    // Start is called before the first frame update
    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.Two))
        {
            if (quitCanvas.gameObject.activeSelf == false)
            {
                if (mainCanvas.gameObject.activeSelf == false)
                {
                    mainCanvas.gameObject.SetActive(true);
                }
                else
                {
                    mainCanvas.gameObject.SetActive(false);
                }
            }
            else
            {
                quitCanvas.gameObject.SetActive(false);
            }

        }

        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            if (uiHelper.gameObject.activeSelf == false)
            {
                uiHelper.gameObject.SetActive(true);
            }
            else
            {
                uiHelper.gameObject.SetActive(false);
            }

        }
    }

    /* Update is called once per frame
    void Update()
    {
        if (OVRInput.Get(OVRInput.Button.Four))
        {
            // 現在のシーン番号を取得
            int sceneIndex = SceneManager.GetActiveScene().buildIndex;

            // 現在のシーンを再読み込みする
            SceneManager.LoadScene(sceneIndex);
        }
    }
    */

    public void OnRetry()
    {
        // 現在のシーン番号を取得
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;

        // 現在のシーンを再読み込みする
        SceneManager.LoadScene(sceneIndex);

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().name);
    }

    public void AskQuit()
    {
        mainCanvas.gameObject.SetActive(false);
        quitCanvas.gameObject.SetActive(true);
    }

    public void GoQuit()
    {
        Application.Quit();
        mainCanvas.gameObject.SetActive(false);
        quitCanvas.gameObject.SetActive(true);
    }

    public void NoQuit()
    {
        quitCanvas.gameObject.SetActive(false);
        mainCanvas.gameObject.SetActive(true);
        
    }


}
