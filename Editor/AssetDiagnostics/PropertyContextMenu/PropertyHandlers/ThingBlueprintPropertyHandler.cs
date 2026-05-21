using Assets.Scripts.Objects;
using Assets.Scripts.UI;
using ilodev.stationeers.moddingtools.diagnostics;
using ilodev.stationeers.moddingtools.uihelpers;
using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace ilodev.stationeersmodding.tools.diagnostics
{
    [InitializeOnLoad]
    public class ThingBlueprintPropertyHandler : IPropertyContextMenuHandler
    {
        private const string PendingThingInstanceIdKey =
            "ilodev.stationeersmodding.tools.diagnostics.PendingBlueprintThingInstanceId";

        static ThingBlueprintPropertyHandler()
        {
            if (SessionState.GetInt(PendingThingInstanceIdKey, 0) != 0)
            {
                SubscribeToEditorUpdate();
            }
        }

        public void Register(PropertyContextMenuRegistry registry)
        {
            registry.RegisterHandler("Blueprint", (menu, property, target) =>
            {
                Thing thing = (Thing)target;

                menu.AddItem(new GUIContent("Generate blueprint prefab"), false, () =>
                {
                    GameObject prefab = BuildBlueprintPrefab(thing, true);

                    if (prefab == null)
                    {
                        return;
                    }

                    thing.Blueprint = prefab;
                    EditorUtility.SetDirty(thing);
                });
            });
        }

        private static GameObject BuildBlueprintPrefab(Thing thing, bool createWireframeIfMissing)
        {
            if (thing == null)
            {
                Debug.LogError("Cannot generate blueprint prefab: Thing is null.");
                return null;
            }

            Type wireframeType = ResolveWireframeType();

            if (wireframeType == null)
            {
                if (!createWireframeIfMissing)
                {
                    Debug.LogError("Cannot generate blueprint prefab: Wireframe type still does not exist after compilation.");
                    return null;
                }

                if (!CreateWireframeScriptIfMissing())
                {
                    return null;
                }

                SessionState.SetInt(PendingThingInstanceIdKey, thing.GetInstanceID());
                SubscribeToEditorUpdate();

                Debug.Log("Created missing Wireframe script. Waiting for Unity to compile before generating blueprint prefab.");
                return null;
            }

            GameObject blueprintGO = new GameObject(thing.name + "_Blueprint");

            MeshFilter filter = blueprintGO.AddComponent<MeshFilter>();
            MeshRenderer renderer = blueprintGO.AddComponent<MeshRenderer>();

            Wireframe wireframe = blueprintGO.AddComponent(wireframeType) as Wireframe;

            if (wireframe == null)
            {
                Debug.LogError("Resolved Wireframe type is not assignable to Assets.Scripts.UI.Wireframe.");
                GameObject.DestroyImmediate(blueprintGO);
                return null;
            }

            wireframe.BlueprintTransform = thing.transform;
            wireframe.BlueprintMeshFilter = filter;
            wireframe.BlueprintRenderer = renderer;

            WireFrameInspector.GenerateWireframeEdgesAndCombinedMesh(wireframe, thing.transform);
            WireFrameInspector.SaveMeshFromObject(wireframe, "Meshes", blueprintGO.name, false);

            string blueprintFolder = "Assets/Blueprints";

            if (!AssetDatabase.IsValidFolder(blueprintFolder))
            {
                AssetDatabase.CreateFolder("Assets", "Blueprints");
            }

            string prefabPath = Path.Combine(blueprintFolder, blueprintGO.name + ".prefab");
            prefabPath = prefabPath.Replace("\\", "/");
            prefabPath = AssetDatabase.GenerateUniqueAssetPath(prefabPath);

            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(blueprintGO, prefabPath);

            Debug.Log("Saved blueprint prefab to: " + prefabPath);

            GameObject.DestroyImmediate(blueprintGO);

            return prefab;
        }

        private static void OnEditorUpdate()
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                return;
            }

            int thingInstanceId = SessionState.GetInt(PendingThingInstanceIdKey, 0);

            if (thingInstanceId == 0)
            {
                UnsubscribeFromEditorUpdate();
                return;
            }

            Thing thing = EditorUtility.InstanceIDToObject(thingInstanceId) as Thing;

            if (thing == null)
            {
                Debug.LogError("Cannot finish blueprint prefab generation: pending Thing no longer exists.");
                SessionState.EraseInt(PendingThingInstanceIdKey);
                UnsubscribeFromEditorUpdate();
                return;
            }

            GameObject prefab = BuildBlueprintPrefab(thing, false);

            if (prefab == null)
            {
                SessionState.EraseInt(PendingThingInstanceIdKey);
                UnsubscribeFromEditorUpdate();
                return;
            }

            thing.Blueprint = prefab;
            EditorUtility.SetDirty(thing);

            SessionState.EraseInt(PendingThingInstanceIdKey);
            UnsubscribeFromEditorUpdate();
        }

        private static void SubscribeToEditorUpdate()
        {
            EditorApplication.update -= OnEditorUpdate;
            EditorApplication.update += OnEditorUpdate;
        }

        private static void UnsubscribeFromEditorUpdate()
        {
            EditorApplication.update -= OnEditorUpdate;
        }

        private static Type ResolveWireframeType()
        {
            string namespaceName = FindFirstAsmdefNamespace();

            if (string.IsNullOrEmpty(namespaceName))
            {
                Debug.LogError("Cannot resolve Wireframe type: no asmdef rootNamespace found.");
                return null;
            }

            string fullTypeName = namespaceName + ".Wireframe";

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type type = assembly.GetType(fullTypeName);

                if (type != null && typeof(Wireframe).IsAssignableFrom(type))
                {
                    return type;
                }
            }

            return null;
        }

        private static bool CreateWireframeScriptIfMissing()
        {
            string namespaceName;
            string folderPath;

            if (!FindFirstAsmdefNamespaceAndFolder(out namespaceName, out folderPath))
            {
                Debug.LogError("Cannot create Wireframe script: no asmdef with rootNamespace was found.");
                return false;
            }

            string scriptPath = Path.Combine("Assets/Scripts/UI/Wireframe.cs").Replace("\\", "/");

            if (File.Exists(scriptPath))
            {
                Debug.LogError(
                    "Wireframe.cs already exists, but the type could not be resolved: " +
                    scriptPath);

                return false;
            }

            string content =
$@"using UnityEngine;

namespace {namespaceName}
{{
    [AddComponentMenu(""Stationeers/UI/Wireframe"")]
    public class Wireframe : Assets.Scripts.UI.Wireframe
    {{
    // DO NOT EDIT THIS FILE. CREATE A NEW CLASS IF YOU NEED CUSTOM LOGIC ATTACHED TO YOUR ASSET.
    }}
}}
";
            // Make sure path exists
            string directory = Path.GetDirectoryName(scriptPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(scriptPath, NormalizeLineEndings(content));
            AssetDatabase.ImportAsset(scriptPath);
            AssetDatabase.Refresh();

            return true;
        }

        private static string FindFirstAsmdefNamespace()
        {
            string namespaceName;
            string folderPath;

            if (FindFirstAsmdefNamespaceAndFolder(out namespaceName, out folderPath))
            {
                return namespaceName;
            }

            return null;
        }

        private static bool FindFirstAsmdefNamespaceAndFolder(
            out string namespaceName,
            out string folderPath)
        {
            namespaceName = null;
            folderPath = null;

            try
            {
                string[] guids = AssetDatabase.FindAssets(
                    "t:AssemblyDefinitionAsset",
                    new[] { "Assets" });

                foreach (string guid in guids)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);

                    if (path.Contains("/Editor/"))
                    {
                        continue;
                    }

                    string json = File.ReadAllText(path);
                    string rootNamespace = GetRootNamespaceFromJson(json);

                    if (string.IsNullOrEmpty(rootNamespace))
                    {
                        continue;
                    }

                    namespaceName = rootNamespace;
                    folderPath = Path.GetDirectoryName(path).Replace("\\", "/");

                    return true;
                }
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }

            return false;
        }

        private static string GetRootNamespaceFromJson(string json)
        {
            const string key = "\"rootNamespace\":";
            int index = json.IndexOf(key);

            if (index >= 0)
            {
                int startIndex = index + key.Length;
                int endIndex = json.IndexOf(",", startIndex);

                if (endIndex == -1)
                {
                    endIndex = json.IndexOf("}", startIndex);
                }

                if (endIndex > startIndex)
                {
                    return json
                        .Substring(startIndex, endIndex - startIndex)
                        .Trim()
                        .Trim('"');
                }
            }

            return null;
        }

        private static string NormalizeLineEndings(string text)
        {
            return text
                .Replace("\r\n", "\n")
                .Replace("\r", "\n")
                .Replace("\n", Environment.NewLine);
        }
    }
}