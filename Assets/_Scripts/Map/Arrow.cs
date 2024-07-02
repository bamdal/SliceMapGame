using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float itemMoveSpeed = 2.0f;

    public float addY = 0.5f;

    float elpasedTime = 0.0f;

    Vector3 origin = Vector3.zero;

    Vector3 move = Vector3.zero;

    GameManager manager;

    private void Awake()
    {
        origin = transform.position;
        
    }

    private void Start()
    {
        manager = GameManager.Instance;
    }

    private void Update()
    {
        if(manager.GameState != GameState.GamePaused)
        {
            elpasedTime += Time.deltaTime * itemMoveSpeed;
            move.y = Mathf.Abs(Mathf.Sin(elpasedTime)) * addY;
            transform.position = new Vector3(transform.position.x, origin.y + move.y, transform.position.z);

        }
    }
}
