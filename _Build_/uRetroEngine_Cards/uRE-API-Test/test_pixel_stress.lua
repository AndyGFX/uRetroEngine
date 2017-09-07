

sprites = {}
count = 2500
function _start()
	Console:Print("Start ...")

	--Display:TargetFrameRate(30)
	Text:SetFont(0,0,7)
	Text:SetFont(1,16*7,7)

	
	
end

function _scanline( line )
	
end

function _update( deltatime )
	Utils:CodeProfilerStart("Update()")
	-- body	
	Utils:CodeProfilerEnd("Update()")
end

function _draw( ... )

	

	Utils:CodeProfilerStart("Clear()")
	Display:Clear(3)
	Utils:CodeProfilerEnd("Clear()")

	Utils:CodeProfilerStart("Pixels")
	for x=0,Display:Width() do
		for y=0,Display:Height() do
			Graphics:PutPixel(x,y,math.random(1,31))
		end
	end
	Utils:CodeProfilerEnd("Pixels")

	Text:Font(0)
	Text:Draw(10,10,"Sprite API test ...",2)

	Utils:CodeProfilerStart("Flip()")
	Display:Flip()	
	Utils:CodeProfilerEnd("Flip()")

	
end
function _close( ... )
	-- body
end