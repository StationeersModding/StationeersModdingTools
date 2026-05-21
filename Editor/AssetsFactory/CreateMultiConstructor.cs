using Assets.Scripts.Objects;
using UnityEditor;
using UnityEngine;

namespace ilodev.stationeersmods.tools.assetsfactory
{
    public sealed class CreateMultiConstructor : StationeersAssetConstructorBase
    {
        public override string Id => "MultiConstructor";
        public override string DefaultGameObjectName => "NewMultiConstructorAsset";
        public override string GeneratedClassName => "MultiConstructor";
        public override string BaseClassName => "Assets.Scripts.Objects.MultiConstructor";
        public override string AddComponentMenuPath => "Stationeers/Objects/MultiConstructor";
        public override string ScriptPath => "Assets/Scripts/Objects/MultiConstructor.cs";

        [MenuItem("Assets/Create/Stationeers/QuickItems/Multi Constructor Kit", false, 1)]
        public static void CreateMultiConstructorAsset()
        {
            StationeersAssetFactory.CreateAsset(new CreateMultiConstructor());
        }

        public override void ConfigureGameObject(GameObject gameObject, System.Type generatedType)
        {
            base.ConfigureGameObject(gameObject, generatedType);
            AddStackableInteractables(gameObject);
        }

        private static void AddStackableInteractables(GameObject gameObject)
        {
            Thing thing = gameObject.GetComponent<Thing>();
            if (thing == null)
            {
                Debug.LogWarning("Created MultiConstructor asset does not have a Thing component. Interactables were not added.");
                return;
            }

            InteractableHelpers.AddInteractable(thing, "SplitOne", InteractableType.Button1);
            InteractableHelpers.AddInteractable(thing, "SplitHalf", InteractableType.Button2);
        }
    }
}
