using Assets.Scripts.Objects;
using UnityEditor;
using UnityEngine;

namespace stationeers.modding.tools.visualizers
{
    /// <summary>
    /// Draws slot bounds and metadata labels for every slot on a <see cref="Thing"/>.
    /// </summary>
    public class ThingSlotsVisualizer : IThingVisualizer
    {
        private const string ColorPrefKey = "Visualizer.Slots.Color";
        private static readonly Color DefaultSlotColor = new Color(0.8f, 0.8f, 0.3f, 1f);
        private Color slotColor = VisualizerPreferencesUtil.LoadColor(ColorPrefKey, DefaultSlotColor);

        public string ToggleTitle => "Slots";
        public string ToggleName => "Visualizer.Slots";
        public string ToggleTooltip => "Display slot bounds and metadata.";
        public bool ToggleState => true;

        public void OnPreferencesGUI()
        {
            slotColor = VisualizerPreferencesUtil.ColorField("Slot Color", ColorPrefKey, slotColor, DefaultSlotColor);
        }

        public void OnSceneGUI(SceneView sceneView, Object target)
        {
            if (target is not Thing thing)
                return;

            foreach (Slot slot in thing.Slots)
            {
                Vector3 position = slot.Location != null ? slot.Location.position : Vector3.zero;
                Vector3 size = slot.Size;

                if (slot.Collider != null)
                {
                    position = slot.Collider.bounds.center;
                    if (size == Vector3.zero)
                        size = slot.Collider.bounds.size;
                }

                if (size == Vector3.zero)
                    continue;

                Handles.color = slotColor;
                if (slot.Location != null)
                {
                    VisualizerDrawUtil.WithHandlesMatrix(
                        Matrix4x4.TRS(position, slot.Location.rotation, Vector3.one),
                        () => Handles.DrawWireCube(Vector3.zero, size));
                }
                else
                {
                    Handles.DrawWireCube(position, size);
                }

                VisualizerDrawUtil.DrawCenteredLabel(position,
                    $"<color=#FFFFFF><b>{slot.StringKey} ({slot.Type})</b></color>\n" +
                    $"<color=#000000><b>{slot.Action}</b></color>");
            }
        }
    }
}
