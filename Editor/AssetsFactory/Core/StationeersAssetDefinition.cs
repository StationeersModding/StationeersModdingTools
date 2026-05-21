using System;

namespace ilodev.stationeersmods.tools.assetsfactory
{
    /// <summary>
    /// Describes one generated Stationeers asset type.
    /// Keep this class data-only so new asset types can be added without copying the creation workflow.
    /// </summary>
    [Serializable]
    public sealed class StationeersAssetDefinition
    {
        public string Id;
        public string MenuName;
        public string DefaultGameObjectName;
        public string GeneratedClassName;
        public string BaseClassName;
        public string AddComponentMenuPath;
        public string ScriptPath;
        public bool AddDefaultStackableInteractables = true;

        public StationeersAssetDefinition(
            string id,
            string menuName,
            string defaultGameObjectName,
            string generatedClassName,
            string baseClassName,
            string addComponentMenuPath,
            string scriptPath)
        {
            Id = id;
            MenuName = menuName;
            DefaultGameObjectName = defaultGameObjectName;
            GeneratedClassName = generatedClassName;
            BaseClassName = baseClassName;
            AddComponentMenuPath = addComponentMenuPath;
            ScriptPath = scriptPath;
        }
    }

    /// <summary>
    /// Central registry of asset types supported by this editor tool.
    /// </summary>
    public static class StationeersAssetDefinitions
    {
        private const string CreatedScriptsPath = "Assets/Scripts/";

        public static readonly StationeersAssetDefinition Constructor = new StationeersAssetDefinition(
            "Constructor",
            "Assets/Create/Stationeers/QuickItems/Constructor Kit",
            "NewConstructorAsset",
            "Constructor",
            "Assets.Scripts.Objects.Constructor",
            "Stationeers/Objects/Constructor",
            CreatedScriptsPath + "Objects/Constructor.cs");

        public static readonly StationeersAssetDefinition MultiConstructor = new StationeersAssetDefinition(
            "MultiConstructor",
            "Assets/Create/Stationeers/QuickItems/Multi Constructor Kit",
            "NewMultiConstructorAsset",
            "MultiConstructor",
            "Assets.Scripts.Objects.MultiConstructor",
            "Stationeers/Objects/MultiConstructor",
            CreatedScriptsPath + "Objects/MultiConstructor.cs");

        public static readonly StationeersAssetDefinition DynamicThingConstructor = new StationeersAssetDefinition(
            "DynamicThingConstructor",
            "Assets/Create/Stationeers/QuickItems/DynamicThing Constructor Kit",
            "NewDynamicThingConstructorAsset",
            "DynamicThingConstructor",
            "Assets.Scripts.Objects.Items.DynamicThingConstructor",
            "Stationeers/Objects/DynamicThingConstructor",
            CreatedScriptsPath + "Objects/DynamicThingConstructor.cs");

        public static StationeersAssetDefinition FindById(string id)
        {
            if (id == Constructor.Id) return Constructor;
            if (id == MultiConstructor.Id) return MultiConstructor;
            if (id == DynamicThingConstructor.Id) return DynamicThingConstructor;
            return null;
        }
    }
}
