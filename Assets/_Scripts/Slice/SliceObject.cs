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

/*        Quaternion rotation = Quaternion.Euler(-transform.eulerAngles);

        Vector3 planeNormal = rotation * transform.TransformDirection(plane.normal);

        float planeDistance = plane.distance + Vector3.Dot(plane.normal, transform.position);
        Plane worldPlane = new Plane(planeNormal, planeDistance);*/

        /*        Vector3 planeNormal = transform.TransformDirection(plane.normal);
                float planeDistance = Vector3.Dot(transform.TransformPoint(plane.normal * plane.distance), planeNormal);
                Plane worldPlane = new Plane(planeNormal, planeDistance);*/

        // 평면을 월드 좌표계로 변환하고 스케일 조정
        /*        Vector3 planeNormal = Vector3.Scale(transform.TransformDirection(plane.normal), transform.lossyScale).normalized;
                float planeDistance = Vector3.Dot(transform.TransformPoint(plane.normal * plane.distance), planeNormal);
                Plane worldPlane = new Plane(planeNormal, planeDistance);*/

        Mesh[] meshes = Slicer(Filter, plane);
        // 잘린 매쉬 중에 1 번은 범위 안에 있는 오브젝트
        if (meshes != null)
        {
            for (int index = 0; index < meshes.Length; index++)
            {
                Mesh mesh = meshes[index];
                GameObject submesh;
                if (index == 0)
                {
                    submesh = Instantiate(this.gameObject, outObject.transform);
                }
                else
                {
                    submesh = Instantiate(this.gameObject, inObject.transform);
                    submesh.gameObject.SetActive(false);
                }
                //submesh.gameObject.transform.position += (2 * transform.right); // 소환 위치
                submesh.name = $"{submesh.name}_Slice_{index}";
              

                MeshFilter filter = submesh.GetComponent<MeshFilter>();   // 메시 적용
                filter.sharedMesh = mesh;

                BoxCollider boxCollider = submesh.GetComponent<BoxCollider>();
                if(boxCollider != null)
                {
                    boxCollider.center = mesh.bounds.center - boxCollider.transform.position;
                    boxCollider.size = mesh.bounds.size;
                }

                MeshCollider meshCollider = submesh.GetComponent<MeshCollider>();
                if (meshCollider != null)
                {
                    meshCollider.sharedMesh = mesh;
                }
            }

            //gameObject.SetActive(false);


        }
    }
}
