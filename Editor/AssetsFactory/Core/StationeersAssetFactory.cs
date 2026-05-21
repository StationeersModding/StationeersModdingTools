using System;
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
            public string ConstructorTypeName;
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

        public static void CreateAsset(IStationeersAssetConstructor constructor)
        {
            if (constructor == null)
            {
                Debug.LogError("Cannot create Stationeers asset: constructor is null.");
                return;
            }

            string namespaceName = AssemblyDefinitionHelpers.FindAsmdefNamespace();
            string assemblyName = AssemblyDefinitionHelpers.FindAsmdefAssemblyName();

            SavePendingRequest(new PendingRequest
            {
                ConstructorTypeName = constructor.GetType().AssemblyQualifiedName,
                NamespaceName = namespaceName,
                AssemblyName = assemblyName
            });

            if (TypeUtils.NamespaceComponentExists(namespaceName, constructor.GeneratedClassName))
            {
                Debug.Log(
                    $"Component already exists, continuing asset creation: {namespaceName}.{constructor.GeneratedClassName}");

                TryCompletePendingRequest();
                return;
            }

            string content = FileUtils.GenerateScript(
                constructor.GeneratedClassName,
                constructor.BaseClassName,
                constructor.AddComponentMenuPath,
                namespaceName);

            FileUtils.CreateTextFile(constructor.ScriptPath, content, true);

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

            IStationeersAssetConstructor constructor = CreateConstructorFromPendingRequest(request);
            if (constructor == null)
            {
                ClearPendingRequest();
                EditorApplication.update -= TryCompletePendingRequest;
                return;
            }

            Type generatedType = StationeersTypeResolver.ResolveComponentType(
                request.NamespaceName,
                constructor.GeneratedClassName,
                request.AssemblyName);

            if (generatedType == null)
            {
                Debug.LogWarning(
                    "Waiting for generated Stationeers component type: " +
                    constructor.GeneratedClassName);

                return;
            }

            GameObject gameObject = CreateConfiguredGameObject(constructor, generatedType);
            Selection.activeObject = gameObject;

            ClearPendingRequest();
            EditorApplication.update -= TryCompletePendingRequest;
        }

        private static IStationeersAssetConstructor CreateConstructorFromPendingRequest(PendingRequest request)
        {
            Type constructorType = Type.GetType(request.ConstructorTypeName);

            if (constructorType == null)
            {
                Debug.LogError(
                    "Cannot complete Stationeers asset creation. Constructor type was not found: " +
                    request.ConstructorTypeName);
                return null;
            }

            if (!typeof(IStationeersAssetConstructor).IsAssignableFrom(constructorType))
            {
                Debug.LogError(
                    "Cannot complete Stationeers asset creation. Type is not an asset constructor: " +
                    constructorType.FullName);
                return null;
            }

            try
            {
                return (IStationeersAssetConstructor)Activator.CreateInstance(constructorType);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                return null;
            }
        }

        private static GameObject CreateConfiguredGameObject(
            IStationeersAssetConstructor constructor,
            Type generatedType)
        {
            GameObject gameObject = new GameObject(constructor.DefaultGameObjectName);
            Undo.RegisterCreatedObjectUndo(gameObject, "Create " + constructor.DefaultGameObjectName);

            constructor.ConfigureGameObject(gameObject, generatedType);

            EditorUtility.SetDirty(gameObject);
            return gameObject;
        }
    }
}
