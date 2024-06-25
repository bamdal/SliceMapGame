using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolaroidTransform : MonoBehaviour
{

    public GameObject PolaroidPrefab;

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

    /// <summary>
    /// 휠로 선택되면 올라갈 Y 포지션
    /// </summary>
    float selectYposition = 1.0f;

    /// <summary>
    /// 사진끼리의 패딩
    /// </summary>
    float padding = 0.2f;
    private void Awake()
    {

    }

    private void Start()
    {
        polaroids = new Polaroid[GameManager.Instance.MaxCutCount];
        Init();
    }

    void Init()
    {
        for (int i = 0; i < polaroids.Length; i++)
        {
            GameObject obj = Instantiate(PolaroidPrefab, transform);
            obj.transform.position = transform.position + nomalPosition;
            polaroids[i] = obj?.GetComponent<Polaroid>();
            obj.SetActive(false);
        }
    }




}
