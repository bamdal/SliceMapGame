using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliceObject : MonoBehaviour
{
    Slice Slicer;

    MeshFilter Filter;

    private void Awake()
    {
        Filter = GetComponent<MeshFilter>();
    }

    private void Start()
    {
        Slicer = GameManager.Instance.Slicer;
    }

    public void SliceMesh(Plane plane)
    {

        Mesh[] meshes = Slicer.Slicer(Filter,plane);
        if (meshes != null)
        {
            for (int index = 0; index < meshes.Length; index++)
            {
                Mesh mesh = meshes[index];

                GameObject submesh = Instantiate(this.gameObject);
                //submesh.gameObject.transform.position += (2 * transform.right); // 소환 위치
                MeshFilter filter = submesh.GetComponent<MeshFilter>();   // 메시 적용
                filter.sharedMesh = mesh;

            }


            gameObject.SetActive(false);

        }
    }
}
