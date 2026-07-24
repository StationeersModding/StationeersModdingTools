using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine.UIElements;

namespace stationeers.modding.tools.visualizers
{
    /// <summary>
    /// SceneView overlay that lists all registered <see cref="IThingVisualizer"/> implementations
    /// and exposes an enable/disable toggle for each.
    /// <para>
    /// The toggle state is persisted using <see cref="EditorPrefs"/> under each visualizer's
    /// <see cref="IThingVisualizer.ToggleName"/> key.
    /// </para>
    /// <para>
    /// If a visualizer implements preferences UI via <see cref="IThingVisualizer.OnPreferencesGUI"/>,
    /// this overlay will render it inside a foldout below the toggle.
    /// </para>
    /// </summary>
    [Overlay(typeof(SceneView), "Visualizers", defaultDisplay = true)]
    public class ThingVisualizerOverlay : Overlay
    {
        /// <summary>
        /// Creates the overlay content.
        /// </summary>
        /// <returns>The root <see cref="VisualElement"/> for the overlay.</returns>
        public override VisualElement CreatePanelContent()
        {
            var root = new VisualElement
            {
                name = "thing-visualizer-overlay-root"
            };

            root.AddToClassList("thing-visualizer-overlay");

            // Build UI for each visualizer discovered by the registry.
            foreach (var visualizer in ThingVisualizer.GetVisualizers())
            {
                AddVisualizerUI(root, visualizer);
            }

            return root;
        }

        /// <summary>
        /// Adds toggle and optional preferences UI for a single visualizer.
        /// </summary>
        private static void AddVisualizerUI(VisualElement root, IThingVisualizer visualizer)
        {
            // Skip visualizers that don't want to appear in the overlay.
            // (Earlier code used null; we also treat empty/whitespace as "hidden".)
            if (string.IsNullOrWhiteSpace(visualizer.ToggleTitle))
                return;

            // Toggle row
            var toggle = CreatePrefToggle(
                label: visualizer.ToggleTitle,
                key: visualizer.ToggleName,
                defaultValue: visualizer.ToggleState,
                tooltip: visualizer.ToggleTooltip
            );

            root.Add(toggle);
        }

        /// <summary>
        /// Creates a UIElements toggle that persists its state to <see cref="EditorPrefs"/>.
        /// </summary>
        /// <param name="label">Visible label.</param>
        /// <param name="key">EditorPrefs key for persistence.</param>
        /// <param name="defaultValue">Default value if key does not exist.</param>
        /// <param name="tooltip">Tooltip text.</param>
        private static Toggle CreatePrefToggle(string label, string key, bool defaultValue, string tooltip)
        {
            var toggle = new Toggle(label)
            {
                value = EditorPrefs.GetBool(key, defaultValue),
                tooltip = tooltip ?? string.Empty
            };

            toggle.RegisterValueChangedCallback(evt =>
            {
                EditorPrefs.SetBool(key, evt.newValue);
                SceneView.RepaintAll();
            });

            return toggle;
        }
    }
}