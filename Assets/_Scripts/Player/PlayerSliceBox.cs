using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSliceBox : MonoBehaviour
{
    /// <summary>
    /// 플레이어가 잘라낼수 있는 범위의 콜라이더
    /// </summary>
    //BoxCollider slicerBox;

    Vector3 boxSize = Vector3.zero;

    /// <summary>
    /// 우,좌,상,하,앞,뒤 순서로 위치조정
    /// </summary>
    public Transform[] boxTransforms = new Transform[6];
    //public BoxCollider SlicerBox => slicerBox;



    Player player;

    bool delay = true;

    public Action<Collider> onColliderInTrigger;

    Bounds bounds;

    public Bounds BoxBounds => bounds;

    private void Awake()
    {
/*        slicerBox = GetComponent<BoxCollider>();
        slicerBox.size = boxSize;
        bounds = slicerBox.bounds;
        slicerBox.enabled = false;
*/

    }

    private void Start()
    {
        player = GameManager.Instance.Player;
        boxSize = player.boxSize;
        transform.position= player.boxCenter;

        boxTransforms[0].transform.position = new(transform.position.x + boxSize.x / 2, transform.position.y, transform.position.z);
        boxTransforms[1].transform.position = new(transform.position.x - boxSize.x / 2, transform.position.y, transform.position.z);
        boxTransforms[2].transform.position = new(transform.position.x, transform.position.y + boxSize.y / 2, transform.position.z);
        boxTransforms[3].transform.position = new(transform.position.x, transform.position.y - boxSize.y / 2, transform.position.z);
        boxTransforms[4].transform.position = new(transform.position.x, transform.position.y, transform.position.z + boxSize.z / 2);
        boxTransforms[5].transform.position = new(transform.position.x, transform.position.y, transform.position.z - boxSize.z / 2);
        /* slicerBox.enabled = false;*/
    }

    public void CheackSlice()
    {
        /*        slicerBox.enabled = true;
                Delay(0.1f);
        */
        Matrix4x4 matrix = Matrix4x4.TRS(transform.parent.position, transform.parent.rotation, transform.localScale);

        // 행렬을 통해 변환된 로컬 위치 계산
        Vector3 transformedLocalPosition = matrix.MultiplyPoint3x4(transform.localPosition);
        Collider[] colliders = Physics.OverlapBox(transformedLocalPosition, boxSize/2, transform.parent.rotation);
        foreach (Collider collider in colliders)
        {
            if(collider.GetComponent<SliceObject>() != null)
                onColliderInTrigger?.Invoke(collider);

        }
    }

/*    private void OnTriggerEnter(Collider other)
    {
        if (delay)
        {
            delay = false;
            StartCoroutine(Delay(1.0f));
            SliceObject slice = other.GetComponent<SliceObject>();
            BoxCollider box = other.GetComponent<BoxCollider>();
            if (slice != null)
            {
                onColliderInTrigger?.Invoke(other);
            }
        }

    }*/

    IEnumerator Delay(float countDelay)
    {
        yield return new WaitForSeconds(countDelay);
/*        slicerBox.enabled = false;*/
        delay = true;
    }

    void OnDrawGizmos()
    {
        // BoxCollider 기즈모 그리기

            Gizmos.color = Color.green;
            Gizmos.matrix = Matrix4x4.TRS(transform.parent.position, transform.parent.rotation, transform.localScale);
            Gizmos.DrawWireCube(transform.localPosition, boxSize);
        
    }
}
