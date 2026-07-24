using UnityEditor;
using UnityEngine;

namespace stationeers.modding.tools.visualizers
{
    /// <summary>
    /// Editor window containing shared and per-visualizer display preferences.
    /// </summary>
    public class ThingVisualizerPreferences : EditorWindow
    {
        private Vector2 scrollPosition;

        [MenuItem("Window/Stationeers Modding Tools/Visualizers")]
        public static void ShowWindow()
        {
            GetWindow<ThingVisualizerPreferences>("Visualizer Settings").minSize = new Vector2(340f, 260f);
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Visualizer Settings", EditorStyles.largeLabel);
            EditorGUILayout.HelpBox("Changes are saved immediately and applied to all Scene views.", MessageType.Info);

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandHeight(true));

            VisualizerPreferencesUtil.Section("Labels", () =>
            {
                int fontSize = EditorPrefs.GetInt(
                    VisualizerDrawUtil.LabelFontSizePrefKey,
                    VisualizerDrawUtil.DefaultLabelFontSize);

                VisualizerPreferencesUtil.IntSlider(
                    "Font Size",
                    VisualizerDrawUtil.LabelFontSizePrefKey,
                    fontSize,
                    VisualizerDrawUtil.DefaultLabelFontSize,
                    8,
                    32);
            });

            foreach (IThingVisualizer visualizer in ThingVisualizer.GetVisualizers())
                VisualizerPreferencesUtil.Section(visualizer.ToggleTitle, visualizer.OnPreferencesGUI);

            EditorGUILayout.EndScrollView();
            DrawFooter();
        }

        /// <summary>
        /// Draws actions that remain visible at the bottom of the settings window.
        /// </summary>
        private void DrawFooter()
        {
            EditorGUILayout.Space(6f);
            EditorGUILayout.LabelField(GUIContent.none, GUI.skin.horizontalSlider);

            if (!GUILayout.Button("Reset All to Defaults", GUILayout.Height(28f)))
                return;

            bool confirmed = EditorUtility.DisplayDialog(
                "Reset Visualizer Settings",
                "Reset every value in the Visualizer Settings window to its default?",
                "Reset",
                "Cancel");

            if (!confirmed)
                return;

            VisualizerPreferencesUtil.ResetAllToDefaults();

            // Visualizers cache preference values in instance fields. Recreate them so the reset
            // is visible immediately without requiring a script reload or reopening Unity.
            ThingVisualizer.GetVisualizers(refresh: true);

            GUI.FocusControl(null);
            Repaint();
        }
    }
}
