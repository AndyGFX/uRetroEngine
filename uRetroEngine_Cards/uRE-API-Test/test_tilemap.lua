xx = 0
yy = 0


function _scanline( line )
	
end

function _start()


	Console:Print("Import start ...")

	-- Test #1: Import from external file and build tilemap data
	-- define tile substitution to flag

	FLAGS[FLAG_FREE] = 0;
    FLAGS[FLAG_WALL] = 6;
    FLAGS[FLAG_OBSTACLE] = 0;
    FLAGS[FLAG_LADDER] = 7;
    FLAGS[FLAG_HAZARD] = 0;
    FLAGS[FLAG_CHEST] = 0;
    FLAGS[FLAG_ITEM] = 0;
    FLAGS[FLAG_COIN] = 0;
    FLAGS[FLAG_LIFE] = 0;

	
	-- import tilemap layers 
    Console:Print("Load tilemap ...")	
	Tilemap:ImportTilemap("RetroTilemap.json")
	Console:Print("... end")	

	
	-- import collision map and convert to tile flags by FLAGS table
	Console:Print("Load collision tilemap ...")	
	Tilemap:ImportCollisionMap("RetroTilemapCol.json",FLAGS,1)
	Console:Print("... end")	

	
	-- save tilemap
	Console:Print("Save tilemap changes...")	
	-- save tilemap data to master file (name is defiend in config)
	Tilemap:Save()
	Console:Print("... end")	
	Console:Print("END")
	
--[[	
	-- test #2: tilemap loaded automaticaly from uRE_Tilemap.json

	w = Tilemap:Width()
	h = Tilemap:Height();

	Console:Print("Tilemap W: "..tostring(w).." H: "..tostring(h))	
]]
	

end

function _draw( ... )
	Display:Clear();

	Tilemap:DrawTilemap(0,0,64,32,xx,yy)

	
	Text:Draw(0,8,"Scroll X: "..xx)
	Text:Draw(0,0,"Scroll Y: "..yy)

	Text:Draw(0,40,"Screen W: "..Display:Width())
	Text:Draw(0,32,"Screen H: "..Display:Height())

	
	Display:Flip();
end

function _update( deltatime )


    if Input:ButtonPressed(KEY_LEFT) then xx=xx-1 end
    if Input:ButtonPressed(KEY_RIGHT) then xx=xx+1 end
    if Input:ButtonPressed(KEY_UP) then yy=yy+1 end
    if Input:ButtonPressed(KEY_DOWN) then yy=yy-1 end


end

function _close( ... )
	-- body
end
		        