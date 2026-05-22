using Assets.Scripts.Objects;
using Assets.Scripts.Util;
using UnityEditor;
using UnityEngine;

namespace ilodev.stationeersmods.tools.assetsfactory
{
    public sealed class CreateFrame : StationeersAssetStructureBase
    {
        public override string Id => "Frame";
        public override string DefaultGameObjectName => "NewStructureFrameAsset";
        public override string GeneratedClassName => "Frame";
        public override string BaseClassName => "Objects.Structures.Frame";
        public override string AddComponentMenuPath => "Stationeers/Objects/Structures/Frame";
        public override string ScriptPath => "Assets/Scripts/Objects/Structures/Frame.cs";

        [MenuItem("Assets/Create/Stationeers/QuickItems/Structure Frame", false, 1)]
        public static void CreateFrameAsset()
        {
            StationeersAssetFactory.CreateAsset(new CreateFrame());
        }

        /// <summary>
        /// Complete gameobject setup
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="generatedType"></param>
        public override void ConfigureGameObject(GameObject gameObject, System.Type generatedType)
        {

            base.ConfigureGameObject(gameObject, generatedType);
            MakeBoxCollider(gameObject, new Vector3(0, 0, 0), new Vector3(2, 2, 2));

            // Add default build state
            AddDefaultBuildState(gameObject);

            // Set Placement Type
            Structure structure = gameObject.GetComponent<Structure>();
            structure.PlacementType = PlacementSnap.Grid;
            structure.SelectionDisplay = SelectionHighlightMethod.Grid;
            structure.StructureCollisionType = CollisionType.BlockGrid;

            CreateBlockSound(gameObject, new Vector3(0, 0, 0), new Vector3(2, 2, 2));

            EditorUtility.SetDirty(gameObject);
        }

    }
}
