using System;
using UnityEditor;
using UnityEngine;

namespace ilodev.stationeersmods.tools.assetsfactory
{
    /// <summary>
    /// Base implementation for assets that share the same basic Unity components.
    /// Derived constructors override ConfigureGameObject to add only asset-specific setup.
    /// </summary>
    public abstract class StationeersAssetConstructorBase : IStationeersAssetConstructor
    {
        private const string CreatedScriptsPath = "Assets/Scripts/";

        public abstract string Id { get; }
        public abstract string DefaultGameObjectName { get; }
        public abstract string GeneratedClassName { get; }
        public abstract string BaseClassName { get; }
        public abstract string AddComponentMenuPath { get; }

        public virtual string ScriptPath => CreatedScriptsPath + GeneratedClassName + ".cs";

        public virtual void ConfigureGameObject(GameObject gameObject, Type generatedType)
        {
            AddMeshComponents(gameObject);
            gameObject.AddComponent(generatedType);
            EditorUtility.SetDirty(gameObject);
        }

        protected static void AddMeshComponents(GameObject gameObject)
        {
            gameObject.AddComponent<MeshRenderer>();
            gameObject.AddComponent<MeshFilter>();
            gameObject.AddComponent<MeshCollider>();
        }
    }
}
