using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIMenu : MonoBehaviour
{
    public Canvas canvas;
    //public Button retryButton;

    // Start is called before the first frame update
    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.Two))
        {
            if (canvas.gameObject.activeSelf == false)
            {
                canvas.gameObject.SetActive(true);
            } else {
                canvas.gameObject.SetActive(false);
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

}
