using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_02_CuttingMap : TestBase
{
    public SliceObject sliceObject;
    public Vector3 point;
    public Vector3 normal = new(0,1,0);
    protected override void OnTest1(InputAction.CallbackContext context)
    {
        sliceObject.SliceMesh(point, normal);
    }
}
