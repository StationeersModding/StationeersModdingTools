# Stationeers Visualizers

Editor-only Scene view helpers for inspecting `Thing`, `Structure`, and `SmallGrid` bounds, slots, interactables, OpenEnds, and occupied cells.

This package is part of `stationeers.modding.tools`.

## How to add a custom visualizer

1. Implement `IThingVisualizer` in an editor assembly.
2. Give it a unique `ToggleName`; that value is used as its `EditorPrefs` unique key.
3. Return early from `OnSceneGUI` when `target` is not a supported type.
4. Put persistent controls in `OnPreferencesGUI` and use `VisualizerPreferencesUtil` so values save, repaint, and reset.
5. Use `VisualizerDrawUtil` rather than duplicating helper functions like label, handle-matrix, cube, or snap logic.

