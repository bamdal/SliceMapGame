using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    Player player;

    public Player Player
    {
        get
        {
            if (player == null)
            {
                player = FindAnyObjectByType<Player>();
            }
            return player;
        }
    }



    InObject inObject;

    public InObject InObject => inObject;

    OutObject outObject;

    public OutObject OutObject => outObject;


    protected override void OnInitialize()
    {
        player = FindAnyObjectByType<Player>();

        inObject = GetComponentInChildren<InObject>();
        outObject = GetComponentInChildren<OutObject>();

    }

}
