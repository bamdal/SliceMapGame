using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : MonoBehaviour
{

    /// <summary>
    /// 칼날의 크기(원점에서 떨어진 거리)
    /// </summary>
    public float knifeSize = 2.0f;

    Slice slice;

    private void Awake()
    {
        Plane plane1 = new Plane(transform.up, new Vector3(transform.position.x,transform.position.y- knifeSize, transform.position.z));
        Plane plane2 = new Plane(-transform.up, new Vector3(transform.position.x,transform.position.y+ knifeSize, transform.position.z));
        Plane plane3 = new Plane(transform.right, new Vector3(transform.position.x- knifeSize, transform.position.y,transform.position.z));
        Plane plane4 = new Plane(-transform.right, new Vector3(transform.position.x+ knifeSize, transform.position.y,transform.position.z));

        // 평면 게임 오브젝트 생성 및 설정
        CreatePlaneObject(plane1, new Vector3(transform.position.x, transform.position.y - knifeSize, transform.position.z), transform.up);
        CreatePlaneObject(plane2, new Vector3(transform.position.x, transform.position.y + knifeSize, transform.position.z), -transform.up);
        CreatePlaneObject(plane3, new Vector3(transform.position.x - knifeSize, transform.position.y, transform.position.z), transform.right);
        CreatePlaneObject(plane4, new Vector3(transform.position.x + knifeSize, transform.position.y, transform.position.z), -transform.right);
    }
    private void Start()
    {
   
    }
    void CreatePlaneObject(Plane plane, Vector3 position, Vector3 normal)
    {
        GameObject planeObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
        planeObject.transform.position = position;
        planeObject.transform.up = normal;

        // 필요에 따라 크기 조정
        planeObject.transform.localScale = new Vector3(1, 1, 1);

        // 필요에 따라 머터리얼 설정
        // Material material = new Material(Shader.Find("Standard"));
        // material.color = Color.white;
        // planeObject.GetComponent<Renderer>().material = material;
    }

    private void OnDrawGizmos()
    {
        // 각 평면을 기즈모로 그리기
        DrawPlane(transform.up, transform.position - transform.up * knifeSize);
        DrawPlane(-transform.up, transform.position + transform.up * knifeSize);
        DrawPlane(transform.right, transform.position - transform.right * knifeSize);
        DrawPlane(-transform.right, transform.position + transform.right * knifeSize);
    }

    // 평면을 기즈모로 그리는 함수
    private void DrawPlane(Vector3 normal, Vector3 point)
    {
        // 평면의 법선 벡터와 한 점을 사용하여 평면을 정의
        Plane plane = new Plane(normal, point);

        // 평면 위의 점들을 정의
        Vector3 p1 = point + Vector3.Cross(normal, Vector3.up).normalized * knifeSize;
        Vector3 p2 = point - Vector3.Cross(normal, Vector3.up).normalized * knifeSize;
        Vector3 p3 = point + Vector3.Cross(normal, Vector3.right).normalized * knifeSize;
        Vector3 p4 = point - Vector3.Cross(normal, Vector3.right).normalized * knifeSize;

        // 기즈모로 평면 그리기
        Gizmos.color = Color.green; // 평면의 색상 설정
        Gizmos.DrawLine(p1, p2);
        Gizmos.DrawLine(p3, p4);
        Gizmos.DrawLine(p1, p3);
        Gizmos.DrawLine(p2, p4);
    }
}
