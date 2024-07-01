using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_08_RendererTexture : TestBase
{
    public GameObject image;

    MeshRenderer meshRenderer;

    public Material material;

    private void Start()
    {
        meshRenderer = image.GetComponent<MeshRenderer>();

    }
#if UNITY_EDITOR

    protected override void OnTest1(InputAction.CallbackContext context)
    {
        StartCoroutine(RecordFrame());
    }

    IEnumerator RecordFrame()
    {
        yield return new WaitForEndOfFrame();
        var texture = ScreenCapture.CaptureScreenshotAsTexture();
        material.SetTexture("_BaseMap", texture);
        meshRenderer.material = material;

    }

#endif
}
