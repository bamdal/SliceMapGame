using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class OptionUI : MonoBehaviour
{
    OptionInputActions inputActions;

    CanvasGroup canvasGroup;

    Button ESCButton;


    private void Awake()
    {
        inputActions = new OptionInputActions();
        canvasGroup = GetComponent<CanvasGroup>();
        ESCButton = GetComponentInChildren<Button>();
        ESCButton.onClick.AddListener(() => { Application.Quit(); });
    }

    private void OnEnable()
    {
        inputActions.Enable();
        inputActions.Option.ESC.performed += OnESC;
    }



    private void OnDisable()
    {
        inputActions.Option.ESC.performed -= OnESC;
        inputActions.Disable();

    }

    private void OnESC(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (canvasGroup.interactable)
            {
                // 화면 끄기
                canvasGroup.interactable = false;
                canvasGroup.alpha = 0f;
                GameManager.Instance.GameState = GameState.GamePlaying;
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                // 화면 키기
                canvasGroup.interactable = true;
                canvasGroup.alpha = 1f;
                GameManager.Instance.GameState = GameState.GamePaused;
                Cursor.lockState = CursorLockMode.None;

            }
        }
    }
}
