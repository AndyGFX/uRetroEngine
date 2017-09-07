
Library:Require("class.lua")
Library:Require("TCellular.lua")

cave = class(TCellular);

function OnStart()

	Text:SetFont(0,0,6)
	Text:SetFont(1,16*7,6)

	cave:Generate(128,64,0.6,3,5,5);
	cave:Dump()
end

function OnUpdate(deltaTime)
	Display:Clear(1)

	DrawGameTitle()	

	cave:PreviewMap(0,0)
		
	Display:Flip()	
end

function OnScanline(line)

end

function OnClose()
end

------------------------------------------------------------------------------

function DrawGameTitle()
	Graphics:DrawRectangle(0,Display:Height(),Display:Width(),Display:Height()-9,2,true)
	Text:Font(0)
	Text:Draw(10,Display:Height()-8,"Survixel The Game",20)
end

		        