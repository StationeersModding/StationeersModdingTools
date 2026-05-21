using Assets.Scripts.Objects;
using UnityEditor;
using UnityEngine;

namespace ilodev.stationeersmods.tools.assetsfactory
{
    public sealed class CreateConstructor : StationeersAssetConstructorBase
    {
        public override string Id => "Constructor";
        public override string DefaultGameObjectName => "NewConstructorAsset";
        public override string GeneratedClassName => "Constructor";
        public override string BaseClassName => "Assets.Scripts.Objects.Constructor";
        public override string AddComponentMenuPath => "Stationeers/Objects/Constructor";
        public override string ScriptPath => "Assets/Scripts/Objects/Constructor.cs";

        [MenuItem("Assets/Create/Stationeers/QuickItems/Constructor Kit", false, 1)]
        public static void CreateConstructorAsset()
        {
            StationeersAssetFactory.CreateAsset(new CreateConstructor());
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
                Debug.LogWarning("Created Constructor asset does not have a Thing component. Interactables were not added.");
                return;
            }

            InteractableHelpers.AddInteractable(thing, "SplitOne", InteractableType.Button1);
            InteractableHelpers.AddInteractable(thing, "SplitHalf", InteractableType.Button2);
        }
    }
}
