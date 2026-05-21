using UnityEditor;

namespace ilodev.stationeersmods.tools.assetsfactory
{
    public static class CreateMultiConstructor
    {
        [MenuItem("Assets/Create/Stationeers/QuickItems/Multi Constructor Kit", false, 1)]
        public static void CreateMultiConstructorAsset()
        {
            StationeersAssetFactory.CreateAsset(StationeersAssetDefinitions.MultiConstructor);
        }
    }
}
