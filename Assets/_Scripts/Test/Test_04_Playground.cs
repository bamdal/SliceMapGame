using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_04_Playground : TestBase
{
    protected override void OnTest1(InputAction.CallbackContext context)
    {
        Player player = GameManager.Instance.Player;
        player.PlayerSliceBox.CheackSlice();
    }
}
