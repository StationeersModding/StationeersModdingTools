using Assets.Scripts.Objects;
using Assets.Scripts.Util;
using UnityEditor;
using UnityEngine;

namespace ilodev.stationeersmods.tools.assetsfactory
{
    public static class InteractableHelpers
    {
        /// <summary>
        /// Adds an interactable to a Thing and marks it dirty for Unity serialization.
        /// </summary>
        public static bool AddInteractable(Thing thing, string name, InteractableType action)
        {
            if (thing == null)
            {
                Debug.LogWarning("Cannot add Stationeers interactable because the target Thing is null.");
                return false;
            }

            if (thing.Interactables == null)
            {
                Debug.LogWarning("Cannot add Stationeers interactable because Thing.Interactables is null.");
                return false;
            }

            Interactable interactable = new Interactable
            {
                StringKey = name,
                Action = action,
                CanKeyInteract = true
            };

            thing.Interactables.Add(interactable);
            EditorUtility.SetDirty(thing);
            return true;
        }

        public static bool AddInteractable(GameObject gameObject, string name, InteractableType action)
        {
            if (gameObject == null)
            {
                Debug.LogWarning("Cannot add Stationeers interactable because the target GameObject is null.");
                return false;
            }

            return AddInteractable(gameObject.GetComponent<Thing>(), name, action);
        }
    }
}
