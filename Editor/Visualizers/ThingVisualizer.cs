using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Objects;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace stationeers.modding.tools.visualizers
{
    /// <summary>
    /// Editor-side visualizer dispatcher.
    /// <para>
    /// Discovers all <see cref="IThingVisualizer"/> implementations, instantiates them once, and invokes
    /// <see cref="IThingVisualizer.OnSceneGUI"/> for valid <see cref="Thing"/> objects.
    /// </para>
    /// <para>
    /// For performance, this uses <see cref="ThingVisualizerRegistry"/> in normal scene mode.
    /// In Prefab Mode, it queries the active prefab stage scene directly.
    /// </para>
    /// </summary>
    [InitializeOnLoad]
    public static class ThingVisualizer
    {
        private static IThingVisualizer[] s_visualizers = Array.Empty<IThingVisualizer>();

        // Reused list to avoid allocations each repaint.
        private static readonly List<IThingVisualizer> s_enabledVisualizers = new List<IThingVisualizer>(32);

        static ThingVisualizer()
        {
            RefreshVisualizers();
            SceneView.duringSceneGui += OnSceneGUI;
        }

        /// <summary>
        /// Returns the currently discovered visualizers.
        /// </summary>
        /// <param name="refresh">If true, forces rediscovery and reinstantiation.</param>
        public static IReadOnlyList<IThingVisualizer> GetVisualizers(bool refresh = false)
        {
            if (refresh || s_visualizers.Length == 0)
                RefreshVisualizers();

            return s_visualizers;
        }

        /// <summary>
        /// Discovers and instantiates all types implementing <see cref="IThingVisualizer"/>.
        /// Instances are cached and reused for subsequent draws.
        /// </summary>
        private static void RefreshVisualizers()
        {
            var types = TypeCache.GetTypesDerivedFrom<IThingVisualizer>();
            var instances = new List<IThingVisualizer>(types.Count);

            foreach (var type in types)
            {
                if (type.IsAbstract || type.IsGenericTypeDefinition)
                    continue;

                try
                {
                    if (Activator.CreateInstance(type) is IThingVisualizer v)
                        instances.Add(v);
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"[ThingVisualizer] Failed to create visualizer '{type.FullName}': {ex.Message}");
                }
            }

            // Stable order for UI and execution.
            s_visualizers = instances
                .OrderBy(v => v.ToggleTitle ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ThenBy(v => v.GetType().Name, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        /// <summary>
        /// SceneView callback. Invokes each enabled visualizer for each eligible <see cref="Thing"/>.
        /// </summary>
        private static void OnSceneGUI(SceneView sceneView)
        {
            if (s_visualizers.Length == 0)
                return;

            // Build enabled visualizer list once per repaint (avoid per-Thing/per-visualizer EditorPrefs calls).
            s_enabledVisualizers.Clear();
            for (int i = 0; i < s_visualizers.Length; i++)
            {
                var v = s_visualizers[i];
                if (EditorPrefs.GetBool(v.ToggleName, v.ToggleState))
                    s_enabledVisualizers.Add(v);
            }

            if (s_enabledVisualizers.Count == 0)
                return;

            PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            if (prefabStage != null)
            {
                DrawForPrefabStage(sceneView, prefabStage);
            }
            else
            {
                DrawForLoadedScenes(sceneView);
            }
        }

        /// <summary>
        /// Returns whether a GameObject should contribute editor visualizers.
        /// In addition to normal activation, this respects the Hierarchy Scene Visibility eye
        /// on the object and any of its parents. Scene visibility does not modify
        /// <see cref="GameObject.activeInHierarchy"/>, so it must be checked separately.
        /// </summary>
        private static bool ShouldDraw(GameObject gameObject)
        {
            if (gameObject == null || !gameObject.activeInHierarchy)
                return false;

            SceneVisibilityManager visibility = SceneVisibilityManager.instance;
            for (Transform current = gameObject.transform; current != null; current = current.parent)
            {
                if (visibility.IsHidden(current.gameObject, includeDescendants: false))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Draws visualizers for Things in normal loaded scenes using the cached registry.
        /// </summary>
        private static void DrawForLoadedScenes(SceneView sceneView)
        {
            var things = ThingVisualizerRegistry.GetThingsInLoadedScenes();
            foreach (Thing thing in things)
            {
                if (thing == null)
                    continue;

                GameObject go = thing.gameObject;
                if (go == null)
                    continue;

                // Registry includes inactive objects; skip disabled objects for expected behavior.
                if (!ShouldDraw(go))
                    continue;

                for (int i = 0; i < s_enabledVisualizers.Count; i++)
                {
                    try
                    {
                        s_enabledVisualizers[i].OnSceneGUI(sceneView, thing);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning($"[ThingVisualizer] Visualizer '{s_enabledVisualizers[i].GetType().Name}' threw: {ex.Message}");
                    }
                }
            }
        }

        /// <summary>
        /// Draws visualizers for Things in the active Prefab Stage.
        /// <para>
        /// Prefab stage objects are not part of the regular loaded scene list, so we query the stage root.
        /// </para>
        /// </summary>
        private static void DrawForPrefabStage(SceneView sceneView, PrefabStage prefabStage)
        {
            GameObject root = prefabStage.prefabContentsRoot;
            if (root == null)
                return;

            // Collect Things under the prefab root (include inactive for consistency).
            var things = root.GetComponentsInChildren<Thing>(includeInactive: true);
            for (int t = 0; t < things.Length; t++)
            {
                Thing thing = things[t];
                if (thing == null)
                    continue;

                GameObject go = thing.gameObject;
                if (go == null)
                    continue;

                if (!ShouldDraw(go))
                    continue;

                for (int i = 0; i < s_enabledVisualizers.Count; i++)
                {
                    try
                    {
                        s_enabledVisualizers[i].OnSceneGUI(sceneView, thing);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning($"[ThingVisualizer] Visualizer '{s_enabledVisualizers[i].GetType().Name}' threw: {ex.Message}");
                    }
                }
            }
        }
    }
}