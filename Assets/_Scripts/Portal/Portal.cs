using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public enum StageName
{
    Stage1 =2,
}

public class Portal : MonoBehaviour
{
    public StageName selectStage = StageName.Stage1;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            SceneManager.LoadScene((int)selectStage);
        }
    }

}
