using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RestartUI : MonoBehaviour
{
    Image image;
    CanvasGroup canvasGroup;

    float elapsedTime;

    float timeThreshold;
    private void Awake()
    {
        image = GetComponent<Image>();
        canvasGroup = GetComponent<CanvasGroup>();
        elapsedTime = 0;
        image.fillAmount = elapsedTime;
        canvasGroup.alpha = elapsedTime;
    }

    private void Start()
    {
        timeThreshold = GameManager.Instance.TimeThreshold;
        GameManager.Instance.onTimeOut += Restart;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.onTimeOut -= Restart;
    }

    void Restart(bool reset)
    {
        if(reset)
        {
            StartCoroutine(RestartCorutine());

        }
        else
        {
            elapsedTime = 0;
            image.fillAmount = elapsedTime;
            canvasGroup.alpha = elapsedTime;
            StopAllCoroutines();
        }
    }

    IEnumerator RestartCorutine()
    {
        while (elapsedTime < timeThreshold)
        {
            elapsedTime += Time.deltaTime;
            image.fillAmount = elapsedTime;
            canvasGroup.alpha = elapsedTime;
            yield return null;

        }
    }
}
