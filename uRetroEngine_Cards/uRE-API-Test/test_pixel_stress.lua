

sprites = {}
count = 2500

W = Display:Width()
H = Display:Height()

function _start()
	Console:Print("Start ...")

	--Display:TargetFrameRate(30)
	Text:SetFont(0,0,7)
	Text:SetFont(1,16*7,7)

	--Display:PixelSize(2,2);
	
	
end

function _scanline( line )
	
end

function _update( deltatime )
	Utils:CodeProfilerStart("Update()")
	-- body	
	Utils:CodeProfilerEnd("Update()")
end

function _draw( ... )

	

	--Utils:CodeProfilerStart("Clear()")
	Display:Clear(3)
	--Utils:CodeProfilerEnd("Clear()")

	
	for x=0,W do
		for y=0,H do
			--Utils:CodeProfilerStart("Pixels")
			Graphics:PutPixel(x,y,math.random(1,31))
			--Utils:CodeProfilerEnd("Pixels")
		end
	end
	

	Text:Font(0)
	Text:Draw(10,10,"Sprite API test ...",2)

	--Utils:CodeProfilerStart("Flip()")
	Display:Flip()	
	--Utils:CodeProfilerEnd("Flip()")

	
end
function _close( ... )
	-- body
end