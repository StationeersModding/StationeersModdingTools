using System;
using UnityEngine;

namespace ilodev.stationeersmods.tools.assetsfactory
{
    /// <summary>
    /// Describes and configures one Stationeers asset creation workflow.
    ///
    /// The shared factory uses the metadata to generate/resolve the component script.
    /// The concrete constructor owns the final GameObject setup.
    /// </summary>
    public interface IStationeersAssetConstructor
    {
        string Id { get; }
        string DefaultGameObjectName { get; }
        string GeneratedClassName { get; }
        string BaseClassName { get; }
        string AddComponentMenuPath { get; }
        string ScriptPath { get; }

        void ConfigureGameObject(GameObject gameObject, Type generatedType);
    }
}
