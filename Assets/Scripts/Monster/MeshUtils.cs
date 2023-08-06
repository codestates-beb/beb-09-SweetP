using UnityEngine;
using System.Collections.Generic;
public class MeshUtils
{
    public static LivingEntity[] CheckLivingEntitiesInsideMesh(Mesh mesh, Vector3 shapeCenter, Quaternion shapeRotation)
    {
        Vector3[] vertices = mesh.vertices;
        int playerLayerMask = LayerMask.GetMask("Player");
        List<LivingEntity> livingEntitiesInsideMesh = new List<LivingEntity>();

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 worldVertex = shapeRotation * vertices[i] + shapeCenter;
            RaycastHit[] hits = Physics.RaycastAll(shapeCenter, worldVertex - shapeCenter, Vector3.Distance(worldVertex, shapeCenter), playerLayerMask);

            foreach (RaycastHit hit in hits)
            {
                LivingEntity livingEntity = hit.collider.GetComponent<LivingEntity>();
                if (livingEntity != null)
                {
                    livingEntitiesInsideMesh.Add(livingEntity);
                }
            }
        }

        return livingEntitiesInsideMesh.ToArray();
    }
}
