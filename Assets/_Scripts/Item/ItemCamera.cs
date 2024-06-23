using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCamera : ItemBase
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player 사진기 줍기");
            GameManager.Instance.PlayerUseCamara(true);
            this.gameObject.SetActive(false);
            
        }
    }
}
