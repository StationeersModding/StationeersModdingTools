using Assets.Scripts.Objects;
using UnityEditor;
using UnityEngine;

namespace stationeers.modding.tools.visualizers
{
    /// <summary>
    /// Highlights the snapped small-grid cell occupied by each configured OpenEnd.
    /// </summary>
    public class SmallGridConnectionsVisualizer : IThingVisualizer
    {
        private const string ColorPrefKey = "Visualizer.OpenEnds.ConnectionColor";
        private const float CellSize = 0.5f;
        private static readonly Color DefaultColor = new Color(1f, 0.5f, 0f, 1f);
        private Color connectionColor = VisualizerPreferencesUtil.LoadColor(ColorPrefKey, DefaultColor);

        public string ToggleTitle => "Open End Cells";
        public string ToggleName => "Visualizer.OpenEndCells";
        public string ToggleTooltip => "Highlight SmallGrid cells used by OpenEnd connections.";
        public bool ToggleState => true;

        public void OnPreferencesGUI()
        {
            connectionColor = VisualizerPreferencesUtil.ColorField("OpenEnd Grid Cell Color", ColorPrefKey, connectionColor, DefaultColor);
        }

        public void OnSceneGUI(SceneView sceneView, Object target)
        {
            if (target is not SmallGrid smallGrid)
                return;

            Handles.color = connectionColor;
            foreach (var openEnd in smallGrid.OpenEnds)
            {
                if (openEnd?.Transform == null)
                    continue;

                Vector3 position = VisualizerDrawUtil.SnapToGrid(openEnd.Transform.position, CellSize);
                Handles.DrawWireCube(position, Vector3.one * CellSize);
            }
        }
    }
}
