
--Library:Include("test_scanline.lua")
--Library:Include("test_graphics.lua")
--Library:Include("test_text.lua")
Library:Include("test_tilemap.lua")
--Library:Include("test_stress.lua")
--Library:Include("test_sprites.lua")

function OnStart()
	_start()
end

function OnScanline(line)
	_scanline(line)
end

function OnUpdate(deltaTime)
	_update(deltaTime)
	_draw()
end


function OnClose()
	_close()
end
		        