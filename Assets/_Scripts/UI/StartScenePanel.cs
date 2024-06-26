using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartScenePanel : MonoBehaviour
{
    Button startButton;

    Button exitButton;


    private void Awake()
    {
        startButton = transform.GetChild(1).GetComponent<Button>();
        exitButton = transform.GetChild(2).GetComponent<Button>();
    }

    private void Start()
    {
        startButton.onClick.AddListener(() => { SceneManager.LoadScene(1); });
        exitButton.onClick.AddListener(() => { Application.Quit(); });
    }
}
