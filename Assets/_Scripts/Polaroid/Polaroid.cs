using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Polaroid : MonoBehaviour
{
    /// <summary>
    /// 휠로 선택되면 올라갈 Y 포지션
    /// </summary>
    public float selectYposition = 1.0f;

    public float polaroidSpeed = 2.0f;

    int index = -1;

    public int Index => index;

    /// <summary>
    /// 사진이 목표로 위치할 위치
    /// </summary>
    Vector3 destination = Vector3.zero;

    /// <summary>
    /// 선택 유무 부울
    /// </summary>
    bool selected = false;

    public bool PolaroidSelected => selected;

    private void OnEnable()
    {
        transform.localRotation = Quaternion.AngleAxis(Random.Range(-5.0f, 5.0f), transform.forward);
    }

    private void Update()
    {
        // 목적지로 지속적으로 이동
        transform.localPosition = Vector3.Lerp(transform.localPosition, destination, 0.7f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Polaroid>().Index > index)
        {
            destination.x -= 1.2f;
        }
    }

    public void SetDestination(Vector3 destination)
    {
       this.destination = destination;
    }

    public void SetIndex(int index)
    {
        this.index = index;
    }

    public void Selected()
    {
        if(!selected)
        {
            destination.y += selectYposition;

        }
        selected = true;
    }

    public void UnSeleted()
    {
        if (selected)
        {
            destination.y -= selectYposition;

        }
        selected= false;
    }
}
