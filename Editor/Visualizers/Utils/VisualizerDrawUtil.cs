using System;
using UnityEditor;
using UnityEngine;

namespace stationeers.modding.tools.visualizers
{
    /// <summary>
    /// Shared Scene view drawing helpers used by visualizers.
    /// </summary>
    internal static class VisualizerDrawUtil
    {
        internal const string LabelFontSizePrefKey = "Visualizer.Labels.FontSize";
        internal const int DefaultLabelFontSize = 12;

        /// <summary>
        /// Draws a wire cube and, optionally, a translucent solid fill.
        /// </summary>
        internal static void DrawCube(Vector3 center, float size, Color wire, Color fill, bool drawFill = true)
        {
            Handles.color = wire;
            Handles.DrawWireCube(center, Vector3.one * size);

            if (drawFill)
            {
                Color outline = wire;
                outline.a = 1f;
                DrawSolidCube(center, size, fill, outline);
            }
        }

        /// <summary>
        /// Draws a cube as six solid rectangles with an outline.
        /// </summary>
        internal static void DrawSolidCube(Vector3 center, float size, Color faceColor, Color outlineColor)
        {
            Vector3 half = Vector3.one * (size * 0.5f);
            Vector3[] corners =
            {
                center + new Vector3(-half.x, -half.y, -half.z),
                center + new Vector3( half.x, -half.y, -half.z),
                center + new Vector3( half.x, -half.y,  half.z),
                center + new Vector3(-half.x, -half.y,  half.z),
                center + new Vector3(-half.x,  half.y, -half.z),
                center + new Vector3( half.x,  half.y, -half.z),
                center + new Vector3( half.x,  half.y,  half.z),
                center + new Vector3(-half.x,  half.y,  half.z)
            };

            DrawFace(corners, 0, 1, 2, 3, faceColor, outlineColor);
            DrawFace(corners, 7, 6, 5, 4, faceColor, outlineColor);
            DrawFace(corners, 4, 5, 1, 0, faceColor, outlineColor);
            DrawFace(corners, 6, 7, 3, 2, faceColor, outlineColor);
            DrawFace(corners, 5, 6, 2, 1, faceColor, outlineColor);
            DrawFace(corners, 7, 4, 0, 3, faceColor, outlineColor);
        }

        private static void DrawFace(Vector3[] corners, int a, int b, int c, int d, Color fill, Color outline)
        {
            Handles.DrawSolidRectangleWithOutline(
                new[] { corners[a], corners[b], corners[c], corners[d] }, fill, outline);
        }

        /// <summary>
        /// Temporarily replaces <see cref="Handles.matrix"/> for one drawing operation.
        /// </summary>
        internal static void WithHandlesMatrix(Matrix4x4 matrix, Action drawAction)
        {
            Matrix4x4 previous = Handles.matrix;
            Handles.matrix = matrix;
            try
            {
                drawAction();
            }
            finally
            {
                Handles.matrix = previous;
            }
        }

        /// <summary>
        /// Draws rich text centered on a world-space point.
        /// </summary>
        internal static void DrawCenteredLabel(Vector3 worldPosition, string richText)
        {
            GUIStyle style = Styles.CenterRichLabel;
            style.fontSize = EditorPrefs.GetInt(LabelFontSizePrefKey, DefaultLabelFontSize);

            GUIContent content = new GUIContent(richText);
            Vector2 guiPosition = HandleUtility.WorldToGUIPoint(worldPosition);
            Vector2 size = style.CalcSize(content);
            Rect rect = new Rect(guiPosition.x - size.x * 0.5f, guiPosition.y - size.y * 0.5f, size.x, size.y);

            Handles.BeginGUI();
            GUI.Label(rect, content, style);
            Handles.EndGUI();
        }

        /// <summary>
        /// Snaps each coordinate to the nearest grid increment and applies an optional offset.
        /// </summary>
        internal static Vector3 SnapToGrid(Vector3 position, float gridSize, float offset = 0f)
        {
            return new Vector3(
                Mathf.Round(position.x / gridSize) * gridSize + offset,
                Mathf.Round(position.y / gridSize) * gridSize + offset,
                Mathf.Round(position.z / gridSize) * gridSize + offset);
        }

        private static class Styles
        {
            internal static readonly GUIStyle CenterRichLabel = new GUIStyle(EditorStyles.label)
            {
                richText = true,
                alignment = TextAnchor.MiddleCenter,
                wordWrap = false
            };
        }
    }
}
