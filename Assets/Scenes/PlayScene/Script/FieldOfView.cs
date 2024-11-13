using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))] 
public class FieldOfView : MonoBehaviour
{
    public float viewRadius = 5f;  // 視界の半径
    public float viewAngle = 90f;  // 視界の角度
    public int rayCount = 50;      // レイの数（角度の分割数）
    public LayerMask obstacleMask; // 壁のレイヤーマスク
    private Mesh viewMesh;
    private MeshFilter viewMeshFilter;
    void Start()    {
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter = GetComponent<MeshFilter>();
        viewMeshFilter.mesh = viewMesh;
    }     
    void LateUpdate()    {       
        DrawFieldOfView();    
    }    
    void DrawFieldOfView()    {
        float angleStep = viewAngle / rayCount; 
        List<Vector3> viewPoints = new List<Vector3>(); 
        for (int i = 0; i <= rayCount; i++)         {  
            float angle = transform.eulerAngles.y - viewAngle / 2 + angleStep * i;   
            viewPoints.Add(CastRay(angle));     
        }       
        int vertexCount = viewPoints.Count + 1;    
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];
        vertices[0] = Vector3.zero;
        for (int i = 0; i < viewPoints.Count; i++)         {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);
            if (i < viewPoints.Count - 1) { triangles[i * 3] = 0; triangles[i * 3 + 1] = i + 1; 
                triangles[i * 3 + 2] = i + 2; } } viewMesh.Clear(); 
        viewMesh.vertices = vertices; viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals(); 
    } 
    Vector3 CastRay(float angle) { 
        Vector3 direction = DirFromAngle(angle, true);
        RaycastHit hit; if (Physics.Raycast(transform.position, direction, out hit, viewRadius, obstacleMask)) 
        { 
            return hit.point; 
        } else { 
            return transform.position + direction * viewRadius;
        }
    }
    Vector3 DirFromAngle(float angleInDegrees, bool isGlobal) {
        if (!isGlobal) {
            angleInDegrees += transform.eulerAngles.y; 
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad)); 
    } 
}

