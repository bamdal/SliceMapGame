using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSliceBox : MonoBehaviour
{
    /// <summary>
    /// 플레이어가 잘라낼수 있는 범위의 콜라이더
    /// </summary>
    BoxCollider slicerBox;


    Vector3 point;


    Player player;

    bool delay = true;
    private void Awake()
    {
        slicerBox = GetComponent<BoxCollider>();
    }

    private void Start()
    {
        player= GameManager.Instance.Player;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (delay)
        {
            delay = false;
            SliceObject slice = other.GetComponent<SliceObject>();
            BoxCollider box = other.GetComponent<BoxCollider>();
            if (slice != null)
            {
                if (Physics.ComputePenetration(slicerBox, slicerBox.transform.position, slicerBox.transform.rotation, box, box.transform.position, box.transform.rotation
                    ,out Vector3 dir, out float distance))
                {
                    point = dir;
                    Debug.Log(dir);
                }

                slice.SliceMesh(point.normalized, player.transform.up);
                slice.SliceMesh(point.normalized, -player.transform.up);
                slice.SliceMesh(point.normalized, player.transform.right);
                slice.SliceMesh(point.normalized, -player.transform.right);
                StartCoroutine(Delay());
            }
        }

    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(3.0f);
        delay = true;
    }
}
