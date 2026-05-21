using UnityEditor;

namespace ilodev.stationeersmods.tools.assetsfactory
{
    public static class CreateFrame
    {
        [MenuItem("Assets/Create/Stationeers/QuickItems/Structure Frame", false, 1)]
        public static void CreateFrameAsset()
        {
            StationeersAssetFactory.CreateAsset(StationeersAssetDefinitions.Frame);
        }
    }
}
