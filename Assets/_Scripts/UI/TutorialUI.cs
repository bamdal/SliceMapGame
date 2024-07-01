using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialUI : MonoBehaviour
{
    TextMeshProUGUI text;

    CanvasGroup canvasGroup;

    Transform target;

    float textSpeed = 2f;

    Vector3 startPos;

    int cutcountZero = 0;

    GameManager gameManager;

    public float logTime =3.0f; 
    private void Awake()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        startPos = text.transform.position;
        canvasGroup = GetComponent<CanvasGroup>();
        target = transform.GetChild(1);
    }

    private void Start()
    {
        gameManager = GameManager.Instance;
        StartCoroutine(Log("카메라를 주우세요"));
        gameManager.onCutCount += OnCutCount;
        gameManager.onGetCamera += OnGetCamera;
        gameManager.onViewPolaroid += OnViewPolaroid;
    }

    private void OnDestroy()
    {
        gameManager.onViewPolaroid -= OnViewPolaroid;
        gameManager.onGetCamera -= OnGetCamera;
        gameManager.onCutCount -= OnCutCount;
    }
    private void OnViewPolaroid(bool view)
    {
        if (view)
        {
            StopAllCoroutines();
            StartCoroutine(Log("휠로 사진 선택 후 우클릭을 눌러 폴라로이드를 확대하고 왼클릭으로 덮어써요"));
        }
    }

    private void OnGetCamera()
    {
        StopAllCoroutines(); StartCoroutine(Log("우클릭을 눌러 카메라를 열고 왼클릭으로 사진을 찍어요"));
    }

    private void OnCutCount(int count)
    {
        if (count < cutcountZero)
        {
            StopAllCoroutines(); StartCoroutine(Log("카메라를 닫고 Tab을 눌러 폴라로이드를 꺼내요"));
        }
        cutcountZero = count;
    }

    float elapsedTime =0;
    IEnumerator Log(string logText)
    {
        this.text.text = logText;
        transform.position = startPos;
        canvasGroup.alpha = 1.0f;
        elapsedTime = 0;
        while (elapsedTime < 1.0f)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
            text.transform.position = Vector3.Lerp(text.transform.position, target.position, textSpeed*Time.deltaTime);
        }
        elapsedTime = 0;
        while (elapsedTime < logTime)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
            canvasGroup.alpha = 1 - elapsedTime / logTime;
        }
    }
}
