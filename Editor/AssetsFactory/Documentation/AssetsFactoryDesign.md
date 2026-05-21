# AssetsFactory design notes

## Purpose

AssetsFactory is a Unity Editor-only helper for creating Stationeers mod assets. A menu item should:

1. Generate a minimal runtime component script under `Assets/Scripts/` when the component class is missing.
2. Let Unity compile that script.
3. After compilation/domain reload, resolve the generated component type.
4. Create a configured `GameObject`.
5. Let the specific asset constructor perform its own setup.
6. Select the new object so the user can continue editing it.

## Current design

Each asset creation menu item is its own constructor class.

For example:

- `CreateConstructor`
- `CreateMultiConstructor`
- `CreateDynamicThingConstructor`
- `CreateFrame`
- `CreateWall`

Each class defines:

- generated class name
- Stationeers base class
- generated script path
- Add Component menu path
- default GameObject name
- asset-specific setup code

## Shared factory responsibility

`StationeersAssetFactory` only owns the shared workflow:

1. Discover the current asmdef root namespace and assembly name.
2. Save a pending request containing the asset constructor type.
3. Check whether the generated component already exists.
4. Generate the missing script if needed.
5. Wait for Unity to finish compiling/updating.
6. Recreate the asset constructor after domain reload.
7. Resolve the generated component type.
8. Create a GameObject and delegate setup to the asset constructor.

The factory should not know which assets need interactables, slots, custom render setup, or other special behavior.

## Asset constructor responsibility

Each asset constructor inherits from `StationeersAssetConstructorBase` and overrides metadata properties.

A simple asset can rely on the base setup:

```csharp
public sealed class CreateWall : StationeersAssetConstructorBase
{
    public override string Id => "Wall";
    public override string DefaultGameObjectName => "NewStructureWallAsset";
    public override string GeneratedClassName => "Wall";
    public override string BaseClassName => "Assets.Scripts.Objects.Wall";
    public override string AddComponentMenuPath => "Stationeers/Objects/Wall";
    public override string ScriptPath => "Assets/Scripts/Objects/Wall.cs";
}
```

An asset that needs special setup overrides `ConfigureGameObject`:

```csharp
public override void ConfigureGameObject(GameObject gameObject, Type generatedType)
{
    base.ConfigureGameObject(gameObject, generatedType);

    Thing thing = gameObject.GetComponent<Thing>();
    InteractableHelpers.AddInteractable(thing, "SplitOne", InteractableType.Button1);
    InteractableHelpers.AddInteractable(thing, "SplitHalf", InteractableType.Button2);
}
```


## Utility classes

- `FileUtils` handles script file creation, folder creation, line endings, and AssetDatabase refresh/import.
- `AssemblyDefinitionHelpers` finds the asmdef root namespace and assembly name.
- `StationeersTypeResolver` resolves generated component types after compile/domain reload.
- `TypeUtils` checks whether a fully-qualified component type already exists.
- `InteractableHelpers` adds Stationeers interactables safely.


## How to add a new asset type

1. Create a new class inheriting from `StationeersAssetConstructorBase`.
2. Add a `[MenuItem]` method that calls `StationeersAssetFactory.CreateAsset(new YourClass())`.
3. Override the metadata properties.
4. Override `ConfigureGameObject` only if the asset needs custom setup.

## Manual test checklist

1. Import the tool into a Unity project with Stationeers assemblies available.
2. Use each menu item under `Assets/Create/Stationeers/QuickItems`.
3. Confirm the generated script is created under `Assets/Scripts/` when missing.
4. Wait for Unity compilation to finish.
5. Confirm a GameObject is created and selected.
6. Confirm it has `MeshRenderer`, `MeshFilter`, `MeshCollider`, and the generated component.
7. Confirm only Constructor/MultiConstructor/DynamicThingConstructor receive the stackable interactables.
8. Confirm Frame and Wall do not receive those interactables unless their constructor class explicitly adds them.
9. Delete a generated script and repeat to verify script regeneration still works.
10. Force a domain reload during compilation and confirm the pending request still completes.
