using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class StageController : MonoBehaviour
{

    public GameState setGameState = GameState.None;

    

    void Start()
    {
        GameManager gameManager = GameManager.Instance;
        gameManager.GameState = setGameState;
        Cursor.lockState = CursorLockMode.Locked;
        StartCoroutine(DestroyChild(gameManager.InObject.transform, 100));
        StartCoroutine(DestroyChild(gameManager.OutObject.transform, 100));

    }
    IEnumerator DestroyChild(Transform transform, int loopCount)
    {
        int count = 0;
        while (transform.childCount > 0 && count < loopCount)
        {
            Destroy(transform.GetChild(0).gameObject);
            count++;
            yield return null;
        }
    }

}
