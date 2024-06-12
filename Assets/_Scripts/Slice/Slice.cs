
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slice : MonoBehaviour
{

    /// <summary>
    /// 메쉬 정보 struct
    /// </summary>
    public struct VertexData
    {
        public Vector3 Position;
        public Vector2 Uv;
        public Vector3 Normal;

        /// <summary>
        /// Vertex의 면이 어느 방향을 향하고 있는지 파악하는 부울
        /// </summary>
        public bool Side;
    }

    /// <summary>
    /// 오브젝트와 자를 지점의 매쉬 데이터
    /// </summary>
    /// <param name="mesh">자를 오브젝트 매쉬</param>
    /// <param name="plane">오브젝트를 자를 평면</param>
    /// <param name="index">매쉬의 폴리곤 인덱스 번호</param>
    /// <returns></returns>
    static VertexData GetVertexData(Mesh mesh, Plane plane, int index)
    {
        if (index < 0 || index >= mesh.vertexCount)
        {
            throw new IndexOutOfRangeException($"Index {index} is out of bounds for mesh vertices.");
        }

        Vector3 position = mesh.vertices[index];
        if (mesh.uv.Length < mesh.vertexCount)
        {
            mesh.uv = new Vector2[mesh.vertexCount];
        }

        VertexData vertexData = new VertexData()
        {
            Position = position,
            Uv = mesh.uv[index],
            Normal = mesh.normals[index],
            Side = plane.GetSide(position)
        };
        return vertexData;
    }

    /// <summary>
    /// 잘린 오브젝트의 매쉬 정보를 가지고 있는 클래스
    /// </summary>
    class SlicedObjectData
    {
        // 잘린 위치정보
        List<Vector3> vertices;
        List<int> triangles;
        List<Vector2> uvs;
        List<Vector3> normals;

        /// <summary>
        /// 정의
        /// </summary>
        public SlicedObjectData()
        {
            vertices = new List<Vector3>();
            triangles = new List<int>();  
            uvs = new List<Vector2>();
            normals = new List<Vector3>();
        }

        /// <summary>
        /// 잘린 오브젝트 매쉬 정보를 받아오는 함수
        /// </summary>
        /// <returns></returns>
        public Mesh GetSlicedObjectMesh()
        {
            Mesh mesh = new Mesh();
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.uv = uvs.ToArray();
            mesh.normals = normals.ToArray();
            return mesh;
        }

        /// <summary>
        /// 정점 3개를 넣어서 폴리곤 좌표 저장하는 함수
        /// </summary>
        /// <param name="vertexData1"></param>
        /// <param name="vertexData2"></param>
        /// <param name="vertexData3"></param>
        public void AddMeshSection(VertexData vertexData1, VertexData vertexData2, VertexData vertexData3)
        {
            int index1 = TryAddVertex(vertexData1);
            int index2 = TryAddVertex(vertexData2);
            int index3 = TryAddVertex(vertexData3);

            AddTriangle(index1, index2, index3);
        }

        /// <summary>
        /// 잘린 폴리곤 저장
        /// </summary>
        /// <param name="index1"></param>
        /// <param name="index2"></param>
        /// <param name="index3"></param>
        void AddTriangle(int index1, int index2, int index3)
        {
            triangles.Add(index1);
            triangles.Add(index2);
            triangles.Add(index3);
        }

        /// <summary>
        /// 정점,노말,uv 데이터를 리스트에 추가하는 함수
        /// </summary>
        /// <param name="vertexData">잘린 위치</param>
        /// <returns></returns>
        int TryAddVertex(VertexData vertexData)
        {
            vertices.Add(vertexData.Position); 
            uvs.Add(vertexData.Uv);
            normals.Add(vertexData.Normal);
            return vertices.Count -1;
        }


    }

    /// <summary>
    /// plane이 지금 월드 좌표 기준이 아니라 로컬기준으로 계산을 해버려서  잘못된 결과 나옴
    /// </summary>
    /// <param name="meshFilter"></param>
    /// <param name="cutPoint"></param>
    /// <param name="normal"></param>
    /// <returns></returns>
    public Mesh[] Slicer(MeshFilter meshFilter, Plane plane)
    {
        Mesh originMesh = meshFilter.sharedMesh;  // 매쉬 가져오고
        SlicedObjectData positiveMesh = new SlicedObjectData(); // 잘린 오브젝트중 첫번째
        SlicedObjectData negativeMesh = new SlicedObjectData(); // 잘린 오브젝트중 두번째

        // 잘려진 정점들을 저장하는 리스트
        List<VertexData> pointsSlicedPlane = new List<VertexData>();

        // 폴리곤 가져오기
        int[] meshTriangles = originMesh.triangles;

        // 정점별로 계산시작
        for (int i = 0; i < meshTriangles.Length; i+=3)
        {


            VertexData vertexData1 = GetVertexData(originMesh,plane, meshTriangles[i]);
            VertexData vertexData2 = GetVertexData(originMesh,plane, meshTriangles[i+1]);
            VertexData vertexData3 = GetVertexData(originMesh,plane, meshTriangles[i+2]);

            bool is12SameSide = vertexData1.Side == vertexData2.Side;
            bool is23SameSide = vertexData2.Side == vertexData3.Side;

            // 모두 자를 평면과 같은 면에 해당하는 방향이라면 -> 안자르고 그대로 두게할 평면들
            if (is12SameSide && is23SameSide)
            {
                SlicedObjectData slicedObjectData = vertexData1.Side ? positiveMesh : negativeMesh; // 첫번째 슬라이스인지 두번째 슬라이스인지 구하고
                slicedObjectData.AddMeshSection(vertexData1,vertexData2,vertexData3);               // 해당하는 슬라이스에 폴리곤 데이터 저장
            }
            else
            {
                // plane에 잘리게된 면들 계산시작

                // 각 정점들이 positive에 갈지 negative에 갈지 구별
                SlicedObjectData slicedObjectData1 = vertexData1.Side? positiveMesh : negativeMesh;
                SlicedObjectData slicedObjectData2 = vertexData2.Side? positiveMesh : negativeMesh;
                SlicedObjectData slicedObjectData3 = vertexData3.Side? positiveMesh : negativeMesh;

                // 평면으로 잘리게된 지점에 대한 정보를 담는 교차점
                VertexData intersection1;
                VertexData intersection2;

                // 반으로 가른 매쉬중 어느 매쉬로 갈지 결정
                if (is12SameSide)
                {
                    // 교차점 구하기
                    intersection1 = GetIntersectionVertex(vertexData1, vertexData3, plane);
                    intersection2 = GetIntersectionVertex(vertexData2, vertexData3, plane);

                    // 구해진 교차점으로 정점들을 넣어서 하나의 폴리곤으로 만든후 메쉬에 넣기
                    // 정점들 순서대로 입력 반드시 중요함
                    slicedObjectData1.AddMeshSection(vertexData1 ,vertexData2, intersection2);      // positive
                    slicedObjectData1.AddMeshSection(vertexData1, intersection2, intersection1);    // positive
                    slicedObjectData3.AddMeshSection(intersection2, vertexData3, intersection1);     // negative
                }
                else if (is23SameSide)
                {
                    intersection1 = GetIntersectionVertex(vertexData2, vertexData1, plane);
                    intersection2 = GetIntersectionVertex(vertexData3, vertexData1, plane);

                    slicedObjectData1.AddMeshSection(intersection2, vertexData1, intersection1);        // positive
                    slicedObjectData2.AddMeshSection(vertexData2, vertexData3, intersection2);          // negative
                    slicedObjectData2.AddMeshSection(vertexData2, intersection2, intersection1);        // negative
                }
                else
                {
                    // 1,3이 같은 매쉬로 통합된다
                    intersection1 = GetIntersectionVertex(vertexData1, vertexData2, plane);
                    intersection2 = GetIntersectionVertex(vertexData3, vertexData2, plane);

                    slicedObjectData1.AddMeshSection(intersection1, intersection2, vertexData1 );        // negative
                    slicedObjectData2.AddMeshSection(vertexData2, intersection2, intersection1);        // positive
                    slicedObjectData1.AddMeshSection(vertexData1, intersection2,vertexData3);        // negative
                }

                // 잘려진 정점들을 리스트에 저장한다.
                pointsSlicedPlane.Add(intersection1);
                pointsSlicedPlane.Add(intersection2);
            }



        }
        if (pointsSlicedPlane.Count > 0)
        {
            JoinPointsAlongPlane(ref positiveMesh, ref negativeMesh, plane.normal, pointsSlicedPlane);
        }else
        {

            return null;
        }
        // 잘린 두개의 매쉬를 리턴
        return new[] { positiveMesh.GetSlicedObjectMesh(), negativeMesh.GetSlicedObjectMesh() };
    }

    /// <summary>
    /// 잘린 지점과 교차되는 지점을 구하는 함수
    /// </summary>
    /// <param name="from">잘릴 오브젝트의 한 선분중 하나</param>
    /// <param name="to">잘릴 오브젝트의 한 선분중 나머지 하나</param>
    /// <param name="planeOrigin">자르는 평면의 벡터값</param>
    /// <param name="normal">자르는 평면의 노말값</param>
    /// <param name="result">잘린 지점, 교차점 위치</param>
    /// <returns>성공적으로 지점을 구했으면 true 아니면 false</returns>
    public static bool PointIntersectAPlane(Vector3 from , Vector3 to, Plane plane, out Vector3 result)
    {
        Vector3 translation = to - from; // from  -> to 방향벡터
        float dot = Vector3.Dot(plane.normal, translation);   // 노말벡터와의 내적 계산

        // 두 벡터가 수직이 아닐때
        if (Mathf.Abs(dot) > Mathf.Epsilon)
        {
            
            Vector3 pointOnPlane =  plane.ClosestPointOnPlane(from);
            Vector3 fromOrigin = from - pointOnPlane;    // 잘린지점에서 from 방향벡터
            float fac = -Vector3.Dot(plane.normal, fromOrigin) / dot; // 비율 구하기
            translation *= fac;                             // 방향벡터를 구한후 비율을 곱해서 나온 result가 잘린 지점이다
            result = from + translation;
            return true;
        }


        result = Vector3.zero;
        return false;
    }

    /// <summary>
    /// 두 정점 사이의 교차점을 계산해 VertexData를 구하는 함수
    /// </summary>
    /// <param name="vertexData1">정점1</param>
    /// <param name="vertexData2">정점2</param>
    /// <param name="planeOrigin">자를 평면</param>
    /// <param name="normal">자를 평면의 노말값</param>
    /// <returns>교차점이 구해졌다면 VertexData</returns>
    static VertexData GetIntersectionVertex(VertexData vertexData1, VertexData vertexData2, Plane plane)
    {
        /*        PointIntersectAPlane(vertexData1.Position,vertexData2.Position,planeOrigin,normal, out Vector3 result);
                float distance1 = Vector3.Distance(vertexData1.Position, result);
                float distance2 = Vector3.Distance(vertexData2.Position, result);

                float t = distance1 / (distance1 + distance2);

                return new VertexData()
                {
                    Position = result,
                    Normal = normal,
                    Uv = Vector2.Lerp(normal, result, t)    // 주의 https://medium.com/@hesmeron/mesh-slicing-in-unity-740b21ffdf84
                    //https://blog.naver.com/shol9570/222224199117
                };*/
        // 두 정점 사이의 교차점 구하기
        if (PointIntersectAPlane(vertexData1.Position, vertexData2.Position, plane, out Vector3 result))
        {
            // 교차점까지의 거리 계산
            float totalDistance = (vertexData2.Position - vertexData1.Position).sqrMagnitude;   // 총 거리비율
            float distanceToIntersection = (result - vertexData1.Position).sqrMagnitude;        // 교차점 까지의 거리비율
            float t = distanceToIntersection / totalDistance;   // 비율 구하기

            // Lerp로 교차점의 UV, Normal구하기
            Vector2 interpolatedUv = Vector2.Lerp(vertexData1.Uv, vertexData2.Uv, t);
            Vector3 interpolatedNomal = Vector3.Lerp(vertexData1.Normal, vertexData2.Normal, t);

            return new VertexData()
            {
                Position = result,
                Uv = interpolatedUv,
                Normal = interpolatedNomal
            };
        }

        return default(VertexData); 
        
    }

/*    public void AddPolygon(List<VertexData> vertexDataList, Vector3 normal)
    {
        Vector3 center = Vector3.zero;
        foreach (var vertexData in vertexDataList)
        {
            center += vertexData.Position;
        }
        center /= vertexDataList.Count;


    }*/

    /// <summary>
    /// 자른 평면을 매꾸는 함수
    /// </summary>
    /// <param name="positive">쪼개진 사각형중 하나</param>
    /// <param name="negative">쪼개진 사각형중 하나</param>
    /// <param name="cutNormal">자른 평면의 노말값</param>
    /// <param name="vertexDataList">잘린 부분의 정점으로 이뤄진 리스트</param>
    private static void JoinPointsAlongPlane(ref SlicedObjectData positive, ref SlicedObjectData negative, Vector3 cutNormal, List<VertexData> vertexDataList)
    {
        // 리스트의 첫번째 지점과 나머지 지점들을 연결한다
        VertexData halfway = new VertexData()
        {
            Position = vertexDataList[0].Position
        };

        /*
             for (int i = 2; i < vertexDataList.Count - 1; i++)
    {
        VertexData firstVertex = vertexDataList[i];
        VertexData secondVertex = vertexDataList[i + 1];
         */

        // 잘린 지점을 따라 삼각형을 생성
        for (int i = 1; i < vertexDataList.Count; i++)
        {
            VertexData firstVertex = vertexDataList[i];
            VertexData secondVertex = vertexDataList[(i + 1) % vertexDataList.Count];

            // 교차점의 노멀 벡터는 자르는 평면의 노멀 벡터를 사용
            firstVertex.Normal = cutNormal;
            secondVertex.Normal = cutNormal;

            // 삼각형의 정점 순서에 따라 폴리곤 추가
            Vector3 normal = ComputeNormal(halfway, secondVertex, firstVertex);
            float dot = Vector3.Dot(normal, cutNormal);

            if (dot > 0)
            {
                // 평면 노멀과 일치하는 경우
                positive.AddMeshSection(firstVertex, secondVertex, halfway);
                negative.AddMeshSection(secondVertex, firstVertex, halfway);
            }
            else
            {
                // 평면 노멀과 반대인 경우
                negative.AddMeshSection(firstVertex, secondVertex, halfway);
                positive.AddMeshSection(secondVertex, firstVertex, halfway);
            }
        }
    }

    public static Vector3 ComputeNormal(VertexData vertexA, VertexData vertexB, VertexData vertexC)
    {
        Vector3 sideL = vertexB.Position - vertexA.Position;
        Vector3 sideR = vertexC.Position - vertexA.Position;

        Vector3 normal = Vector3.Cross(sideL, sideR);

        return normal.normalized;
    }

}
