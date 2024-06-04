
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
        Vector3 position = mesh.vertices[index];
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

    public Mesh[] Slicer(MeshFilter meshFilter, Vector3 cutPoint, Vector3 nomal)
    {
        Mesh originMesh = meshFilter.mesh;

        Plane plane = new Plane(nomal, cutPoint);   // 오브젝트를 자를 평면
        SlicedObjectData positiveMesh = new SlicedObjectData(); // 잘린 오브젝트중 첫번째
        SlicedObjectData negativeMesh = new SlicedObjectData(); // 잘린 오브젝트중 두번째

        Vector3[] originVertices = originMesh.vertices;
        Vector3[] originNormals = originMesh.normals;
        Vector2[] originUVs = originMesh.uv;

        // 폴리곤 가져오기
        int[] meshTriangles = originMesh.triangles;

        for (int i = 0; i < meshTriangles.Length; i+=3)
        {
            VertexData vertexData1 = GetVertexData(originMesh,plane, meshTriangles[i]);
            VertexData vertexData2 = GetVertexData(originMesh,plane, meshTriangles[i+1]);
            VertexData vertexData3 = GetVertexData(originMesh,plane, meshTriangles[i+2]);

            bool is12SameSide = vertexData1.Side == vertexData2.Side;
            bool is23SameSide = vertexData2.Side == vertexData3.Side;

            // 모두 같은 면에 해당하는 방향이라면
            if (is12SameSide && is23SameSide)
            {
                SlicedObjectData slicedObjectData = vertexData1.Side ? positiveMesh : negativeMesh; // 첫번째 슬라이스인지 두번째 슬라이스인지 구하고
                slicedObjectData.AddMeshSection(vertexData1,vertexData2,vertexData3);               // 해당하는 슬라이스에 폴리곤 데이터 저장
            }
        }

        // 잘린 두개의 매쉬를 리턴
        return new[] { positiveMesh.GetSlicedObjectMesh(), negativeMesh.GetSlicedObjectMesh() };
    }

    /// <summary>
    /// 잘린 지점과 교차되는 지점을 구하는 함수
    /// </summary>
    /// <param name="from">잘릴 오브젝트의 한 선분중 하나</param>
    /// <param name="to">잘릴 오브젝트의 한 선분중 나머지 하나</param>
    /// <param name="planeOrigin">자르는 벡터값</param>
    /// <param name="normal">자르는 평면의 노말값</param>
    /// <param name="result">잘린 지점의 벡터값</param>
    /// <returns>성공적으로 지점을 구했으면 true 아니면 false</returns>
    public static bool PointIntersectAPlane(Vector3 from , Vector3 to, Vector3 planeOrigin, Vector3 normal, out Vector3 result)
    {
        Vector3 translation = to - from; // from  -> to 방향벡터
        float dot = Vector3.Dot(normal, translation);   // 노말벡터와의 내적 계산

        // 두 벡터가 수직이 아닐때
        if (Mathf.Abs(dot) > Mathf.Epsilon)
        {
            Vector3 fromOrigin = from - planeOrigin;    // 잘린지점에서 from 방향벡터
            float fac = -Vector3.Dot(normal, fromOrigin) / dot; // 비율 구하기
            translation *= fac;                             // 방향벡터를 구한후 비율을 곱해서 나온 result가 잘린 지점이다
            result = from + translation;
            return true;
        }

        result = Vector3.zero;
        return false;
    }

    static VertexData GetIntersectionVertex(VertexData vertexData1, VertexData vertexData2, Vector3 planeOrigin, Vector3 normal)
    {
        PointIntersectAPlane(vertexData1.Position,vertexData2.Position,planeOrigin,normal, out Vector3 result);
        float distance1 = Vector3.Distance(vertexData1.Position, result);
        float distance2 = Vector3.Distance(vertexData2.Position, result);

        float t = distance1 / (distance1 + distance2);

        return new VertexData()
        {
            Position = result,
            Normal = normal,
            Uv = Vector2.Lerp(normal, result, t)    // 주의 https://medium.com/@hesmeron/mesh-slicing-in-unity-740b21ffdf84
            //https://blog.naver.com/shol9570/222224199117
        };
    }
    
}
