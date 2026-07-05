# ThunderstoreModAssistant

Decompiled source of [ThunderstoreModAssistant](https://thunderstore.io/c/bonelab/p/notnotnotswipez/ThunderstoreModAssistant/) v1.2.1 by notnotnotswipez.

A BONELAB mod to assist in downloading code mods off Thunderstore in-game.

## Origin

Source decompiled from the published DLLs via Thunderstore's source page:

- **Mods/ThunderstoreModAssistant.dll** → Main mod logic (mod listing, install/uninstall, thumbnail loading, blacklist, UI)
- **Plugins/ThunderstoreModAssistantPlugin.dll** → Plugin bootstrap and update handling

## Contents

| File | Description |
|------|-------------|
| `Mods/ThunderstoreModAssistant.dll.cs` | Main mod (`MelonMod` with `MelonInfo("ThunderstoreModAssistant", "1.2.1")`) |
| `Plugins/ThunderstoreModAssistantPlugin.dll.cs` | Plugin (`MelonMod` with `MelonInfo("ThunderstoreModAssistantPlugin", "1.0.0")` and `MelonPriority(-20000)`) |

## License

Original mod by [notnotnotswipez](https://github.com/notnotnotswipez). Decompiled source provided by Thunderstore.
