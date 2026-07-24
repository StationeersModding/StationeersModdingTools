using System.Collections.Generic;
using Assets.Scripts.Objects;
using UnityEditor;
using UnityEngine;

namespace stationeers.modding.tools.visualizers
{
    /// <summary>
    /// Highlights grid cells occupied by a <see cref="Structure"/>'s shaped bounds using the Thing's Grid cell metrics.
    /// </summary>
    public class GridBlockingVisualizer : IThingVisualizer
    {
        private const int MaxCellsToDraw = 250;
        private const string CellColorPrefKey = "Visualizer.GridBounds.Cell";
        private static readonly Color DefaultCellColor = new Color(1f, 0f, 0f, 0.5f);
        private Color cellColor = VisualizerPreferencesUtil.LoadColor(CellColorPrefKey, DefaultCellColor);

        public string ToggleTitle => "Grid Bounds Auto";
        public string ToggleName => "Visualizer.GridBounds";
        public string ToggleTooltip => "Highlight blocked cells using the structure grid size.";
        public bool ToggleState => true;

        public void OnPreferencesGUI()
        {
            cellColor = VisualizerPreferencesUtil.ColorField("Blocked Cell Color", CellColorPrefKey, cellColor, DefaultCellColor);
        }

        public void OnSceneGUI(SceneView sceneView, Object target)
        {
            if (target is not Structure structure)
                return;

            float cellSize = structure.GridSize;
            if (cellSize <= 0f)
                return;

            if (structure.Bounds.size == Vector3.zero)
                CachePrefabBounds(structure);

            IReadOnlyList<Vector3Int> cells = StructureGridUtil.GetCells(structure, cellSize, MaxCellsToDraw);
            Color wire = cellColor;
            Color fill = cellColor;
            fill.a *= 0.1f;
            Color outline = cellColor;
            outline.a = 1f;

            foreach (Vector3Int gridIndex in cells)
            {
                Vector3 worldPosition = GridToWorldPosition(gridIndex, cellSize);
                Handles.color = wire;
                Handles.DrawWireCube(worldPosition, Vector3.one * cellSize);
                VisualizerDrawUtil.DrawSolidCube(worldPosition, cellSize, fill, outline);
            }
        }

        /// <summary>
        /// Recalculates renderer bounds and placement surface area for a structure.
        /// </summary>
        public static void CachePrefabBounds(Structure structure) => StructureGridUtil.CachePrefabBounds(structure);

        /// <summary>
        /// Converts a world position to the nearest grid coordinate.
        /// </summary>
        public static Vector3Int WorldToGridPosition(Vector3 worldPosition, float cellSize) =>
            StructureGridUtil.WorldToGridPosition(worldPosition, cellSize);

        /// <summary>
        /// Converts a grid coordinate to its world-space grid point.
        /// </summary>
        public static Vector3 GridToWorldPosition(Vector3Int gridPosition, float cellSize) =>
            StructureGridUtil.GridToWorldPosition(gridPosition, cellSize);
    }
}
