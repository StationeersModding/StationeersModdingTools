using Assets.Scripts.Objects;
using UnityEditor;
using UnityEngine;

namespace stationeers.modding.tools.visualizers
{
    /// <summary>
    /// Draws interactable bounds and action labels for every interactable on a <see cref="Thing"/>.
    /// </summary>
    public class ThingInteractablesVisualizer : IThingVisualizer
    {
        private const string ColorPrefKey = "Visualizer.Interactables.Color";
        private static readonly Color DefaultColor = new Color(1f, 0.5f, 0.9f, 1f);
        private Color interactableColor = VisualizerPreferencesUtil.LoadColor(ColorPrefKey, DefaultColor);

        public string ToggleTitle => "Interactables";
        public string ToggleName => "Visualizer.Interactables";
        public string ToggleTooltip => "Display interactable bounds and metadata.";
        public bool ToggleState => true;

        public void OnPreferencesGUI()
        {
            interactableColor = VisualizerPreferencesUtil.ColorField("Interactable Color", ColorPrefKey, interactableColor, DefaultColor);
        }

        public void OnSceneGUI(SceneView sceneView, Object target)
        {
            if (target is not Thing thing)
                return;

            foreach (Interactable interactable in thing.Interactables)
            {
                Vector3 position = interactable.Bounds.center;
                Vector3 size = interactable.Bounds.size;
                Quaternion? rotation = null;

                if (interactable.Collider != null)
                {
                    position = interactable.Collider.bounds.center;
                    rotation = interactable.Collider.transform.rotation;
                    if (size == Vector3.zero)
                        size = interactable.Collider.bounds.size;
                }
                else
                {
                    Transform slotTransform = FindSlotTransform(thing, interactable.Action);
                    if (slotTransform != null)
                    {
                        position += slotTransform.position;
                        rotation = slotTransform.rotation;
                    }
                }

                if (size == Vector3.zero)
                    continue;

                Handles.color = interactableColor;
                if (rotation.HasValue)
                {
                    VisualizerDrawUtil.WithHandlesMatrix(
                        Matrix4x4.TRS(position, rotation.Value, Vector3.one),
                        () => Handles.DrawWireCube(Vector3.zero, size));
                }
                else
                {
                    Handles.DrawWireCube(position, size);
                }

                VisualizerDrawUtil.DrawCenteredLabel(position,
                    $"<color=#FFFFFF><b>{interactable.StringKey}</b></color>\n{interactable.Action}");
            }
        }

        private static Transform FindSlotTransform(Thing thing, InteractableType interactableType)
        {
            foreach (Slot slot in thing.Slots)
            {
                if (slot.Action == interactableType)
                    return slot.Location;
            }

            return null;
        }
    }
}
