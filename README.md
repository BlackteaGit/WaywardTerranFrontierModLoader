[Modding Tutorial](https://github.com/BlackteaGit/WaywardTerranFrontierModLoader/wiki/A-Quick-Introduction-To-DLL-Modding) | [Current Release](https://github.com/BlackteaGit/WaywardTerranFrontierModLoader/releases) | [Wiki] (https://github.com/BlackteaGit/WaywardTerranFrontierModLoader/wiki)
# WaywardTerranFrontierModLoader

A tool to enable loading multiple mods into the game Wayward Terran Frontier: Zero Falls. This project is forked from Phoenix Point Mod Loader
(https://github.com/Ijwu/PhoenixPointModLoader).

# How it works

On the first run, the loader will create an additional folder `Mods` in game root directory (for the option to load non-steam mods and for saving settings/log file).
The mod manager will load all found .dll files from `Mods` folder, `\steamapps\workshop\content\392080` folder and any included subdirectories into the game. Usually those .dll files will be utilizing Harmony library.
Harmony ( https://github.com/pardeike/Harmony ) is a tool developed for modders, which enables the ability to patch game methods at runtime without changing the game files.
Additionaly all dll mods manged by the loader can access each other's code via the loader by using reflection.
Basic management options like mod version check, disabling certain mods and conflict resolution are also provided.
Optional use of https://github.com/jbevain/cecil to inject a method call into WTF.exe, which will create `Mods` directory in the game root folder and load the mod manager.

# Installation

**Steam method:** (requires game 0.9 dev build 22 or later, the latest WTFML build, steam client must be running)

Go to the steam workshop mod page: https://steamcommunity.com/workshop/filedetails/?id=2464675377 ,
subscribe to to the workshop item, the game will load and run it on the next start.
Run the game once to create `Mods` folder, place your local mods there. The loader will also run any downloaded subscribed mods from steam workshop directory.
The steam version no longer contains `WTFModLoaderInjector.exe`, does not modify any of your game files and will be automatically updated by the steam client.

**Local method:** (requires game 0.9 dev build 22 or later, the latest WTFML build, will work without running steam client)

Unpack the contents of the .zip file into the `Extensions` folder of your game root directory.
The game will load it automatically from there on the next start.
Run the game once to create `Mods` folder, place your local mods there. The loader will also run any downloaded subscribed mods from steam workshop directory.

**Legacy method:** (required by pre 0.9 game builds, required for WTFML builds v0.1/v0.2, will also work for any later WTFML builds and game versions )

Place the contents of the .zip file in your game root directory. Run the executable titled `WTFModLoaderInjector.exe`.
Run the game to have it generate the `Mods` folder for the Mod Loader. Place all mod directories/files into this folder.
You will have to reinstall the loader by running `WTFModLoaderInjector.exe` with every new game patch if you want to continue using this loading method.

**IMPORTANT NOTICE:** 

If you are using v0.1 or v0.2 non-steam WTFML build from github releases loaded via `WTFModLoaderInjector.exe`, make sure that you restore WTF.exe to vanilla before subscribing to the current steam WTFML build or placing the loader files into `Extensions` folder, or the game will run both versions at once and crash.
You can restore your vanilla game files via steam game file integrity verification check or by renaming `WTF.exe.orig` file from the game root directory back to `WTF.exe`.

Please report any game bugs with installed mod loader to me first, or make sure that you can reproduce them in vanilla game before reporting them to WTF devs.

# Contribute

You are welcome to contribute to this project in any way, please use the "development" branch for development.
This project uses game librarys `WTF.exe` and `MonoGame.Framework.dll` which you will have to reference if building this project from quellcode.

# Development To-Do List

- [x] More robust mod metadata. (e.g. game version compatibility, mod conflicts, etc.)
- [x] More robust mod loading settings. (e.g. disable certain mods)
- [x] Provide ingame GUI for settings.
- [ ] Display full mod conflict/dependencies info in GUI. (a list of conflicting/dependant mods)
- [ ] More management options (e.g. copy steam mods to local, custom mod lists, delete loc. mods, unsub steam mods, etc.)
- [ ] Custom ingame Steam upload UI.
- [ ] Manage game texture loading from Steam workshop. (e.g. conflict resolution)
- [ ] Provode mod configuration settings in GUI. (write mod specific config file if provided)
- [ ] CAS .NET framework implementation?

# Contact

 Feel free to contact me on Wayward Terran Frontier discord (https://discord.gg/MpjRsAp) using "@blacktea".
