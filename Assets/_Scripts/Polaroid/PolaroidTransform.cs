using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;
using static UnityEngine.ScreenCapture;

public class PolaroidTransform : MonoBehaviour
{
    /// <summary>
    /// 폴라로이드 프리펩
    /// </summary>
    public GameObject PolaroidPrefab;

    /// <summary>
    /// 폴라로이드에 적용할 이미지 머티리얼
    /// </summary>
    public Material[] polarioidMaterial;

    /// <summary>
    /// 폴라로이드 메시 렌더러
    /// </summary>
    MeshRenderer[] meshRenderer;


    Polaroid[] polaroids;
    //활성화시 0,1.35,2 에 ratation 0,0,0

    /// <summary>
    /// 사진 선택시 이동시켜 확대할 포지션
    /// </summary>
    Vector3 viewPosition = new Vector3(0, 1.35f, 2);

    /// <summary>
    /// 비활성화된 이미지들 위치 ,x값으로 이미지 여러장 조절 (-6~6) 초기 위치 4
    /// </summary>
    Vector3 nomalPosition = new Vector3(4, -2.5f, 10);

    Vector3 destination = Vector3.zero;


    /// <summary>
    /// 현재 선택한 사진 인덱스 (0~MaxCutCount-1);
    /// </summary>
    int currnetPolaroidIndex;

    public int CurrnetPolaroidIndex
    {
        get => currnetPolaroidIndex;
        set
        {

            CurrentPolaroidExit(currnetPolaroidIndex);
            currnetPolaroidIndex = value;
            CurrentPolaroidEnter(currnetPolaroidIndex);


        }
    }

    /// <summary>
    /// 사진찍힌 폴라로이드 검색용 커서
    /// </summary>
    int selectedCurser = 0;


    /// <summary>
    /// 사진이 찍혀 활성화된 폴라로이드 인덱스
    /// </summary>
    List<int> enablePolaroids;



    GameManager gameManager;

    private void Awake()
    {
        enablePolaroids = new List<int>();
    }

    private void Start()
    {
        gameManager = GameManager.Instance;
        StartCoroutine(Init());
        
    }

    private void Update()
    {
        // 목적지로 지속적으로 이동
        transform.localPosition = Vector3.Lerp(transform.localPosition, destination, 0.7f);
    }

    IEnumerator Init()
    {
        polaroids = new Polaroid[gameManager.MaxCutCount];
        meshRenderer = new MeshRenderer[polaroids.Length];
        while (gameManager.InObject.onPolaroidIndex != null)
        {
            yield return null;
        }
        gameManager.InObject.onPolaroidIndex += EnablePolaroid;
        gameManager.onViewPolaroid += SetActivate;

        for (int i = 0; i < polaroids.Length; i++)
        {
            yield return null;
            GameObject obj = Instantiate(PolaroidPrefab, transform);
            //obj.transform.position = transform.position + nomalPosition;
            meshRenderer[i] = obj.GetComponent<MeshRenderer>();

            polaroids[i] = obj?.GetComponent<Polaroid>();
            polaroids[i].SetIndex(i);
            polaroids[i].SetDestination(transform.position + nomalPosition);

            

            obj.SetActive(false);
        }

        destination.y = -5;
    }

    private void OnDestroy()
    {
        gameManager.InObject.onPolaroidIndex -= EnablePolaroid;
        gameManager.onViewPolaroid -= SetActivate;
    }

    /// <summary>
    /// 화면의 사진을 캡쳐할 코루틴
    /// </summary>
    /// <returns></returns>
    IEnumerator RecordFrame(int index)
    {
        yield return new WaitForEndOfFrame();
        var texture = ScreenCapture.CaptureScreenshotAsTexture(StereoScreenCaptureMode.BothEyes);
        polarioidMaterial[index].SetTexture("_BaseMap", texture);
        meshRenderer[index].material = polarioidMaterial[index];

    }

    /// <summary>
    /// 사진이 찍혔을때 해당 폴라로이드 활성화
    /// </summary>
    /// <param name="index">폴라로이드 인덱스</param>
    public void EnablePolaroid(int index)
    {
        StartCoroutine(RecordFrame(index));
        polaroids[index].gameObject.SetActive(true);
        enablePolaroids.Add(index);
    }


    /// <summary>
    /// 폴라로이드 사용
    /// </summary>
    public void DisablePolaroid()
    {
        polaroids[currnetPolaroidIndex].gameObject.SetActive(false);
        enablePolaroids.RemoveAt(selectedCurser);
    }

    public void EnableChild()
    {
        foreach (var index in enablePolaroids)
        {
            polaroids[index].gameObject.SetActive(true);
        }
    }

    public void DisbleChild()
    {
        foreach (var child in polaroids)
        {
            child.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 부울값에 따라 index자동 조정
    /// </summary>
    /// <param name="input"></param>
    public void SetPolaroidIndex(bool input)
    {
        if (gameManager.ViewPolaroid && !gameManager.Player.TogglePolaroid)
        {
            if (input)
            {
                selectedCurser++;

            }
            else
            {
                selectedCurser--;
            }
            selectedCurser = Mathf.Clamp(selectedCurser, 0, enablePolaroids.Count - 1);
            if (enablePolaroids.Count > 0)
            {
                CurrnetPolaroidIndex = enablePolaroids[selectedCurser];
                Debug.Log(CurrnetPolaroidIndex);

            }
        }

    }


    public void Reload()
    {
        if(enablePolaroids.Count>0)
            CurrnetPolaroidIndex = enablePolaroids[0];


    }

    /// <summary>
    /// 현재 커서가 향해서 위로 올라올 폴라로이드
    /// </summary>
    /// <param name="index"></param>
    void CurrentPolaroidEnter(int index)
    {
        polaroids[index].Selected();
    }

    /// <summary>
    /// 현재 커서가 나가서 아래로 내려갈 폴라로이드
    /// </summary>
    /// <param name="index"></param>
    void CurrentPolaroidExit(int index)
    {
        polaroids[index].UnSelected();
    }

    /// <summary>
    /// tab 버튼 활성화시 유뮤
    /// </summary>
    /// <param name="active">true면 탭 눌림 false는 탭에서 손 땜</param>
    void SetActivate(bool active)
    {
        if (active)
        {
            destination = Vector3.zero;
        }
        else
        {
            destination.y = -5.0f;
        }
    }

    /// <summary>
    /// 우클릭시 폴라로이드 보기
    /// </summary>
    /// <param name="view"></param>
    public void ViewCurrentPolaroid(bool view)
    {
        if (view)
        {
            polaroids[CurrnetPolaroidIndex].SetDestination(viewPosition);

        }
        else
        {
            // 원래 위치로
            polaroids[CurrnetPolaroidIndex].SetDestination(nomalPosition + new Vector3(0, polaroids[CurrnetPolaroidIndex].selectYposition, 0));
        }
    }

    public bool TakePolaroidPossible()
    {
        return polaroids[currnetPolaroidIndex].PolaroidSelected;
    }
}
