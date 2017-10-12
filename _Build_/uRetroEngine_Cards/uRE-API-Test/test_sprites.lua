
pixels = {}

function _start()
	Console:Print("Start ...")

	Text:SetFont(0,0,6)
	Text:SetFont(1,16*7,6)


	pixels = Sprites:GetPixels(0);

end

function _scanline( line )
	
end

function _update( deltatime )
	-- body

end

function _draw( ... )

	Display:Clear(3)
	Display:PixelSize(1,1)
	-- normal
	Sprites:DrawSprite(0,32,32,false,false)
	Sprites:DrawSprite(1,40,32,false,false)

	-- X flipped
	Sprites:DrawSprite(0,32+32,32,true,false)
	Sprites:DrawSprite(1,32+40,32,true,false)

    -- Y flipped
	Sprites:DrawSprite(0,32+32,48,true,true)
	Sprites:DrawSprite(1,32+40,48,true,true)

	Display:PixelSize(2,2)
	-- X flipped
	Sprites:DrawSprite(0,32+32,50+32,true,false)
	Sprites:DrawSprite(1,32+40,50+32,true,false)

    -- Y flipped
	Sprites:DrawSprite(0,32+32,50+48,true,true)
	Sprites:DrawSprite(1,32+40,50+48,true,true)

	-- draw pixels
	Graphics:DrawPixels(16,32,8,pixels)

	Text:Font(0)
	Text:Draw(10,10,"Sprites API test ...",1)
	Display:Flip()	
end
function _close( ... )
	-- body
end