# AssetsFactory design notes

## Purpose

AssetsFactory is a Unity Editor-only helper for creating Stationeers mod assets. A menu item should:

1. Generate a minimal component script under `Assets/Scripts/`.
2. Let Unity compile that script.
3. After compilation/domain reload, create a configured `GameObject`.
4. Attach mesh components, the generated Stationeers component, and the default stackable interactables.
5. Select the new object so the user can continue editing it.

The idea is that these assets are not just an empty script, but mostly functional items, e.g. including pre-defined interactables, slots, etc.

### `StationeersAssetDefinition`

A data-only description of an asset type:

- definition id
- menu name
- default object name
- generated class name
- Stationeers base class
- `AddComponentMenu` path
- generated script path
- whether default stackable interactables are added

Add new supported assets by registering another definition in `StationeersAssetDefinitions` and adding a small menu wrapper, or optionally move this to its own class.

### `StationeersAssetFactory`

The shared workflow:

1. Discover the current asmdef root namespace and assembly name.
2. Generate the minimal script.
3. Save one pending request to `EditorPrefs`.
4. Subscribe to `EditorApplication.update`.
5. After Unity finishes compiling/updating, resolve the generated component type.
6. Create and configure the `GameObject`.
7. Clear the pending request only after successful creation.

If the generated type is not available yet, the pending request is kept instead of failing. This makes the workflow more tolerant of Unity compilation timing.

### `StationeersTypeResolver`

Resolves the generated component type using multiple strategies:

1. Preferred assembly-qualified name from the asmdef.
2. Unqualified type name.
3. Scan loaded assemblies for the full type name.

It also verifies that the resolved type is a valid Unity `Component` before it can be attached.

### Menu wrappers

`CreateConstructor`, `CreateMultiConstructor`, and `CreateDynamicThingConstructor` are tiny wrappers that call the shared factory.

### `InteractableHelpers`

Now returns `bool` and handles null targets safely. This prevents a missing or unresolved generated component from turning into a hard null-reference failure.

## Files changed or added

- `CreateConstructor.cs`: menu wrapper.
- `CreateMultiConstructor.cs`: menu wrapper.
- `CreateDynamicThingConstructor.cs`: menu wrapper.
- `Core/StationeersAssetDefinition.cs`: central registry and asset descriptions.
- `Core/StationeersAssetFactory.cs`: shared creation workflow.
- `Core/StationeersTypeResolver.cs`: component type resolution.
- `Interactables/InteractableHelpers.cs`: safe interactable creation.
- `Utils/FileUtils.cs`: script generation and import behavior.

## How to add a new asset type

1. Add a new `StationeersAssetDefinition` in `StationeersAssetDefinitions`.
2. Add a menu wrapper class with a `[MenuItem]` attribute.
3. Call `StationeersAssetFactory.CreateAsset(StationeersAssetDefinitions.YourDefinition)`.

Example:

```csharp
public static readonly StationeersAssetDefinition Example = new StationeersAssetDefinition(
    "Example",
    "Assets/Create/Stationeers/Examples/Example", // Not used for now
    "NewExampleAsset",
    "Example",
    "Assets.Scripts.Objects.ExampleBase",
    "Stationeers/Examples/Example",
    CreatedScriptsPath + "Example.cs");
```

```csharp
[MenuItem("Assets/Create/Stationeers/Examples/Example", false, 1)]
public static void CreateExampleAsset()
{
    StationeersAssetFactory.CreateAsset(StationeersAssetDefinitions.Example);
}
```

## Reliability notes

- The pending creation request survives Unity domain reload through `EditorPrefs`.
- The editor update callback is de-duplicated before it is added.
- The pending request is only cleared after the generated type is found and the object is created.
- If the generated component does not derive from `Component`, it is rejected.
- Default interactables are only added when the final object actually has a `Thing` component.

## Manual test checklist (until it is moved to proper testing)

1. Import the tool into a Unity project with Stationeers assemblies available.
2. Use `Assets/Create/Stationeers/QuickItems/Constructor`.
3. Confirm `Assets/Scripts/Objects/Constructor.cs` is generated.
4. Wait for Unity compilation to finish.
5. Confirm a `NewConstructorAsset` GameObject is created and selected.
6. Confirm it has `MeshRenderer`, `MeshFilter`, `MeshCollider`, and the generated `Constructor` component.
7. Confirm the `SplitOne` and `SplitHalf` interactables are present.
8. Repeat for Multi Constructor and DynamicThing Constructor.
9. Delete the generated script and repeat to verify script regeneration still works.
10. Force a domain reload during compilation and confirm the pending request still completes.
