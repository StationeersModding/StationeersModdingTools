using UnityEditor;

namespace ilodev.stationeersmods.tools.assetsfactory
{
    public static class CreateWall
    {
        [MenuItem("Assets/Create/Stationeers/QuickItems/Structure Wall", false, 1)]
        public static void CreateWallAsset()
        {
            StationeersAssetFactory.CreateAsset(StationeersAssetDefinitions.Wall);
        }
    }
}
