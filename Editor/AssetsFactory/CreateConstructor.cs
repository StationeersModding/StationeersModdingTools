using UnityEditor;

namespace ilodev.stationeersmods.tools.assetsfactory
{
    public static class CreateConstructor
    {
        [MenuItem("Assets/Create/Stationeers/QuickItems/Constructor Kit", false, 1)]
        public static void CreateConstructorAsset()
        {
            StationeersAssetFactory.CreateAsset(StationeersAssetDefinitions.Constructor);
        }
    }
}
