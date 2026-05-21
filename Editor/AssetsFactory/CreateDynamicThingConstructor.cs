using Assets.Scripts.Objects;
using UnityEditor;
using UnityEngine;

namespace ilodev.stationeersmods.tools.assetsfactory
{
    public sealed class CreateDynamicThingConstructor : StationeersAssetConstructorBase
    {
        public override string Id => "DynamicThingConstructor";
        public override string DefaultGameObjectName => "NewDynamicThingConstructorAsset";
        public override string GeneratedClassName => "DynamicThingConstructor";
        public override string BaseClassName => "Assets.Scripts.Objects.Items.DynamicThingConstructor";
        public override string AddComponentMenuPath => "Stationeers/Objects/DynamicThingConstructor";
        public override string ScriptPath => "Assets/Scripts/Objects/DynamicThingConstructor.cs";

        [MenuItem("Assets/Create/Stationeers/QuickItems/DynamicThing Constructor Kit", false, 1)]
        public static void CreateDynamicThingConstructorAsset()
        {
            StationeersAssetFactory.CreateAsset(new CreateDynamicThingConstructor());
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
                Debug.LogWarning("Created DynamicThingConstructor asset does not have a Thing component. Interactables were not added.");
                return;
            }

            InteractableHelpers.AddInteractable(thing, "SplitOne", InteractableType.Button1);
            InteractableHelpers.AddInteractable(thing, "SplitHalf", InteractableType.Button2);
        }
    }
}
