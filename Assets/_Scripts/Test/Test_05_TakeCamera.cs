using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_05_TakeCamera : TestBase
{
#if UNITY_EDITOR

    public int cutCount = 5;
    protected override void OnTest1(InputAction.CallbackContext context)
    {
        GameManager.Instance.PlayerUseCamara(true);
    }
    protected override void OnTest2(InputAction.CallbackContext context)
    {
        GameManager.Instance.SetCutCount(cutCount);
    }
    #endif
}
