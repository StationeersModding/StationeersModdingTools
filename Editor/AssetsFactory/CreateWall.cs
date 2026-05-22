using Assets.Scripts.Objects;
using Assets.Scripts.Util;
using UnityEditor;
using UnityEngine;

namespace ilodev.stationeersmods.tools.assetsfactory
{
    public sealed class CreateWall : StationeersAssetStructureBase
    {
        public override string Id => "Wall";
        public override string DefaultGameObjectName => "NewStructureWallAsset";
        public override string GeneratedClassName => "Wall";
        public override string BaseClassName => "Assets.Scripts.Objects.Wall";
        public override string AddComponentMenuPath => "Stationeers/Objects/Wall";
        public override string ScriptPath => "Assets/Scripts/Objects/Wall.cs";

        [MenuItem("Assets/Create/Stationeers/QuickItems/Structure Wall", false, 1)]
        public static void CreateWallAsset()
        {
            StationeersAssetFactory.CreateAsset(new CreateWall());
        }

        /// <summary>
        /// Complete gameobject setup
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="generatedType"></param>
        public override void ConfigureGameObject(GameObject gameObject, System.Type generatedType)
        {

            base.ConfigureGameObject(gameObject, generatedType);
            MakeBoxCollider(gameObject, new Vector3(0, 0, 0.065f), new Vector3(2, 2, 0.13f));

            // Add default build state
            AddDefaultBuildState(gameObject);

            // Set Placement Type
            Structure structure = gameObject.GetComponent<Structure>();
            structure.PlacementType = PlacementSnap.Face;
            structure.SelectionDisplay = SelectionHighlightMethod.Grid;
            structure.StructureCollisionType = CollisionType.BlockFace;

            CreateBlockSound(gameObject, new Vector3(0, 0, 0.065f), new Vector3(2, 2, 0.13f));

            // Set rotation type
            ISmartRotatable smartRotatable = gameObject.GetComponent<ISmartRotatable>();
            smartRotatable.SetConnectionType(SmartRotate.ConnectionType.FaceAllAll);

            EditorUtility.SetDirty(gameObject);
        }
    }
}
