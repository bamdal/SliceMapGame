using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSliceBox : MonoBehaviour
{
    /// <summary>
    /// 플레이어가 잘라낼수 있는 범위의 콜라이더
    /// </summary>
    BoxCollider slicerBox;

    public BoxCollider SlicerBox => slicerBox;


    Vector3 point;


    Player player;

    bool delay = true;

    public Action<Collider> onColliderInTrigger;

    Bounds bounds;

    public Bounds BoxBounds => bounds;
    private void Awake()
    {
        slicerBox = GetComponent<BoxCollider>();
        bounds = slicerBox.bounds;
        slicerBox.enabled = false;
    }

    private void Start()
    {
        player = GameManager.Instance.Player;
        slicerBox.size = player.boxSize;
        transform.position= player.boxCenter;
        slicerBox.enabled = false;
    }

    public void CheackSlice()
    {
        slicerBox.enabled = true;
        Delay(0.1f);

    }

    private void OnTriggerEnter(Collider other)
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

    }

    IEnumerator Delay(float countDelay)
    {
        yield return new WaitForSeconds(countDelay);
        slicerBox.enabled = false;
        delay = true;
    }

    private void OnDrawGizmos()
    {
        // BoxCollider 기즈모 그리기
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        if (boxCollider != null)
        {
            Gizmos.color = Color.green;
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
            Gizmos.DrawWireCube(boxCollider.center, boxCollider.size);
        }
    }
}
