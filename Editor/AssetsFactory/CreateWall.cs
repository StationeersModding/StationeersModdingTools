using UnityEditor;

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
    }
}
