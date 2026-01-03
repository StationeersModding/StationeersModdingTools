using System;
using Assets.Scripts.Objects;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Drawing;

namespace ilodev.stationeersmods.tools.visualizers
{
  public class ThingInteractableVisualizer : IThingVisualizer
  {
    public void OnSceneGUI(SceneView sceneView, UnityEngine.Object target)
    {
      if (!EditorPrefs.GetBool("Visualizer.Interactables", true))
        return;

      Thing thing = target as Thing;
      if (thing == null) return;

      foreach (Interactable interactable in thing.Interactables)
      {
        Handles.color = new UnityEngine.Color(1.0f, 0.5f, 0.9f, 1.0f); // Purple

        Vector3 localCenter = interactable.Bounds.center;
        Vector3 size = interactable.Bounds.size;

        // If we have a collider, follow the GetSelection() logic
        if (interactable.Collider != null)
        {
          Transform t = interactable.Collider.transform;

          // Apply lossy scale to the size as seen in GetSelection
          Vector3 scale = t.lossyScale;
          size.x *= scale.x;
          size.y *= scale.y;
          size.z *= scale.z;

          // Draw using the transform's matrix so rotation and position are perfect
          // We use Vector3.Scale(localCenter, scale) to position the box 
          // relative to the transform's origin
          WithHandlesMatrix(Matrix4x4.TRS(t.position, t.rotation, Vector3.one), () =>
          {
            Vector3 scaledCenter = Vector3.Scale(localCenter, scale);
            Handles.DrawWireCube(scaledCenter, size);
          });

          // Label at world position
          DrawLabel(t.position + t.TransformDirection(Vector3.Scale(localCenter, scale)), interactable);
        }
        else
        {
          // Fallback for non-collider interactables (Slots)
          Transform slotTransform = GetSlotTransform(thing, interactable.Action);
          if (slotTransform != null)
          {
            WithHandlesMatrix(Matrix4x4.TRS(slotTransform.position, slotTransform.rotation, Vector3.one), () =>
            {
              Handles.DrawWireCube(Vector3.zero, new Vector3(0.1f, 0.1f, 0.1f));
            });
            DrawLabel(slotTransform.position, interactable);
          }
        }
      }
    }

    private void DrawLabel(Vector3 position, Interactable interactable)
    {
      GUIStyle boldLabel = new GUIStyle(EditorStyles.label) { richText = true };
      string text = $"<color=#FFFFFF><b>{interactable.StringKey}</b></color>\n{interactable.Action}";
      Handles.Label(position, text, boldLabel);
    }

    /// <summary>
    /// Draw a square rotated
    /// </summary>
    /// <param name="matrix"></param>
    /// <param name="drawAction"></param>
    void WithHandlesMatrix(Matrix4x4 matrix, Action drawAction)
    {
      var oldMatrix = Handles.matrix;
      Handles.matrix = matrix;
      drawAction();
      Handles.matrix = oldMatrix;
    }

    Transform GetSlotTransform(Thing thing, InteractableType interactableType)
    {
      foreach (Slot slot in thing.Slots)
      {
        if (slot.Action == interactableType)
        {
          return slot.Location;
        }
      }
      return (default(Transform));
    }

  }
}
