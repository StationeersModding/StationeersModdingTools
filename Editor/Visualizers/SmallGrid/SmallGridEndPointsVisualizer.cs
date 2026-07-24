using Assets.Scripts.Objects;
using UnityEditor;
using UnityEngine;

namespace stationeers.modding.tools.visualizers
{
    /// <summary>
    /// Draws OpenEnd markers, forward arrows, and connection metadata for a <see cref="SmallGrid"/>.
    /// </summary>
    public class SmallGridEndPointsVisualizer : IThingVisualizer
    {
        private const string PipeColorKey = "Visualizer.OpenEnds.PipeColor";
        private const string LiquidColorKey = "Visualizer.OpenEnds.LiquidColor";
        private const string ChuteColorKey = "Visualizer.OpenEnds.ChuteColor";
        private const string ElectricalColorKey = "Visualizer.OpenEnds.ElectricalColor";
        private const string OtherColorKey = "Visualizer.OpenEnds.OtherColor";
        private const string MarkerSizeKey = "Visualizer.OpenEnds.MarkerSize";
        private const string ArrowSizeKey = "Visualizer.OpenEnds.ArrowSize";

        private const float DefaultMarkerSize = 0.1f;
        private const float DefaultArrowSize = 0.2f;

        private static readonly Color DefaultPipeColor = Color.yellow;
        private static readonly Color DefaultLiquidColor = Color.blue;
        private static readonly Color DefaultChuteColor = Color.gray;
        private static readonly Color DefaultElectricalColor = Color.red;
        private static readonly Color DefaultOtherColor = Color.green;

        private Color pipeColor = VisualizerPreferencesUtil.LoadColor(PipeColorKey, DefaultPipeColor);
        private Color liquidColor = VisualizerPreferencesUtil.LoadColor(LiquidColorKey, DefaultLiquidColor);
        private Color chuteColor = VisualizerPreferencesUtil.LoadColor(ChuteColorKey, DefaultChuteColor);
        private Color electricalColor = VisualizerPreferencesUtil.LoadColor(ElectricalColorKey, DefaultElectricalColor);
        private Color otherColor = VisualizerPreferencesUtil.LoadColor(OtherColorKey, DefaultOtherColor);
        private float markerSize = EditorPrefs.GetFloat(MarkerSizeKey, DefaultMarkerSize);
        private float arrowSize = EditorPrefs.GetFloat(ArrowSizeKey, DefaultArrowSize);

        public string ToggleTitle => "Open Ends";
        public string ToggleName => "Visualizer.OpenEnds";
        public string ToggleTooltip => "Display OpenEnd direction and connection information.";
        public bool ToggleState => true;

        public void OnPreferencesGUI()
        {
            pipeColor = VisualizerPreferencesUtil.ColorField("Pipe Color", PipeColorKey, pipeColor, DefaultPipeColor);
            liquidColor = VisualizerPreferencesUtil.ColorField("Liquid Pipe Color", LiquidColorKey, liquidColor, DefaultLiquidColor);
            chuteColor = VisualizerPreferencesUtil.ColorField("Chute Color", ChuteColorKey, chuteColor, DefaultChuteColor);
            electricalColor = VisualizerPreferencesUtil.ColorField("Power / Data Color", ElectricalColorKey, electricalColor, DefaultElectricalColor);
            otherColor = VisualizerPreferencesUtil.ColorField("Other Network Color", OtherColorKey, otherColor, DefaultOtherColor);
            markerSize = VisualizerPreferencesUtil.FloatField("Marker Size", MarkerSizeKey, markerSize, DefaultMarkerSize, 0.01f, 2f);
            arrowSize = VisualizerPreferencesUtil.FloatField("Arrow Size", ArrowSizeKey, arrowSize, DefaultArrowSize, 0.01f, 4f);
        }

        public void OnSceneGUI(SceneView sceneView, Object target)
        {
            if (target is not SmallGrid smallGrid)
                return;

            foreach (var openEnd in smallGrid.OpenEnds)
            {
                if (openEnd?.Transform == null)
                    continue;

                Vector3 position = openEnd.Transform.position;
                Vector3 forward = openEnd.Transform.forward;
                Color color = GetColor(openEnd.ConnectionType);
                color.a = 0.6f;
                Handles.color = color;

                Handles.SphereHandleCap(0, position, Quaternion.identity, markerSize, EventType.Repaint);
                Handles.ArrowHandleCap(0, position - forward * markerSize, Quaternion.LookRotation(forward), arrowSize, EventType.Repaint);

                VisualizerDrawUtil.DrawCenteredLabel(position,
                    $"<color=#FFFFFF><b>{openEnd.ConnectionType}</b></color>\n{openEnd.ConnectionRole}");
            }
        }

        private Color GetColor(NetworkType type)
        {
            return type switch
            {
                NetworkType.Pipe => pipeColor,
                NetworkType.PipeLiquid => liquidColor,
                NetworkType.Chute => chuteColor,
                NetworkType.Power => electricalColor,
                NetworkType.Data => electricalColor,
                NetworkType.PowerAndData => electricalColor,
                _ => otherColor
            };
        }
    }
}
