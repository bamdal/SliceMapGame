
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InObject : MonoBehaviour
{
    int PlayerCutCount;

    /// <summary>
    /// 컷카운트가 내려갈때만 카운트하기위한 인트값
    /// </summary>
    int cutcountZero;

    Dictionary<int,List<GameObject>> polaroidPicture;

    public Action<int> onPolaroidIndex;

    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        // 첫번째 키값을 0으로 하기위함
        polaroidPicture = new Dictionary<int, List<GameObject>>();
        PlayerCutCount = -1;
    }

    private void Start()
    {
        GameManager.Instance.onCutCount += (count) => 
        {
            if (count < cutcountZero)
            {
                PlayerCutCount++;
                onPolaroidIndex?.Invoke(PlayerCutCount);
            }
            cutcountZero = count;
        };
    }

    /// <summary>
    /// 폴라로이드 정보 저장하는 함수
    /// </summary>
    /// <param name="obj"></param>
    public void SliceObjectInList(GameObject obj)
    {
        if (!polaroidPicture.ContainsKey(PlayerCutCount))
        {
            polaroidPicture[PlayerCutCount] = new List<GameObject>();
        }
        polaroidPicture[PlayerCutCount].Add(obj);
    }

    public void SliceObjectEnable(int index)
    {
        polaroidPicture.TryGetValue(index, out List<GameObject> list);
        if (list != null)
        {
            foreach (GameObject obj in list)
            {
                obj.transform.parent = transform;
                obj.SetActive(true);
            }
        }
    }

#if UNITY_EDITOR
    public Dictionary<int, List<GameObject>> Test_GetPolaroid()
    {
        return polaroidPicture;
    }
#endif
}
