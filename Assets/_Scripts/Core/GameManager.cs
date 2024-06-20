using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

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
        }
    }


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
    bool Cutable;

    protected override void OnInitialize()
    {
        player = FindAnyObjectByType<Player>();

        inObject = GetComponentInChildren<InObject>();
        outObject = GetComponentInChildren<OutObject>();

        SetCutCount(1);

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

    public void TakaPicture()
    {
        // 사진 찍기 가능 여부 체크후 횟수감소와 찍기구현
        Debug.Log("사진찍기");
    }

    // 자르는 횟수 제한 
    // 잘랐을 때 잘랐던 영역(B)만 따로 보관후 소환 할때 소환 위치 장애물(A) 자르고  영역(B) 다시 소환
    // 뒤로 감기 혹은 리셋
    // 자른 후 vertex 재정리
}
