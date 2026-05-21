using UnityEditor;

namespace ilodev.stationeersmods.tools.assetsfactory
{
    public sealed class CreateFrame : StationeersAssetConstructorBase
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
    }
}
