using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CameraUI : MonoBehaviour
{
    CanvasGroup canvasGroup;

    TextMeshProUGUI filmCountText;
    

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        Transform child = transform.GetChild(2);
        child = child.GetChild(1);
        filmCountText = child.GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        GameManager.Instance.Player.onCameraDisplay += OnCameraDisplay;
        GameManager.Instance.onCutCount += (count) => { filmCountText.text = count.ToString(); };
    }

    private void OnCameraDisplay(bool cameraActivate)
    {
        if(cameraActivate)
        {
            canvasGroup.alpha = 1;
        }
        else
        {
            canvasGroup.alpha = 0;
        }
    }
}
