using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Vector3 boxCenter = new Vector3(10, 10, 10);
    public Vector3 boxSize = new Vector3(10, 10, 100);

    private void Awake()
    {
    }

    private void Start()
    {
    }

    public void TakePicture()
    {
        // 박스의 범위 정의
        Bounds boxBounds = new Bounds(transform.position + boxCenter, boxSize);
        // 박스의 6면을 Plane으로 정의
        Plane[] boxPlanes = GetBoxPlanes(boxBounds);

        // OverlapBox를 사용하여 박스와 겹치는 콜라이더 찾기
        Collider[] colliders = Physics.OverlapBox(transform.position + boxCenter, boxSize / 2);

        foreach (Collider collider in colliders)
        {
            Bounds colliderBounds = collider.bounds;

            if (boxBounds.Contains(colliderBounds.min) && boxBounds.Contains(colliderBounds.max))
            {
                Debug.Log($"{collider.name} 완전히 안에 있는 상태");
            }
            else
            {
                Debug.Log($"{collider.name} 테두리에 걸쳐져있는 상태");
                SliceObject sliceObject = collider.GetComponent<SliceObject>();
                if (sliceObject != null)
                {
                    // 콜라이더가 테두리에 걸쳐있는 경우
                    List<Plane> intersectingPlanes = new List<Plane>();
                    
                    if (IsColliderIntersectingPlanes(collider, boxPlanes, boxBounds, out intersectingPlanes))
                    {
                        foreach (Plane plane in intersectingPlanes)
                        {
                            Debug.Log($"{sliceObject.name}이 겹친 평면 노말 : {plane.normal} 거리 : {plane.distance}");
                            sliceObject.SliceMesh(plane);
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// 탐지 범위에 맞게 Plane들 만들기
    /// </summary>
    /// <param name="bounds"></param>
    /// <returns></returns>
    Plane[] GetBoxPlanes(Bounds bounds)
    {
        Vector3 center = bounds.center;
        Vector3 extents = bounds.extents;
      

        Plane[] planes = new Plane[6];
        planes[0] = new Plane(Vector3.right, center + Vector3.right * extents.x); // 오른쪽 면
        planes[1] = new Plane(Vector3.left, center + Vector3.left * extents.x); // 왼쪽 면
        planes[2] = new Plane(Vector3.up, center + Vector3.up * extents.y); // 위쪽 면
        planes[3] = new Plane(Vector3.down, center + Vector3.down * extents.y); // 아래쪽 면
        planes[4] = new Plane(Vector3.forward, center + Vector3.forward * extents.z); // 앞쪽 면
        planes[5] = new Plane(Vector3.back, center + Vector3.back * extents.z); // 뒤쪽 면

        return planes;
    }

    /// <summary>
    /// 겹쳐진 평면값을 구하기
    /// </summary>
    /// <param name="collider"></param>
    /// <param name="boxPlanes"></param>
    /// <param name="boxBounds"></param>
    /// <param name="planes">겹쳐진 평면 리스트</param>
    /// <returns></returns>
    bool IsColliderIntersectingPlanes(Collider collider, Plane[] boxPlanes, Bounds boxBounds, out List<Plane> planes)
    {
        planes = new List<Plane>();

        Vector3[] colliderVertices = GetColliderVertices(collider);

        foreach (Plane plane in boxPlanes)
        {
            bool hasPositive = false;
            bool hasNegative = false;

            foreach (Vector3 vertex in colliderVertices)
            {
                float distance = plane.GetDistanceToPoint(vertex);

                if (distance > 0) hasPositive = true;
                if (distance < 0) hasNegative = true;

                // If there are vertices on both sides of the plane, the collider intersects this plane
                if (hasPositive && hasNegative)
                {
                    planes.Add(plane);
                    break;
                }
            }
        }

        return planes.Count > 0;
    }

    /// <summary>
    /// 함수에서 박스 콜라이더의 정점들을 월드 좌표계로 변환
    /// </summary>
    /// <param name="collider"></param>
    /// <returns></returns>
    Vector3[] GetColliderVertices(Collider collider)
    {
        if (collider is BoxCollider boxCollider)
        {
            Vector3 center = boxCollider.center;
            Vector3 size = boxCollider.size / 2;

            Vector3[] vertices = new Vector3[8];

            vertices[0] = center + new Vector3(size.x, size.y, size.z);
            vertices[1] = center + new Vector3(size.x, size.y, -size.z);
            vertices[2] = center + new Vector3(size.x, -size.y, size.z);
            vertices[3] = center + new Vector3(size.x, -size.y, -size.z);
            vertices[4] = center + new Vector3(-size.x, size.y, size.z);
            vertices[5] = center + new Vector3(-size.x, size.y, -size.z);
            vertices[6] = center + new Vector3(-size.x, -size.y, size.z);
            vertices[7] = center + new Vector3(-size.x, -size.y, -size.z);

            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = collider.transform.TransformPoint(vertices[i]);
            }

            return vertices;
        }

        // 다른 종류의 콜라이더는 여기에서 다룰 수 있습니다
        return new Vector3[0];
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawCube(boxCenter + transform.position, boxSize);
    }
#endif
}
