using Assets.Scripts.Objects;
using UnityEditor;
using UnityEngine;

namespace ilodev.stationeersmods.tools.assetsfactory
{
    public sealed class CreateWall : StationeersAssetConstructorBase
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
            MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
            MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();

            BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
            boxCollider.center = new Vector3(0, 0, 0.065f);
            boxCollider.size = new Vector3(2, 2, 0.13f);

            gameObject.AddComponent(generatedType);

            // Add default build state
            // TODO: move this to a helper function
            Structure structure = gameObject.GetComponent<Structure>();
            // Add default build state
            BuildState defaultBuildState = new BuildState();
            defaultBuildState.BlockAir = true;
            defaultBuildState.BlockLight = true;
            defaultBuildState.BlockGravity = true;
            defaultBuildState.Visualizer = meshRenderer;
            defaultBuildState.RenderMode = BuildStateRenderMode.OnMineAndPreviousStates;
            structure.BuildStates = new System.Collections.Generic.List<BuildState>();
            structure.BuildStates.Add(defaultBuildState);


            // Add sound blocker
            // TODO: Move this to a helper function
            GameObject blockSound = new GameObject("BlockSound");
            blockSound.transform.parent = gameObject.transform;
            blockSound.layer = LayerMask.NameToLayer("BlockSound");
            BoxCollider blockSoundCollider = blockSound.AddComponent<BoxCollider>();
            blockSoundCollider.center = boxCollider.center;
            blockSoundCollider.size = boxCollider.size;


            EditorUtility.SetDirty(gameObject);
        }
    }
}
