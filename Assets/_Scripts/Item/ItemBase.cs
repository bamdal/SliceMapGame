using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBase : MonoBehaviour
{
    public float itemMoveSpeed = 2.0f;

    float elpasedTime = 0.0f;

    Vector3 move = Vector3.zero;

    private void Update()
    {
        elpasedTime += Time.deltaTime* itemMoveSpeed;
        move.y =Mathf.Abs( Mathf.Sin(elpasedTime))*0.1f;
        transform.position = new Vector3(transform.position.x, move.y, transform.position.z);
    }
}
