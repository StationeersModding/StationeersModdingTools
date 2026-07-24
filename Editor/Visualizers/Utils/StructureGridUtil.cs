using System.Collections.Generic;
using Assets.Scripts.Objects;
using UnityEngine;

namespace stationeers.modding.tools.visualizers
{
    /// <summary>
    /// Shared bounds, grid and cell calculations used by all grid visualizers.
    /// </summary>
    internal static class StructureGridUtil
    {
        internal static void CachePrefabBounds(Structure structure)
        {
            structure.ThingTransform = structure.transform;

            Quaternion originalRotation = structure.ThingTransform.rotation;
            Vector3 originalPosition = structure.ThingTransform.position;

            try
            {
                structure.ThingTransform.rotation = Quaternion.identity;
                structure.ThingTransformPosition = Vector3.zero;
                structure.Bounds = new Bounds(Vector3.zero, Vector3.zero);

                foreach (Renderer renderer in structure.GetComponentsInChildren<Renderer>())
                    structure.Bounds.Encapsulate(renderer.bounds);
            }
            finally
            {
                structure.ThingTransform.SetPositionAndRotation(originalPosition, originalRotation);
            }

            Vector3 size = structure.Bounds.size;
            structure.SurfaceArea = 2f * (size.x * size.y + size.y * size.z + size.z * size.x)
                                    * structure.SurfaceAreaScale;
        }

        /// <summary>
        /// Returns the grid cells covered by the structure bounds, capped at <paramref name="maxCells"/>.
        /// </summary>
        internal static IReadOnlyList<Vector3Int> GetCells(Structure structure, float cellSize, int maxCells)
        {
            Bounds bounds = structure.Bounds;
            bounds.Expand(structure.BoundsExpand);

            Vector3 worldMin = bounds.min * structure.BoundsGridRatio;
            worldMin.y += structure.BoundsGridAddBottom;
            worldMin.x += worldMin.x * structure.BoundsGridExtraWidth + structure.BoundsGridShiftSide;
            worldMin.z += worldMin.z * structure.BoundsGridExtraForward + structure.BoundsGridShiftForward;

            Vector3 worldMax = bounds.max * structure.BoundsGridRatio;
            worldMax.y += structure.BoundsGridAddHeight;
            worldMax.y += worldMax.y * structure.BoundsGridExtraHeight;
            worldMax.x += worldMax.x * structure.BoundsGridExtraWidth;
            worldMax.z += worldMax.z * structure.BoundsGridExtraForward;
            worldMax.z += worldMax.z * structure.BoundsForward;
            worldMax.z += structure.BoundsGridShiftForward;
            worldMax.x += structure.BoundsGridShiftSide;

            worldMin += structure.transform.position;
            worldMax += structure.transform.position;

            Vector3Int gridMin = WorldToGridPosition(worldMin, cellSize);
            Vector3Int gridMax = WorldToGridPosition(worldMax, cellSize);

            int minX = Mathf.Min(gridMin.x, gridMax.x);
            int minY = Mathf.Min(gridMin.y, gridMax.y);
            int minZ = Mathf.Min(gridMin.z, gridMax.z);
            int maxX = Mathf.Max(gridMin.x, gridMax.x);
            int maxY = Mathf.Max(gridMin.y, gridMax.y);
            int maxZ = Mathf.Max(gridMin.z, gridMax.z);

            int capacity = Mathf.Min(maxCells, (maxX - minX + 1) * (maxY - minY + 1) * (maxZ - minZ + 1));
            var cells = new List<Vector3Int>(capacity);

            for (int x = minX; x <= maxX; x++)
            for (int y = minY; y <= maxY; y++)
            for (int z = minZ; z <= maxZ; z++)
            {
                if (cells.Count >= maxCells)
                    return cells;

                cells.Add(new Vector3Int(x, y, z));
            }

            return cells;
        }

        /// <summary>
        /// Converts a world-space position to the nearest grid coordinate.
        /// </summary>

        internal static Vector3Int WorldToGridPosition(Vector3 worldPosition, float cellSize)
        {
            float half = cellSize * 0.5f;
            return new Vector3Int(
                Mathf.FloorToInt((worldPosition.x + half) / cellSize),
                Mathf.FloorToInt((worldPosition.y + half) / cellSize),
                Mathf.FloorToInt((worldPosition.z + half) / cellSize));
        }

        /// <summary>
        /// Converts a grid coordinate to its world-space grid point.
        /// </summary>
        internal static Vector3 GridToWorldPosition(Vector3Int gridPosition, float cellSize)
        {
            return new Vector3(gridPosition.x * cellSize, gridPosition.y * cellSize, gridPosition.z * cellSize);
        }
    }
}
