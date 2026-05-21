using UnityEditor;

namespace ilodev.stationeersmods.tools.assetsfactory
{
    public static class CreateDynamicThingConstructor
    {
        [MenuItem("Assets/Create/Stationeers/QuickItems/DynamicThing Constructor Kit", false, 1)]
        public static void CreateDynamicThingConstructorAsset()
        {
            StationeersAssetFactory.CreateAsset(StationeersAssetDefinitions.DynamicThingConstructor);
        }
    }
}
