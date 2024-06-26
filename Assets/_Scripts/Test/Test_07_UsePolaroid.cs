using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_07_UsePolaroid : TestBase
{
#if UNITY_EDITOR
    public SliceObject[] SliceObject;

    public Transform target;

    public int i = 0;
    protected override void OnTest1(InputAction.CallbackContext context)
    {
        SliceObject = GameManager.Instance.InObject.GetComponentsInChildren<SliceObject>();
    }

    protected override void OnTest2(InputAction.CallbackContext context)
    {
        SliceObject[i].transform.position += target.position;
        SliceObject[i].transform.rotation *= target.rotation;
    }
#endif
}
