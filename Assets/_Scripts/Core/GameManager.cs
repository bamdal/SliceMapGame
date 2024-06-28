using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    None,
    GameReady,
    GameStarted,
    GameFinished,
    GamePaused

}

public class GameManager : Singleton<GameManager>
{
    Player player;

    public Player Player
    {
        get
        {
            if (player == null)
            {
                player = FindAnyObjectByType<Player>();
            }
            return player;
        }
    }

    GameState gameState = GameState.None;

    public GameState GameState
    {
        get => gameState;
        set
        {

            if (gameState != value)
            {
                GameStateExit(gameState);
                gameState = value;
                GameStateEnter(gameState);
            }
        }
    }

    /// <summary>
    /// 자르는 최대 횟수
    /// </summary>
    int maxCutCount = 5;

    /// <summary>
    /// 자르는 최대 횟수
    /// </summary>
    public int MaxCutCount => maxCutCount;

    /// <summary>
    /// 자르는 횟수
    /// </summary>
    int cutCount = 5;

    /// <summary>
    /// 자르는 횟수 프로퍼티
    /// </summary>
    public int CutCount
    {
        get => cutCount;
        set
        {
            cutCount = Mathf.Clamp(value, 0, maxCutCount);
            onCutCount?.Invoke(cutCount);
        }
    }


    public Action<int> onCutCount;

    InObject inObject;

    /// <summary>
    /// 잘랐을때 안에 있는 메시 저장 장소
    /// </summary>
    public InObject InObject => inObject;

    OutObject outObject;

    /// <summary>
    /// 잘랐을떄 밖에 있는 메시 저장 장소
    /// </summary>
    public OutObject OutObject => outObject;



    /// <summary>
    /// 자르기 가능 유무 체크
    /// </summary>
    bool cutCoolTime;

    /// <summary>
    /// 카메라 소유 유무
    /// </summary>
    bool getCamera;

    public bool GetCamera
    {
        get => getCamera;
        set
        {
            getCamera = value;
            if (getCamera)
            {
                onGetCamera?.Invoke();
            }
        }
    }

    public Action onGetCamera;

    bool viewPolaroid;

    /// <summary>
    /// tab 누르고 있는중
    /// </summary>
    public bool ViewPolaroid
    {
        get => viewPolaroid;
        set
        {
            if (viewPolaroid != value)
            {

                viewPolaroid = value;
                onViewPolaroid?.Invoke(viewPolaroid);
            }
        }
    }

    /// <summary>
    /// 탭 누를시 폴라로이드 보는 타이밍 체크용
    /// </summary>
    public Action<bool> onViewPolaroid;

    /// <summary>
    /// 사진찍기 쿨타임
    /// </summary>
    public float takePictureCoolTime = 3.0f;

    
    // 리셋용 변수들
    Coroutine resetSceneCorutine;
    AsyncOperation resetSceneAsync;

    float elapsedTime = 0;
    bool timeOut => elapsedTime > timeThreshold;

    float timeThreshold = 1.2f;

    public float TimeThreshold => timeThreshold;

    public Action<bool> onTimeOut;


    protected override void OnInitialize()
    {
        //onTimeOut = null;
        //onViewPolaroid = null;
        //onCutCount = null;
        player = FindAnyObjectByType<Player>();
        if(inObject == null)
            inObject = GetComponentInChildren<InObject>();
        if(outObject == null)
            outObject = GetComponentInChildren<OutObject>();
        //inObject.onPolaroidIndex = null;
        inObject.Init();
        GameRefresh();
    }

    /// <summary>
    /// 게임 재시작시 초기화
    /// </summary>
    void GameRefresh()
    {
        SetCutCount(0);
        getCamera = false;
        cutCoolTime = true;
        StartCoroutine(DestroyChild(inObject.transform, 100));
        StartCoroutine(DestroyChild(outObject.transform, 100));
    }

    IEnumerator DestroyChild(Transform transform, int loopCount)
    {
        int count = 0;
        while (transform.childCount > 0 && count < loopCount)
        {
            Destroy(transform.GetChild(0).gameObject);
            count++;
            yield return null;  
        }
    }

