using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

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

    GameState GameState
    {
        get => gameState;
        set
        {

            if(gameState != value)
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
            cutCount = Mathf.Clamp(value,0,maxCutCount);
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

    public bool GetCamera => getCamera;

    /// <summary>
    /// 사진찍기 쿨타임
    /// </summary>
    public float takePictureCoolTime = 3.0f;

    protected override void OnInitialize()
    {
        player = FindAnyObjectByType<Player>();

        inObject = GetComponentInChildren<InObject>();
        outObject = GetComponentInChildren<OutObject>();

     
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
    }


    void GameStateExit(GameState gameState)
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
        if (cutCoolTime)
        {
            if (getCamera)
            {

                if (CutCount > 0)
                {
                    StartCoroutine(CoolTime(takePictureCoolTime));
                    Debug.Log("사진찍기");
                    CutCount--;
                    player.PlayerSliceBox.CheackSlice();

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
        }else
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
