using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace stationeers.modding.tools.visualizers
{
    /// <summary>
    /// Shared IMGUI controls for visualizer preferences. Every control persists immediately,
    /// registers its default value, and repaints all Scene views.
    /// </summary>
    internal static class VisualizerPreferencesUtil
    {
        private static readonly Dictionary<string, Action> s_resetActions = new Dictionary<string, Action>();

        /// <summary>
        /// Draws and persists a color preference.
        /// </summary>
        internal static Color ColorField(
            string label,
            string preferenceKey,
            Color currentValue,
            Color defaultValue)
        {
            RegisterDefault(preferenceKey, () => SaveColor(preferenceKey, defaultValue));

            EditorGUI.BeginChangeCheck();
            Color value = EditorGUILayout.ColorField(label, currentValue);
            if (EditorGUI.EndChangeCheck())
            {
                SaveColor(preferenceKey, value);
                SceneView.RepaintAll();
            }

            return value;
        }

        /// <summary>
        /// Draws, clamps, and persists a floating-point preference.
        /// </summary>
        internal static float FloatField(
            string label,
            string preferenceKey,
            float currentValue,
            float defaultValue,
            float minimum,
            float maximum)
        {
            RegisterDefault(preferenceKey, () => EditorPrefs.SetFloat(preferenceKey, defaultValue));

            EditorGUI.BeginChangeCheck();
            float value = Mathf.Clamp(EditorGUILayout.FloatField(label, currentValue), minimum, maximum);
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetFloat(preferenceKey, value);
                SceneView.RepaintAll();
            }

            return value;
        }

        /// <summary>
        /// Draws and persists an integer slider preference.
        /// </summary>
        internal static int IntSlider(
            string label,
            string preferenceKey,
            int currentValue,
            int defaultValue,
            int minimum,
            int maximum)
        {
            RegisterDefault(preferenceKey, () => EditorPrefs.SetInt(preferenceKey, defaultValue));

            EditorGUI.BeginChangeCheck();
            int value = EditorGUILayout.IntSlider(label, currentValue, minimum, maximum);
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetInt(preferenceKey, value);
                SceneView.RepaintAll();
            }

            return value;
        }

        /// <summary>
        /// Draws a consistently formatted preference section.
        /// </summary>
        internal static void Section(string title, Action drawContents)
        {
            EditorGUILayout.Space(6f);
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            try
            {
                drawContents();
            }
            finally
            {
                EditorGUI.indentLevel--;
            }
        }

        /// <summary>
        /// Restores every registered preference to its declared default.
        /// </summary>
        internal static void ResetAllToDefaults()
        {
            foreach (Action resetAction in s_resetActions.Values)
                resetAction();

            SceneView.RepaintAll();
        }

        private static void RegisterDefault(string preferenceKey, Action resetAction)
        {
            s_resetActions[preferenceKey] = resetAction;
        }

        /// <summary>
        /// Saves color as Hex string
        /// </summary>
        /// <param name="key"></param>
        /// <param name="color"></param>
        public static void SaveColor(string key, Color color)
        {
            string hex = ColorUtility.ToHtmlStringRGBA(color);
            EditorPrefs.SetString(key, hex);
        }

        /// <summary>
        /// Load Color from Hex String
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultColor"></param>
        /// <returns></returns>
        public static Color LoadColor(string key, Color defaultColor)
        {
            if (!EditorPrefs.HasKey(key))
                return defaultColor;

            string hex = EditorPrefs.GetString(key);
            if (ColorUtility.TryParseHtmlString("#" + hex, out Color color))
                return color;

            return defaultColor;
        }

        /// <summary>
        /// Save Color as separate RGBA floats
        /// </summary>
        /// <param name="key"></param>
        /// <param name="color"></param>
        public static void SaveColorRGBA(string key, Color color)
        {
            EditorPrefs.SetFloat(key + "_R", color.r);
            EditorPrefs.SetFloat(key + "_G", color.g);
            EditorPrefs.SetFloat(key + "_B", color.b);
            EditorPrefs.SetFloat(key + "_A", color.a);
        }

        /// <summary>
        /// Load Color from RGBA floats
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultColor"></param>
        /// <returns></returns>
        public static Color LoadColorRGBA(string key, Color defaultColor)
        {
            if (!EditorPrefs.HasKey(key + "_R"))
                return defaultColor;

            float r = EditorPrefs.GetFloat(key + "_R");
            float g = EditorPrefs.GetFloat(key + "_G");
            float b = EditorPrefs.GetFloat(key + "_B");
            float a = EditorPrefs.GetFloat(key + "_A");
            return new Color(r, g, b, a);
        }

        /// <summary>
        /// Delete stored color keys (Hex)
        /// </summary>
        /// <param name="key"></param>
        public static void DeleteColor(string key)
        {
            if (EditorPrefs.HasKey(key))
                EditorPrefs.DeleteKey(key);
        }

        /// <summary>
        /// Delete stored color keys (RGBA)
        /// </summary>
        /// <param name="key"></param>
        public static void DeleteColorRGBA(string key)
        {
            EditorPrefs.DeleteKey(key + "_R");
            EditorPrefs.DeleteKey(key + "_G");
            EditorPrefs.DeleteKey(key + "_B");
            EditorPrefs.DeleteKey(key + "_A");
        }
    }
}

