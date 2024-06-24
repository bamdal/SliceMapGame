
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

    Dictionary<int,List<GameObject>> polaroidPicture = new Dictionary<int,List<GameObject>>();

    private void Awake()
    {
        PlayerCutCount = 0;
    }

    private void Start()
    {
        GameManager.Instance.onCutCount += (count) => 
        {
            if (count < cutcountZero)
            {
                PlayerCutCount++; 

            }
            cutcountZero = count;
        };
    }

    public void SliceObjectInList(GameObject obj)
    {
        if (!polaroidPicture.ContainsKey(PlayerCutCount))
        {
            polaroidPicture[PlayerCutCount] = new List<GameObject>();
        }
        polaroidPicture[PlayerCutCount].Add(obj);
    }

#if UNITY_EDITOR
    public Dictionary<int, List<GameObject>> Test_GetPolaroid()
    {
        return polaroidPicture;
    }
#endif
}
