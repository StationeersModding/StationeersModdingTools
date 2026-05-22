using Assets.Scripts.Objects;
using System;
using UnityEditor;
using UnityEngine;

namespace ilodev.stationeersmods.tools.assetsfactory
{
    /// <summary>
    /// Base implementation for assets that inherit from Structure, to share the same construction process.
    /// </summary>
    public abstract class StationeersAssetStructureBase : StationeersAssetConstructorBase
    {
        public override void ConfigureGameObject(GameObject gameObject, Type generatedType)
        {
            MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
            MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
            gameObject.AddComponent(generatedType);

            EditorUtility.SetDirty(gameObject);
        }

        public virtual void AddDefaultBuildState(GameObject gameObject, MeshRenderer meshRenderer = null)
        {
            Structure structure = gameObject.GetComponent<Structure>();
            BuildState defaultBuildState = new BuildState();
            defaultBuildState.BlockAir = true;
            defaultBuildState.BlockLight = true;
            defaultBuildState.BlockGravity = true;
            defaultBuildState.Visualizer = meshRenderer;
            defaultBuildState.RenderMode = BuildStateRenderMode.OnMineAndPreviousStates;
            structure.BuildStates = new System.Collections.Generic.List<BuildState>();
            structure.BuildStates.Add(defaultBuildState);
        }

        public virtual BoxCollider MakeBoxCollider(GameObject gameObject, Vector3 center, Vector3 size)
        {
            BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
            boxCollider.center = new Vector3(0, 0, 0.065f);
            boxCollider.size = new Vector3(2, 2, 0.13f);

            return boxCollider;
        }

        public virtual GameObject CreateBlockSound(GameObject gameObject, Vector3 center, Vector3 size)
        {
            GameObject blockSound = new GameObject("BlockSound");
            blockSound.transform.parent = gameObject.transform;
            blockSound.layer = LayerMask.NameToLayer("BlockSound");
            BoxCollider blockSoundCollider = blockSound.AddComponent<BoxCollider>();
            blockSoundCollider.center = center;
            blockSoundCollider.size = size;
            return blockSound;
        }


    }
}
