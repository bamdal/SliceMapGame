using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CameraUI : MonoBehaviour
{
    CanvasGroup canvasGroup;

    TextMeshProUGUI filmCountText;
    
    GameManager gameManager;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        Transform child = transform.GetChild(2);
        child = child.GetChild(1);
        filmCountText = child.GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        gameManager = GameManager.Instance;
        gameManager.Player.onCameraDisplay += OnCameraDisplay;
        gameManager.onCutCount += OnFilmCount;
    }

    private void OnFilmCount(int count)
    {
        filmCountText.text = count.ToString();
    }

    private void OnDestroy()
    {
        if(gameManager.Player!=null)
            gameManager.Player.onCameraDisplay -= OnCameraDisplay;
        gameManager.onCutCount -= OnFilmCount;
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
