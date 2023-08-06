using UnityEngine;

public class test : MonoBehaviour
{
   public float attackAngle = 60f;
    public float attackDistance = 5f;
    public float patternTwoDamage = 90f;

    public Material warningMaterial;

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private Mesh WedgeMesh;

    private void Start()
    {
        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshRenderer = gameObject.AddComponent<MeshRenderer>();

        meshRenderer.material = warningMaterial;
        meshRenderer.material.color = new Color(1f, 0f, 0f, 0.5f); // 빨간색과 알파값을 설정

        WedgeMesh = CreateWedgeMesh(attackAngle, attackDistance);
        meshFilter.mesh = WedgeMesh;

        DrawWedgeAroundBoss(attackAngle, attackDistance);
    }

    private Mesh CreateWedgeMesh(float angle, float distance)
    {
        Mesh wedgeMesh = new Mesh();

        int vertexCount = 30;
        Vector3[] vertices = new Vector3[vertexCount + 1];
        int[] triangles = new int[vertexCount * 3];

        // 원점 추가
        vertices[0] = Vector3.zero;

        // 부채꼴의 정점 추가
        for (int i = 0; i < vertexCount; i++)
        {
            float angleStep = angle / vertexCount;
            float currentAngle = -angle / 2f + angleStep * i;
            Vector3 direction = Quaternion.Euler(0f, currentAngle, 0f) * Vector3.forward;
            vertices[i + 1] = direction * distance;
        }

        // 삼각형 인덱스 설정
        for (int i = 0; i < vertexCount; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }

        triangles[vertexCount * 3 - 1] = 1; // 마지막 삼각형 인덱스 수정

        wedgeMesh.vertices = vertices;
        wedgeMesh.triangles = triangles;

        return wedgeMesh;
    }

    private void DrawWedgeAroundBoss(float angle, float distance)
    {
        transform.rotation = Quaternion.identity; // 보스의 회전을 초기화
        transform.position = GetPlayerPosition(); // 보스의 위치를 플레이어 위치로 설정

        // 원하는 위치와 회전으로 부채꼴을 배치
        Vector3 rotation = new Vector3(0f, GetPlayerRotation(), 0f);
        transform.Rotate(rotation);
        transform.Translate(Vector3.forward * distance);
    }

   
    private Vector3 GetPlayerPosition()
    {
        return GameObject.FindGameObjectWithTag("Player").transform.position;
    }

    private float GetPlayerRotation()
    {
        return GameObject.FindGameObjectWithTag("Player").transform.eulerAngles.y;
    }
}