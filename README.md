[Wiki](https://github.com/BlackteaGit/WaywardTerranFrontierModLoader/wiki) | [Current Release](https://github.com/BlackteaGit/WaywardTerranFrontierModLoader/releases)
# WaywardTerranFrontierModLoader

A tool to enable loading multiple mods into the game Wayward Terran Frontier: Zero Falls. This project is forked from Phoenix Point Mod Loader
(https://github.com/Ijwu/PhoenixPointModLoader).

# How it works

It uses https://github.com/jbevain/cecil to inject a method call into WTF.exe, which will create "Mods" directory in the game root folder and load the mod manager.
The mod manager will load all found .dll files from "Mods" folder, "\steamapps\workshop\content\392080" folder and any included subdirectories into the game. Usually those .dll files will be utilizing Harmony library (https://github.com/pardeike/Harmony) to patch game methods at run time.

# Installation

Place the contents of the .zip file in your game root directory. Run the executable titled `WTFModLoaderInjector.exe`.
Run the game to have it generate the `Mods` folder for the Mod Loader. Place all mod directories/files into this folder.

# Contribute

You are welcome to contribute to this project in any way, please use the "development" branch for development.
This project uses game librarys "WTF.exe" and "MonoGame.Framework.dll" which you will have to reference if building this project from quellcode.

# Contact

 Feel free to contact me on Wayward Terran Frontier discord (https://discord.gg/MpjRsAp) using "@blacktea".
