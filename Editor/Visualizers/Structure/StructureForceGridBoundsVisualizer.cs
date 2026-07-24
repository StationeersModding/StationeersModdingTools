using Assets.Scripts.GridSystem;
using Assets.Scripts.Objects;
using UnityEditor;
using UnityEngine;

namespace stationeers.modding.tools.visualizers
{
    /// <summary>
    /// Displays the grid cells explicitly blocked by
    /// <see cref="Structure.ForceGridBounds"/>.
    /// </summary>
    public class StructureForceGridBoundsVisualizer : IThingVisualizer
    {
        private const string FaceColorPrefKey =
            "Visualizer.ForceGridBounds.FaceColor";

        private const string LineColorPrefKey =
            "Visualizer.ForceGridBounds.LineColor";

        private static readonly Color DefaultFaceColor =
            new Color(1f, 1f, 1f, 0.05f);

        private static readonly Color DefaultLineColor = Color.white;

        private Color faceColor = VisualizerPreferencesUtil.LoadColor(FaceColorPrefKey, DefaultFaceColor);

        private Color lineColor = VisualizerPreferencesUtil.LoadColor(LineColorPrefKey, DefaultLineColor);

        public string ToggleTitle => "Grid Bounds Forced";

        // Retains the preference key used by the previous implementation.
        public string ToggleName => "Visualizer.ForceGridBounds";

        public string ToggleTooltip =>
            "Display the grid cells explicitly blocked by Structure.ForceGridBounds.";

        public bool ToggleState => true;

        /// <summary>
        /// Draws settings specific to force-grid bounds.
        /// </summary>
        public void OnPreferencesGUI()
        {
            faceColor = VisualizerPreferencesUtil.ColorField(
                "Cell Face Color",
                FaceColorPrefKey,
                faceColor,
                DefaultFaceColor);

            lineColor = VisualizerPreferencesUtil.ColorField(
                "Cell Outline Color",
                LineColorPrefKey,
                lineColor,
                DefaultLineColor);
        }

        /// <summary>
        /// Draws each cell listed in <see cref="Structure.ForceGridBounds"/>.
        /// </summary>
        public void OnSceneGUI(SceneView sceneView, Object target)
        {
            if (target is not Structure structure || structure is SmallGrid)
                return;

            if (structure.GridSize <= 0f || structure.ForceGridBounds == null)
                return;

            VisualizerDrawUtil.WithHandlesMatrix(
                structure.transform.localToWorldMatrix,
                () =>
                {
                    foreach (Grid3 gridCell in structure.ForceGridBounds)
                    {
                        Vector3 localCenter =
                            gridCell.ToVector3() * structure.GridSize;

                        VisualizerDrawUtil.DrawSolidCube(
                            localCenter,
                            structure.GridSize,
                            faceColor,
                            lineColor);
                    }
                });
        }
    }
}