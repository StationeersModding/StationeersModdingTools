using Assets.Scripts.Objects;
using UnityEditor;
using UnityEngine;

namespace stationeers.modding.tools.visualizers
{
    /// <summary>
    /// Draws the local-space <see cref="Thing.Bounds"/> transformed into the Scene view.
    /// Note: The visualizer expects the Thing.Bounds to be already calculated.
    /// </summary>
    public class ThingBoundsVisualizer : IThingVisualizer
    {
        private const string ColorPrefKey = "Visualizer.ThingBounds.Color";
        private static readonly Color DefaultBoundsColor = new Color(0.2f, 1f, 0.9f, 1f);

        private Color boundsColor = VisualizerPreferencesUtil.LoadColor(ColorPrefKey, DefaultBoundsColor);

        public string ToggleTitle => "Thing Bounds";
        public string ToggleName => "Visualizer.ThingBounds";
        public string ToggleTooltip => "Display the Thing.Bounds box, calculating it from renderers or colliders when missing.";
        public bool ToggleState => true;

        public void OnPreferencesGUI()
        {
            boundsColor = VisualizerPreferencesUtil.ColorField(
                "Bounds Color",
                ColorPrefKey,
                boundsColor,
                DefaultBoundsColor);
        }

        public void OnSceneGUI(SceneView sceneView, Object target)
        {
            if (target is not Thing thing)
                return;

            Bounds bounds = thing.Bounds;


            if (bounds.size == Vector3.zero)
                return;

            Handles.color = boundsColor;
            VisualizerDrawUtil.WithHandlesMatrix(
                thing.transform.localToWorldMatrix,
                () => Handles.DrawWireCube(bounds.center, bounds.size)
            );
        }
    }
}
