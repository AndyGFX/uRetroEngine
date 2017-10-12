CubesTeam
o.kollar@gmail.com

# uRetroEngine

Unity retro game framework with lua support

FEATURES:
-------------

- user defined screen size (limitation is only performance)
- defined screen origin (top-left / bottom-left)
- load color palette from image (default 16)
- load sprite sheet from image (limitation is only memory)
- load font from image (limitation is only memory)
- create tilemap
- for scripting is used NLua for unity
- you can import module/dll in main.lua code (UnityEngine included)
- command line argents
- build-in console with commands to manage cartridge
- build-in code profiler (user defined start/end places, more in wiki) 
- you can create retro game and build to PC/MAC/Linux/Androis or Mac using uRetro classes
- or create universal player and via lua script create game outside of unity

Installation instruction:
------------------------------

1) Open Build project/ Player setting and put this

UNITY_3D;USE_KERALUA;LUA_CORE;CATCH_EXCEPTIONS;NLUA

to Scripting define symbols*

Note: all concele errors are now removed.


2) INSTALL external data for working with examples:
- unzip uRetroEngine_folders.rar from resources to project folder 

directory then looks like this:

..
_Build_
Assets
Library
ProjectSettings
uRetroEngine_Cards
uRetroEngine_Game
uRetroEngine_Libs

3) Open one from examples and PLAY.


BUILD:
-------------

1) Open scene uRe-Runner-LUA\Scenes\Test-uRetroEngine-LUA.unity
2) Select scene object uRetroEngineCanvas-LUA
3) Press button [Build RetroEngine] from inspector on uRetroEngine_Runner_LUA component
4) Optional: Press button [Move cartridges] from inspector when you made any changes from editor



EXECUTE:
------------

> Start game from expanded cartridge (from folder):

	uRetroEngine.exe -game LuaGame -resolution 1280 800 false

	*- load folder LuaGame and set resulution*

    > Start game from cartridge only:

	uRetroEngine.exe -cartridge LuaGame -resolution 1280 800 false
	
	*- load cartridge and run game in defined resolution*

> Start engine without game

	uRetroEngine.exe

	*- press F1*
	*- press enter and write to console cmd help*

	
> Create new cartridge folder tmeplate:

	uRetroEngine.exe -create LuaGame
	
	*- create cartridge folder template*