    /// <summary>
    /// R버튼 누르고 있을시 재시작 코루틴 시작
    /// </summary>
    /// <param name="reset">true일때 버튼누르는중 false는 버튼에서 손 땜</param>
    public void ReloadScene(bool reset)
    {
        onTimeOut?.Invoke(reset);
        if (reset)
        {
            if (resetSceneCorutine == null)
            {
                elapsedTime = 0;
                resetSceneCorutine = StartCoroutine(AsyncReloadScene());
                Debug.Log("리셋누름");
            }
        }
        else
        {
            if (resetSceneCorutine != null)
            {
                StopCoroutine(resetSceneCorutine);
                resetSceneCorutine = null;
                resetSceneAsync = null;
                Debug.Log("리셋종료");
            }
        }
    }

    /// <summary>
    /// 씬 재시작 코루틴
    /// </summary>
    /// <returns></returns>
    IEnumerator AsyncReloadScene()
    {
        
        while (!timeOut)
        {
            elapsedTime += Time.deltaTime;
            Debug.Log(elapsedTime);
            yield return null;
            if (timeOut)
            {
                onTimeOut = null;   // 씬이동 후 전에 남아있던 UI 접근 방지 초기화
                resetSceneAsync = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    void GameStateExit(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.None:
                GameRefresh();
                break;
            case GameState.GameReady:
                GameRefresh();
                break;
            case GameState.GameStarted:
                break;
            case GameState.GameFinished:
                break;
            case GameState.GamePaused:
                break;
        }
    }

    void GameStateEnter(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.None:
                break;
            case GameState.GameReady:
                break;
            case GameState.GameStarted:

                break;
            case GameState.GameFinished:
                break;
            case GameState.GamePaused:
                break;
        }
    }

    /// <summary>
    /// 자르는 횟수 설정
    /// </summary>
    /// <param name="count">자를 횟수</param>
    public void SetCutCount(int count)
    {
        CutCount = count;
    }

    /// <summary>
    /// 자르는 횟수 1 증가
    /// </summary>
    public void AddCutCount()
    {
        CutCount++;
    }

    /// <summary>
    /// 사진 찍기 함수
    /// </summary>
    public void TakaPicture()
    {
        // 사진 찍기 가능 여부 체크후 횟수감소와 찍기구현
        if (player.TogglePolaroid)
        {

            // 폴라로이드
            player.PlayerSliceBox.CheackSlice();


        }
        else if (cutCoolTime)
        {
            if (getCamera)
            {

                if (CutCount > 0)
                {
                    StartCoroutine(CoolTime(takePictureCoolTime));
                    if (player.TogglePolaroid)
                    {
                        // 폴라로이드
                        player.PlayerSliceBox.CheackSlice();
                    }
                    else
                    {
                        CutCount--;
                        Debug.Log("사진찍기");
                        player.PlayerSliceBox.CheackSlice();
                    }


                }
                else
                {
                    Debug.Log($"현재 사진 찍기 불가능 상태 CutCount : {CutCount}");
                }
            }
            else
            {
                Debug.Log($"현재 사진 찍기 불가능 상태 getCamera : {getCamera}");
            }
        }
        else
        {
            Debug.Log($"현재 사진 찍기 불가능 상태 쿨타임중");
        }


    }

    /// <summary>
    /// 플레이어가 카메라활성화 유무
    /// </summary>
    public void PlayerUseCamara(bool able)
    {
        StopAllCoroutines();
        getCamera = able;
    }

    /// <summary>
    /// 폴라로이드 사진 덮어쓰기
    /// </summary>
    public void PolaroidOverwrite()
    {

    }

    IEnumerator CoolTime(float coolTime)
    {
        cutCoolTime = false;
        yield return new WaitForSeconds(coolTime);
        cutCoolTime = true;
    }


    // 자르는 횟수 제한 
    // 잘랐을 때 잘랐던 영역(B)만 따로 보관후 소환 할때 소환 위치 장애물(A) 자르고  영역(B) 다시 소환
    // 뒤로 감기 혹은 리셋
    // 자른 후 vertex 재정리
}
