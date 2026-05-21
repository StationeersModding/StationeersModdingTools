using System;
using Assets.Scripts.Objects;
using UnityEditor;
using UnityEngine;

namespace ilodev.stationeersmods.tools.assetsfactory
{
    [InitializeOnLoad]
    public static class StationeersAssetFactory
    {
        private const string PendingRequestKey = "StationeersModdingTools_PendingAssetFactoryRequest";

        [Serializable]
        private sealed class PendingRequest
        {
            public string DefinitionId;
            public string NamespaceName;
            public string AssemblyName;
        }

        static StationeersAssetFactory()
        {
            if (HasPendingRequest())
            {
                SubscribeToEditorUpdate();
            }
        }

        public static void CreateAsset(StationeersAssetDefinition definition)
        {
            if (definition == null)
            {
                Debug.LogError("Cannot create Stationeers asset: definition is null.");
                return;
            }

            string namespaceName = AssemblyDefinitionHelpers.FindAsmdefNamespace();
            string assemblyName = AssemblyDefinitionHelpers.FindAsmdefAssemblyName();

            SavePendingRequest(new PendingRequest
            {
                DefinitionId = definition.Id,
                NamespaceName = namespaceName,
                AssemblyName = assemblyName
            });

            if (TypeUtils.NamespaceComponentExists(namespaceName, definition.GeneratedClassName))
            {
                Debug.Log(
                    $"Component already exists, continuing asset creation: {namespaceName}.{definition.GeneratedClassName}");

                TryCompletePendingRequest();
                return;
            }

            string content = FileUtils.GenerateScript(
                definition.GeneratedClassName,
                definition.BaseClassName,
                definition.AddComponentMenuPath,
                namespaceName);

            FileUtils.CreateTextFile(definition.ScriptPath, content, true);

            SubscribeToEditorUpdate();
        }

        private static void SubscribeToEditorUpdate()
        {
            EditorApplication.update -= TryCompletePendingRequest;
            EditorApplication.update += TryCompletePendingRequest;
        }

        private static bool HasPendingRequest()
        {
            return !string.IsNullOrEmpty(EditorPrefs.GetString(PendingRequestKey, string.Empty));
        }

        private static void SavePendingRequest(PendingRequest request)
        {
            EditorPrefs.SetString(PendingRequestKey, JsonUtility.ToJson(request));
        }

        private static PendingRequest LoadPendingRequest()
        {
            string json = EditorPrefs.GetString(PendingRequestKey, string.Empty);
            return string.IsNullOrEmpty(json) ? null : JsonUtility.FromJson<PendingRequest>(json);
        }

        private static void ClearPendingRequest()
        {
            EditorPrefs.DeleteKey(PendingRequestKey);
        }

        private static void TryCompletePendingRequest()
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                return;
            }

            PendingRequest request = LoadPendingRequest();
            if (request == null)
            {
                EditorApplication.update -= TryCompletePendingRequest;
                return;
            }

            StationeersAssetDefinition definition =
                StationeersAssetDefinitions.FindById(request.DefinitionId);

            if (definition == null)
            {
                Debug.LogError(
                    "Cannot complete Stationeers asset creation. Unknown definition id: " +
                    request.DefinitionId);

                ClearPendingRequest();
                EditorApplication.update -= TryCompletePendingRequest;
                return;
            }

            Type generatedType = StationeersTypeResolver.ResolveComponentType(
                request.NamespaceName,
                definition.GeneratedClassName,
                request.AssemblyName);

            if (generatedType == null)
            {
                Debug.LogWarning(
                    "Waiting for generated Stationeers component type: " +
                    definition.GeneratedClassName);

                return;
            }

            GameObject gameObject = CreateConfiguredGameObject(definition, generatedType);
            Selection.activeObject = gameObject;

            ClearPendingRequest();
            EditorApplication.update -= TryCompletePendingRequest;
        }

        private static GameObject CreateConfiguredGameObject(
            StationeersAssetDefinition definition,
            Type generatedType)
        {
            GameObject gameObject = new GameObject(definition.DefaultGameObjectName);
            Undo.RegisterCreatedObjectUndo(gameObject, "Create " + definition.DefaultGameObjectName);

            gameObject.AddComponent<MeshRenderer>();
            gameObject.AddComponent<MeshFilter>();
            gameObject.AddComponent<MeshCollider>();
            gameObject.AddComponent(generatedType);

            if (definition.AddDefaultStackableInteractables)
            {
                AddDefaultStackableInteractables(gameObject);
            }

            EditorUtility.SetDirty(gameObject);
            return gameObject;
        }

        private static void AddDefaultStackableInteractables(GameObject gameObject)
        {
            Thing thing = gameObject.GetComponent<Thing>();
            if (thing == null)
            {
                Debug.LogWarning(
                    "Created asset does not have a Thing component. Default interactables were not added.");

                return;
            }

            InteractableHelpers.AddInteractable(thing, "SplitOne", InteractableType.Button1);
            InteractableHelpers.AddInteractable(thing, "SplitHalf", InteractableType.Button2);
        }
    }
}