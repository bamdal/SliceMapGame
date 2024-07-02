using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public enum StageName
{
    Stage1 =2,
    Stage2,
    Stage3,
    Stage4,
    Stage5,
}

public class Portal : MonoBehaviour
{
    public StageName selectStage = StageName.Stage1;

    public bool isOpen = true;

    ParticleSystem portalParticle;

    private void Awake()
    {
        portalParticle = transform.GetChild(1).GetComponent<ParticleSystem>();
        if (isOpen)
        {
            portalParticle.Play();
        }
        else
        {
            portalParticle.Stop();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isOpen && other.CompareTag("Player"))
        {

            SceneManager.LoadScene((int)selectStage);
        }
    }

}
