using System.Collections.Generic;
using Assets.Scripts.Objects;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace stationeers.modding.tools.visualizers
{
    /// <summary>
    /// Maintains a cached set of <see cref="Thing"/> instances present during SceneView rendering.
    /// </summary>
    [InitializeOnLoad]
    internal static class ThingVisualizerRegistry
    {
        private static readonly HashSet<Thing> s_things = new HashSet<Thing>();
        private static readonly List<Thing> s_buffer = new List<Thing>(256);
        private static bool s_dirty = true;

        static ThingVisualizerRegistry()
        {
            EditorApplication.hierarchyChanged += MarkDirty;
            EditorSceneManager.sceneOpened += (_, __) => MarkDirty();
            EditorSceneManager.sceneClosed += _ => MarkDirty();
            EditorSceneManager.activeSceneChangedInEditMode += (_, __) => MarkDirty();
            EditorApplication.playModeStateChanged += _ => MarkDirty();

            MarkDirty();
        }

        internal static void MarkDirty() => s_dirty = true;

        /// <summary>
        /// Returns a cached snapshot of Things in loaded scenes. Rebuilds cache if needed.
        /// </summary>
        internal static IReadOnlyCollection<Thing> GetThingsInLoadedScenes()
        {
            if (s_dirty)
                Rebuild();

            return s_things;
        }

        private static void Rebuild()
        {
            s_things.Clear();

            int sceneCount = SceneManager.sceneCount;
            for (int i = 0; i < sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (!scene.IsValid() || !scene.isLoaded)
                    continue;

                foreach (GameObject root in scene.GetRootGameObjects())
                {
                    root.GetComponentsInChildren(true, s_buffer);

                    for (int j = 0; j < s_buffer.Count; j++)
                        s_things.Add(s_buffer[j]);

                    s_buffer.Clear();
                }
            }

            s_dirty = false;
        }
    }
}