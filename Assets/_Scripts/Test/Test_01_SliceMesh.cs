using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Test_01_SliceMesh : TestBase
{
    public Slice Slicer;
    public MeshFilter Filter;
    public Vector3 point;
    public Vector3 normal;


#if UNITY_EDITOR
/*    public void SliceMesh()
    {
        Mesh[] meshes = Slicer.Slicer(Filter, point - transform.position, normal);
        for (int index = 0; index < meshes.Length; index++)
        {
            Mesh mesh = meshes[index];
            
            GameObject submesh = Instantiate(this.gameObject);
            //submesh.gameObject.transform.position += (2 * transform.right); // 소환 위치
            MeshFilter filter =  submesh.GetComponent<MeshFilter>();   // 메시 적용
            filter.sharedMesh = mesh;

        }
        //gameObject.SetActive(false);
    }*/
    [CustomEditor(typeof(Test_01_SliceMesh))]
    public class MeshSlicerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            Test_01_SliceMesh meshSlicer = target as Test_01_SliceMesh;

            if (GUILayout.Button("Slice mesh"))
            {
                Undo.RecordObject(meshSlicer, "Slice");
                //meshSlicer.SliceMesh();
                EditorUtility.SetDirty(meshSlicer);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        //We construct new gizmos matrix taking our _normal as forward position
        Gizmos.matrix = Matrix4x4.TRS(transform.position, Quaternion.LookRotation(normal), Vector3.one);
        //We draw cubes that will now represent our slicing plane
        Gizmos.color = new Color(0, 1, 0, 0.4f);

        Gizmos.DrawCube((point - transform.position) * transform.localScale.x, new Vector3(2, 2, 0.0001f));
        Gizmos.color = new Color(0, 1, 0, 1f);
        Gizmos.DrawWireCube( (point - transform.position) * transform.localScale.x, new Vector3(2, 2, 0.0001f));
        //We set matrix to our object matrix and draw all of the normals.
        //It will be especially usefull after we start
        //slicing mesh and have to check
        //if all faces where created correctly 

        /*
        for (int i = 0; i < Filter.sharedMesh.normals.Length; i++)
        {
            Vector3 normal = Filter.sharedMesh.normals[i];
            Vector3 vertex = Filter.sharedMesh.vertices[i];
            Gizmos.DrawLine(vertex, vertex + normal);
        }*/
    }
#endif
}
