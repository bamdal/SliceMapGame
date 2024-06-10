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



    Slice slicer;

    public Slice Slicer
    {
        get
        {
            if (slicer == null)
            {
                slicer = GetComponent<Slice>();
            }
            return slicer;
        }
    }
    protected override void OnInitialize()
    {
        slicer = GetComponent<Slice>();
        player = FindAnyObjectByType<Player>();

    }

}
