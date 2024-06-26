using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_06_GetPolaroid : TestBase
{
#if UNITY_EDITOR
    protected override void OnTest1(InputAction.CallbackContext context)
    {
        Dictionary<int, List<GameObject>> test = GameManager.Instance.InObject.Test_GetPolaroid();
        Debug.Log(test.Count);
        if (test.Count > 0)
        {
            if(test.TryGetValue(0, out List<GameObject> list1))
            {
                Debug.Log(list1);
            }
            if (test.TryGetValue(1, out List<GameObject> list2))
            {
                Debug.Log(list2);

            }
            if (test.TryGetValue(2, out List<GameObject> list3))
            {
                Debug.Log(list3);
            }
        }
    }

    
#endif
}
