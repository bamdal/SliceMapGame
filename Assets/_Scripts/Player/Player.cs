using System;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Vector3 boxCenter = new Vector3(0, 1.5f, 50);
    public Vector3 boxSize = new Vector3(10, 10, 100);

    PlayerSliceBox playerSliceBox;

    public PlayerSliceBox PlayerSliceBox => playerSliceBox;

    Animator animator;

    readonly int anim_UpCameraHash = Animator.StringToHash("UpCamera");
    readonly int anim_DownCameraHash = Animator.StringToHash("DownCamera");

    /// <summary>
    /// 카메라 상태 구분용 bool (true면 카메라가 올려져 있고 false면 카메라가 내려져 있다)
    /// </summary>
    bool usedCamera = false;

    /// <summary>
    /// 폴라로이드를 볼수있는 상태
    /// </summary>
    bool viewPolaroidPossible = false;

    /// <summary>
    /// 폴라로이드 확대용 토글(true 확대, false 축소)
    /// </summary>
    bool togglePolaroid = false;

    /// <summary>
    /// 폴라로이드 확대용 토글(true 확대, false 축소) 폴라로이드 확대시만 폴라로이드 찍기 가능
    /// </summary>
    public bool TogglePolaroid => togglePolaroid;

    /// <summary>
    /// 카메라 애니메이션 적용할 트랜스폼
    /// </summary>
    Transform CameraTransform;

    public Action<bool> onCameraDisplay;

    /// <summary>
    /// 폴라로이드 사진들의 트랜스폼
    /// </summary>
    PolaroidTransform polaroidTransform;

    public PolaroidTransform PolaroidTransform => polaroidTransform;

    GameManager gameManager;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        CameraTransform = transform.GetChild(2);
        polaroidTransform = GetComponentInChildren<PolaroidTransform>();
    }

    private void Start()
    {
        playerSliceBox = GetComponentInChildren<PlayerSliceBox>(true);
        playerSliceBox.onColliderInTrigger += TakePicture;

        gameManager = GameManager.Instance;
        gameManager.onViewPolaroid += (view) => 
        {
            viewPolaroidPossible = view; 
            if (!view)
            {
                // 폴라로이드 강제로 내리기
                polaroidTransform.ViewCurrentPolaroid(false);
                togglePolaroid = false;
            }
        };
    }

    // camera 관련 =================================================================
    void Anim_UpCamera()
    {
        animator.ResetTrigger(anim_DownCameraHash);
        animator.SetTrigger(anim_UpCameraHash);
    }

    void Anim_DownCamera()
    {
        animator.ResetTrigger(anim_UpCameraHash);
        animator.SetTrigger(anim_DownCameraHash);
    }

    /// <summary>
    /// 애니메이션으로 비활성화
    /// </summary>
    void CameraDisable()
    {
        CameraTransform.gameObject.SetActive(false);
        polaroidTransform.gameObject.SetActive(false);
        onCameraDisplay?.Invoke(usedCamera);
        
    }

    /// <summary>
    /// 애니메이션으로 활성화
    /// </summary>
    void CameraEnable()
    {
        CameraTransform.gameObject.SetActive(true);
        polaroidTransform.gameObject.SetActive(true);
    }

    /// <summary>
    /// 카메라 활성화 토글
    /// </summary>
    void Toggle_Anim_Camera()
    {

        if (gameManager.GetCamera && !gameManager.ViewPolaroid)
        {
            if (usedCamera)
            {
                usedCamera = false;
                Anim_DownCamera();
            }
            else
            {
                usedCamera = true;
                Anim_UpCamera();
            }
        }

    }

    /// <summary>
    /// 사진 찍기
    /// </summary>
    void PlayerTakePicture()
    {
        if (gameManager.GetCamera && usedCamera)
        {
            gameManager.TakaPicture();
            polaroidTransform.Reload();

        }
    }

    /// <summary>
    /// 탭 누르는중에 우클릭시 확대 우클릭 때기, 탭때기 하면 확대 취소
    /// </summary>
    void Toggle_VeiwPolaroid()
    {
        if (!usedCamera && viewPolaroidPossible && polaroidTransform.TakePolaroidPossible())
        {
            if (togglePolaroid)
            {
                Debug.Log("폴라 내리기");
                polaroidTransform.ViewCurrentPolaroid(false);

                togglePolaroid = false;
            }
            else
            {
                Debug.Log("폴라 보기");
                polaroidTransform.ViewCurrentPolaroid(true);
                togglePolaroid = true;
            }
        }
    }

    void TakePolaroid()
    {
        if (!usedCamera && TogglePolaroid && polaroidTransform.TakePolaroidPossible())
        {
            gameManager.TakaPicture();
            polaroidTransform.DisablePolaroid();
            polaroidTransform.ViewCurrentPolaroid(false);
            gameManager.InObject.SliceObjectEnable(polaroidTransform.CurrnetPolaroidIndex);
            togglePolaroid = false;
        }
    }

    public void LClickInput()
    {
        PlayerTakePicture();
        TakePolaroid();
    }

    public void RClickInput()
    {
        Toggle_Anim_Camera();
        Toggle_VeiwPolaroid();
    }

    // mesh Slice 관련 =========================================================================================

    public void TakePicture(Collider collider)
    {
        // 박스의 범위 정의
        Bounds boxBounds = playerSliceBox.BoxBounds;
        // 박스의 6면을 Plane으로 정의
        Plane[] boxPlanes = GetBoxPlanes(boxBounds);

        Vector3[] targetPoints = GetColliderCorners(collider);

        bool isInside = true;

        foreach (Vector3 point in targetPoints)
        {
            if (!IsPointInsidePlanes(point, boxPlanes))
            {
                isInside = false;
                break;
            }
        }

        if (isInside)
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
                    /*                    foreach (Plane plane in intersectingPlanes)
                                        {
                                            Debug.Log($"{sliceObject.name}이 겹친 평면 노말 : {plane.normal} 거리 : {plane.distance}");
                                            sliceObject.SliceMesh(plane);
                                        }*/
                    foreach (Plane plane in intersectingPlanes)
                    {
                        Debug.Log($"{sliceObject.name}이 겹친 평면 노말 : {plane.normal} 거리 : {plane.distance}");
                    }
                        sliceObject.SliceMesh(intersectingPlanes);

                }
            }
        }
        if(TogglePolaroid)
            collider.gameObject.SetActive(false);
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
        planes[0] = new Plane(playerSliceBox.boxTransforms[0].right, playerSliceBox.boxTransforms[0].position); // 오른쪽 면
        planes[1] = new Plane(-playerSliceBox.boxTransforms[1].right, playerSliceBox.boxTransforms[1].position); // 왼쪽 면
        planes[2] = new Plane(playerSliceBox.boxTransforms[2].up, playerSliceBox.boxTransforms[2].position); // 위쪽 면
        planes[3] = new Plane(-playerSliceBox.boxTransforms[3].up, playerSliceBox.boxTransforms[3].position); // 아래쪽 면
        planes[4] = new Plane(playerSliceBox.boxTransforms[4].forward, playerSliceBox.boxTransforms[4].position); // 앞쪽 면
        planes[5] = new Plane(-playerSliceBox.boxTransforms[5].forward, playerSliceBox.boxTransforms[5].position); // 뒤쪽 면
  /*      Debug.Log(transform.forward);
        Debug.Log(transform.TransformDirection(transform.forward));
        Debug.Log(playerSliceBox.transform.TransformPoint(transform.forward * extents.z));
        Debug.Log(transform.TransformPoint(playerSliceBox.transform.position + transform.forward * extents.z));

*/
 

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
        else if (collider is MeshCollider meshCollider)
        {
            Mesh mesh = meshCollider.sharedMesh;
            Vector3[] meshVertices = mesh.vertices;
            Vector3[] worldVertices = new Vector3[meshVertices.Length];

            for (int i = 0; i < meshVertices.Length; i++)
            {
                worldVertices[i] = collider.transform.TransformPoint(meshVertices[i]);
            }

            return worldVertices;
        }
        Debug.Log("다른 콜라이이더 이므로 예외처리 필요");
            return new Vector3[0];
    }

    Vector3[] GetColliderCorners(Collider collider)
    {
        Vector3[] corners = new Vector3[8];
        Bounds bounds = collider.bounds;

        corners[0] = bounds.min;
        corners[1] = new Vector3(bounds.max.x, bounds.min.y, bounds.min.z);
        corners[2] = new Vector3(bounds.min.x, bounds.max.y, bounds.min.z);
        corners[3] = new Vector3(bounds.min.x, bounds.min.y, bounds.max.z);
        corners[4] = new Vector3(bounds.max.x, bounds.max.y, bounds.min.z);
        corners[5] = new Vector3(bounds.max.x, bounds.min.y, bounds.max.z);
        corners[6] = new Vector3(bounds.min.x, bounds.max.y, bounds.max.z);
        corners[7] = bounds.max;

        return corners;
    }

    bool IsPointInsidePlanes(Vector3 point, Plane[] planes)
    {
        foreach (Plane plane in planes)
        {
            // 점이 평면의 법선 반대방향 쪽에 있는 경우 false 반환
            if (plane.GetSide(point))
            {
                return false;
            }
        }
        return true;
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        playerSliceBox = GetComponentInChildren<PlayerSliceBox>(true);
        /*        BoxCollider BoxBounds = playerSliceBox.GetComponent<BoxCollider>();


                Bounds bounds = BoxBounds.bounds;*/
        Bounds bounds = playerSliceBox.BoxBounds;
        Plane[] planes = GetBoxPlanes(bounds);

        // 각 평면을 그리기 위한 Gizmos 색상 설정
        Gizmos.color = Color.red;
        DrawPlaneGizmos(planes[0]);

        Gizmos.color = Color.green;
        DrawPlaneGizmos(planes[1]);

        Gizmos.color = Color.blue;
        DrawPlaneGizmos(planes[2]);

        Gizmos.color = Color.yellow;
        DrawPlaneGizmos(planes[3]);

        Gizmos.color = Color.cyan;
        DrawPlaneGizmos(planes[4]);

        Gizmos.color = Color.magenta;
        DrawPlaneGizmos(planes[5]);
    }
    void DrawPlaneGizmos(Plane plane)
    {
        // 평면의 중심과 방향 벡터를 계산
        Vector3 planeCenter = -plane.normal * plane.distance ;
        Vector3 planeNormal = plane.normal;
        // 평면의 네 꼭짓점을 계산하기 위한 임의의 두 벡터
        Vector3 v3 = Vector3.Cross(planeNormal, transform.up);
        if (v3 == Vector3.zero)
        {
            v3 = Vector3.Cross(planeNormal, transform.right);
        }

        v3.Normalize();
        v3 *= 5; // 크기 조절

        Vector3 corner0 = planeCenter + v3;
        Vector3 corner1 = planeCenter - v3;
        Vector3 corner2 = planeCenter + Quaternion.AngleAxis(90.0f, planeNormal) * v3;
        Vector3 corner3 = planeCenter - Quaternion.AngleAxis(90.0f, planeNormal) * v3;

        // 평면의 네 꼭짓점을 연결하여 사각형 그리기
        Gizmos.DrawLine(corner0, corner2);
        Gizmos.DrawLine(corner2, corner1);
        Gizmos.DrawLine(corner1, corner3);
        Gizmos.DrawLine(corner3, corner0);
    }
#endif
}
