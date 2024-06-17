using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliceObject : Slice
{

    MeshFilter Filter;

    InObject inObject;

    OutObject outObject;

    private void Awake()
    {
        Filter = GetComponent<MeshFilter>();
    }

    private void Start()
    {
        GameManager gameManager = GameManager.Instance;

        inObject = gameManager.InObject;
        outObject = gameManager.OutObject;
    }

    public void SliceMesh(Plane plane)
    {

        Vector3 planeNormal = transform.TransformDirection(plane.normal);
        float planeDistance = plane.distance + Vector3.Dot(plane.normal, transform.position);
        Plane worldPlane = new Plane(planeNormal, planeDistance);
        Mesh[] meshes = Slicer(Filter, worldPlane);
        // 잘린 매쉬 중에 1 번은 범위 안에 있는 오브젝트
        if (meshes != null)
        {
            for (int index = 0; index < meshes.Length; index++)
            {
                Mesh mesh = meshes[index];
                GameObject submesh;
                if (index == 0)
                {
                    submesh = Instantiate(this.gameObject,outObject.transform);

                }
                else
                {
                    submesh = Instantiate(this.gameObject,inObject.transform);
                    submesh.gameObject.SetActive(false);
                }
                //submesh.gameObject.transform.position += (2 * transform.right); // 소환 위치
                submesh.name = $"{submesh.name}_Slice_{index}";
                MeshFilter filter = submesh.GetComponent<MeshFilter>();   // 메시 적용
                filter.sharedMesh = mesh;

            }

            gameObject.SetActive(false);


        }
    }
}
