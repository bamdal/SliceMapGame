using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageController : MonoBehaviour
{

    public GameState setGameState = GameState.None;

    void Start()
    {
        GameManager gameManager = GameManager.Instance;
        gameManager.GameState = setGameState;
    }


}
