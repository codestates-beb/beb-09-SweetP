using UnityEngine;

public class MeshReadEnableSetter : MonoBehaviour
{
    private void Awake()
    {
        // MeshFilter 컴포넌트를 가져옵니다.
        MeshFilter meshFilter = GetComponent<MeshFilter>();

        // Mesh의 Read/Write Enabled 속성을 true로 설정합니다.
        Mesh mesh = meshFilter.mesh;
        mesh.MarkDynamic(); // Mesh를 동적으로 변경하도록 표시합니다.
        mesh.UploadMeshData(false); // Mesh 데이터를 업로드합니다.
    }
}