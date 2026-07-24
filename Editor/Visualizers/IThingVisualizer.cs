using UnityEditor;
using UnityEngine;

namespace stationeers.modding.tools.visualizers
{
    /// <summary>
    /// Contract for scene/prefab visualizers that draw editor gizmos/handles and optionally expose preferences UI.
    /// </summary>
    public interface IThingVisualizer
    {
        /// <summary>
        /// Title shown in the UI section/header for this visualizer (e.g. "Pipe Networks").
        /// </summary>
        string ToggleTitle { get; }

        /// <summary>
        /// Unique/internal name used to persist the toggle state in preferences (e.g. "show_pipe_networks").
        /// </summary>
        string ToggleName { get; }

        /// <summary>
        /// Tooltip shown when hovering the toggle control.
        /// </summary>
        string ToggleTooltip { get; }

        /// <summary>
        /// Current enabled/disabled state of this visualizer.
        /// </summary>
        bool ToggleState { get; }

        /// <summary>
        /// Called while the Scene View (or Prefab Stage) is being rendered. Use this to draw handles/gizmos.
        /// </summary>
        /// <param name="sceneView">The SceneView currently rendering.</param>
        /// <param name="target">The currently inspected/selected Unity object the visualizer is acting on.</param>
        void OnSceneGUI(SceneView sceneView, Object target);

        /// <summary>
        /// Draws any preferences/configuration UI for this visualizer (e.g. colors, sizes, filters).
        /// </summary>
        void OnPreferencesGUI();
    }
}