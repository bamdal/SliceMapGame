using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Polaroid : MonoBehaviour
{
    private void OnEnable()
    {
        transform.localRotation = Quaternion.AngleAxis(Random.Range(-5.0f, 5.0f), transform.forward);
    }
}
