using System.IO;
using UnityEditor;
using UnityEngine;

namespace ilodev.stationeersmods.tools.assetsfactory
{
    public static class MeshAssetUtils
    {
        public static Mesh GetOrCreateMeshFromBoxCollider(
            BoxCollider boxCollider,
            string folderPath,
            string meshName)
        {
            if (boxCollider == null)
            {
                Debug.LogError("Cannot create mesh: BoxCollider is null.");
                return null;
            }

            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                Directory.CreateDirectory(folderPath);
                AssetDatabase.Refresh();
            }

            string meshPath = Path.Combine(folderPath, meshName + ".asset")
                .Replace("\\", "/");

            Mesh existingMesh = AssetDatabase.LoadAssetAtPath<Mesh>(meshPath);

            if (existingMesh != null)
            {
                return existingMesh;
            }

            Mesh mesh = CreateMeshFromBoxCollider(boxCollider);
            mesh.name = meshName;

            AssetDatabase.CreateAsset(mesh, meshPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return mesh;
        }

        private static Mesh CreateMeshFromBoxCollider(BoxCollider boxCollider)
        {
            Vector3 center = boxCollider.center;
            Vector3 size = boxCollider.size;

            Vector3 half = size * 0.5f;

            Vector3 p0 = center + new Vector3(-half.x, -half.y, -half.z);
            Vector3 p1 = center + new Vector3(half.x, -half.y, -half.z);
            Vector3 p2 = center + new Vector3(half.x, -half.y, half.z);
            Vector3 p3 = center + new Vector3(-half.x, -half.y, half.z);

            Vector3 p4 = center + new Vector3(-half.x, half.y, -half.z);
            Vector3 p5 = center + new Vector3(half.x, half.y, -half.z);
            Vector3 p6 = center + new Vector3(half.x, half.y, half.z);
            Vector3 p7 = center + new Vector3(-half.x, half.y, half.z);

            Vector3[] vertices =
            {
            p0, p1, p2, p3, // bottom
            p7, p6, p5, p4, // top
            p4, p5, p1, p0, // back
            p5, p6, p2, p1, // right
            p6, p7, p3, p2, // front
            p7, p4, p0, p3  // left
        };

            int[] triangles =
            {
            0, 1, 2, 0, 2, 3,
            4, 5, 6, 4, 6, 7,
            8, 9, 10, 8, 10, 11,
            12, 13, 14, 12, 14, 15,
            16, 17, 18, 16, 18, 19,
            20, 21, 22, 20, 22, 23
        };

            Mesh mesh = new Mesh
            {
                vertices = vertices,
                triangles = triangles
            };

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }
    }
}
