using Assets.Scripts.Objects;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace StationeersModding.tools.visualizers
{
  public class SmallGridBlockingVisualizer : IThingVisualizer
  {
    public float GridSize = 0.5f;

    public void OnSceneGUI(SceneView sceneView, Object target)
    {
      if (!EditorPrefs.GetBool("Visualizer.SmallGridBounds", true))
        return;

      Structure structure = target as Structure;
      if (structure == null)
        return;

      Handles.BeginGUI();
      GUILayout.BeginArea(new Rect(10, sceneView.position.height - 100, 200, 50));
      if (GUILayout.Button("Auto Calculate Grid Bounds"))
      {
        AutoCalculateGridParameters(structure);
      }
      GUILayout.EndArea();
      Handles.EndGUI();


      if (structure.Bounds.size == Vector3.zero)
        CachePrefabBounds(structure);

      List<Vector3> gridCenters = GetAccurateSmallGridCells(structure);

      foreach (Vector3 localPos in gridCenters)
      {

        Vector3 worldPos = structure.transform.TransformPoint(localPos);

        Handles.color = new Color(1.0f, 0f, 0f, 0.5f);

        Handles.DrawWireCube(worldPos, Vector3.one * GridSize);

        Color fill = new Color(1f, 0f, 0f, 0.1f);

        Color outline = Color.red;

        Matrix4x4 rotationMatrix = Matrix4x4.TRS(worldPos, structure.transform.rotation, Vector3.one);
        Matrix4x4 oldMatrix = Handles.matrix;
        Handles.matrix = rotationMatrix;

        DrawSolidCube(Vector3.zero, GridSize, fill, outline);

        Handles.matrix = oldMatrix;
      }
    }

    // <summary>

    // Solves the Structure grid parameters to fit the Renderer bounds.

    // </summary>

    public void AutoCalculateGridParameters(Structure structure)
    {
      Undo.RecordObject(structure, "Auto Calculate Grid Bounds");

      Bounds renderBounds = new Bounds(Vector3.zero, Vector3.zero);
      bool hasBounds = false;

      Vector3 originalPos = structure.transform.position;
      Quaternion originalRot = structure.transform.rotation;
      Vector3 originalScale = structure.transform.localScale;

      structure.transform.position = Vector3.zero;
      structure.transform.rotation = Quaternion.identity;
      structure.transform.localScale = Vector3.one;

      foreach (Renderer r in structure.GetComponentsInChildren<Renderer>())
      {
        if (r is ParticleSystemRenderer || r is TrailRenderer) continue;
        if (!hasBounds)
        {
          renderBounds = r.bounds;
          hasBounds = true;
        }
        else
        {
          renderBounds.Encapsulate(r.bounds);
        }
      }

      structure.transform.position = originalPos;
      structure.transform.rotation = originalRot;
      structure.transform.localScale = originalScale;

      if (!hasBounds) return;

      if (structure.Bounds.size == Vector3.zero)
      {
        structure.Bounds = renderBounds;
      }

      Bounds source = structure.Bounds;
      Bounds target = renderBounds;

      structure.BoundsGridRatio = 1f;
      structure.BoundsGridAddBottom = 0f;
      structure.BoundsGridAddHeight = 0f;
      structure.BoundsGridExtraWidth = 0f;
      structure.BoundsGridExtraForward = 0f;
      structure.BoundsGridShiftForward = 0f;
      structure.BoundsForward = 0f;
      structure.BoundsGridExtraHeight = 0f;

      structure.BoundsGridAddBottom = target.min.y - source.min.y;

      structure.BoundsGridAddHeight = target.max.y - source.max.y;

      float sizeZ_Source = source.size.z;
      float sizeZ_Target = target.size.z;

      if (sizeZ_Source > 0.001f)
      {
        structure.BoundsGridExtraForward = (sizeZ_Target / sizeZ_Source) - 1f;
      }

      float expectedMinZ = source.min.z * (1f + structure.BoundsGridExtraForward);
      structure.BoundsGridShiftForward = target.min.z - expectedMinZ;

      float maxExtentsSource = Mathf.Max(Mathf.Abs(source.min.x), Mathf.Abs(source.max.x));
      float maxExtentsTarget = Mathf.Max(Mathf.Abs(target.min.x), Mathf.Abs(target.max.x));

      if (maxExtentsSource > 0.001f)
      {
        structure.BoundsGridExtraWidth = (maxExtentsTarget / maxExtentsSource) - 1f;
      }

      EditorUtility.SetDirty(structure);
      Debug.Log("Auto-Calculated Grid Bounds.");
    }

    // <summary>

    // Replicates the exact math from Structure.GetLocalSmallGridBounds()

    // </summary>

    private List<Vector3> GetAccurateSmallGridCells(Structure structure)
    {
      List<Vector3> cells = new List<Vector3>();
      Bounds bounds = structure.Bounds;

      bounds.Expand(structure.BoundsExpand);

      Vector3 worldMin = bounds.min * structure.BoundsGridRatio;
      worldMin.y += structure.BoundsGridAddBottom;
      worldMin.x += worldMin.x * structure.BoundsGridExtraWidth;
      worldMin.z += worldMin.z * structure.BoundsGridExtraForward + structure.BoundsGridShiftForward;

      Vector3 worldMax = bounds.max * structure.BoundsGridRatio;
      worldMax.y += structure.BoundsGridAddHeight;
      worldMax.y += worldMax.y * structure.BoundsGridExtraHeight;
      worldMax.x += worldMax.x * structure.BoundsGridExtraWidth;

      worldMax.z += worldMax.z * structure.BoundsGridExtraForward + worldMax.z * structure.BoundsForward + structure.BoundsGridShiftForward;

      Vector3Int minGrid = WorldToGridPosition(worldMin, GridSize);
      Vector3Int maxGrid = WorldToGridPosition(worldMax, GridSize);

      int countX = Mathf.Abs(maxGrid.x - minGrid.x);
      int countY = Mathf.Abs(maxGrid.y - minGrid.y);
      int countZ = Mathf.Abs(maxGrid.z - minGrid.z);

      if (countX * countY * countZ > 5000) return cells;

      for (int x = 0; x <= countX; x++)
      {
        for (int y = 0; y <= countY; y++)
        {
          for (int z = 0; z <= countZ; z++)
          {

            Vector3 offset = new Vector3(x * GridSize, y * GridSize, z * GridSize);

            Vector3 basePos = GridToWorldPosition(minGrid, GridSize);

            cells.Add(basePos + offset);
          }
        }
      }
      return cells;
    }

    public static void DrawSolidCube(Vector3 center, float size, Color faceColor, Color outlineColor)
    {
      Vector3 halfSize = Vector3.one * (size * 0.5f);
      Vector3[] corners = new Vector3[8];
      corners[0] = center + new Vector3(-halfSize.x, -halfSize.y, -halfSize.z);
      corners[1] = center + new Vector3(halfSize.x, -halfSize.y, -halfSize.z);
      corners[2] = center + new Vector3(halfSize.x, -halfSize.y, halfSize.z);
      corners[3] = center + new Vector3(-halfSize.x, -halfSize.y, halfSize.z);
      corners[4] = center + new Vector3(-halfSize.x, halfSize.y, -halfSize.z);
      corners[5] = center + new Vector3(halfSize.x, halfSize.y, -halfSize.z);
      corners[6] = center + new Vector3(halfSize.x, halfSize.y, halfSize.z);
      corners[7] = center + new Vector3(-halfSize.x, halfSize.y, halfSize.z);

      int[][] faces = new int[][]
      {
                new int[]{0, 1, 2, 3}, new int[]{7, 6, 5, 4},
                new int[]{4, 5, 1, 0}, new int[]{6, 7, 3, 2},
                new int[]{5, 6, 2, 1}, new int[]{7, 4, 0, 3},
      };

      foreach (var face in faces)
      {
        Handles.DrawSolidRectangleWithOutline(
            new Vector3[] { corners[face[0]], corners[face[1]], corners[face[2]], corners[face[3]] },
            faceColor, outlineColor
        );
      }
    }

    public static void CachePrefabBounds(Structure structure)
    {
      structure.ThingTransform = structure.transform;
      Quaternion rotation = structure.ThingTransform.rotation;
      Vector3 position = structure.ThingTransform.position;
      structure.ThingTransform.rotation = Quaternion.identity;
      structure.ThingTransformPosition = Vector3.zero;
      structure.Bounds = new Bounds(Vector3.zero, Vector3.zero);

      bool hasBounds = false;
      foreach (Renderer renderer in structure.GetComponentsInChildren<Renderer>())
      {
        if (renderer is ParticleSystemRenderer || renderer is TrailRenderer) continue;

        if (!hasBounds)
        {
          structure.Bounds = renderer.bounds;
          hasBounds = true;
        }
        else
        {
          structure.Bounds.Encapsulate(renderer.bounds);
        }
      }

      structure.Bounds.center -= position;

      structure.ThingTransform.SetPositionAndRotation(position, rotation);
    }

    public static Vector3Int WorldToGridPosition(Vector3 worldPosition, float cellSize)
    {
      return new Vector3Int(
          Mathf.FloorToInt((worldPosition.x + cellSize / 2f) / cellSize),
          Mathf.FloorToInt((worldPosition.y + cellSize / 2f) / cellSize),
          Mathf.FloorToInt((worldPosition.z + cellSize / 2f) / cellSize)
      );
    }

    public static Vector3 GridToWorldPosition(Vector3Int gridPosition, float cellSize)
    {
      return new Vector3(
          gridPosition.x * cellSize,
          gridPosition.y * cellSize,
          gridPosition.z * cellSize
      );
    }
  }
}