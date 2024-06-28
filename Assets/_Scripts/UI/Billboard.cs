using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    Player player;

    private void Start()
    {
        player= GameManager.Instance.Player;

    }


    private void Update()
    {
        transform.forward = -Vector3.Lerp(-transform.forward,player.transform.position - transform.position,Time.deltaTime);  
    }
}
