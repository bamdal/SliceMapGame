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


    public void SliceMesh(List<Plane> intersectingPlanes)
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

        GameManager gameManager = GameManager.Instance;

        inObject = gameManager.InObject;
        outObject = gameManager.OutObject;

        Plane plane = intersectingPlanes[0];
        intersectingPlanes.RemoveAt(0);

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
                    RefreshMesh(index, mesh, submesh);

                }
                else
                {
                    submesh = Instantiate(this.gameObject, inObject.transform);
                    RefreshMesh(index, mesh, submesh);


                    if (intersectingPlanes.Count > 0)
                    {
                        SliceObject sliceObject = submesh.GetComponent<SliceObject>();
                        sliceObject.SliceMesh(intersectingPlanes);
                        Destroy(sliceObject.gameObject);
                    }
                    else
                    {

                        // 맨 마지막 에 깔끔하게 잘려진 오브젝트들
                        submesh.gameObject.SetActive(false);
                        if (!gameManager.Player.TogglePolaroid && gameManager.GetCamera)
                        {
                            inObject.SliceObjectInList(submesh);

                        }
                    }


                }
                //submesh.gameObject.transform.position += (2 * transform.right); // 소환 위치
              
    

            }

            //gameObject.SetActive(false);


        }
    }

    /// <summary>
    /// 메시 이름, 메시 정보 콜라이더 재정리
    /// </summary>
    /// <param name="index"></param>
    /// <param name="mesh"></param>
    /// <param name="submesh"></param>
    private static void RefreshMesh(int index, Mesh mesh, GameObject submesh)
    {
        submesh.name = $"{submesh.name}_Slice_{index}";

        MeshFilter filter = submesh.GetComponent<MeshFilter>();   // 메시 적용
        filter.sharedMesh = mesh;


        BoxCollider boxCollider = submesh.GetComponent<BoxCollider>();
        if (boxCollider != null)
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
}
