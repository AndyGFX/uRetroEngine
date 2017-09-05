

sprites = {}
count = 2500
function _start()
	Console:Print("Start ...")

	--Display:TargetFrameRate(30)
	Text:SetFont(0,0,7)
	Text:SetFont(1,16*7,7)

	
	for i=1,count do
		spr = {}
		spr.id = math.random(1,7)
		spr.x = math.random(32,255-32)
		spr.y = math.random(32,240-32)
		spr.dx = math.sin(math.random(-math.pi,math.pi))
		--if (dx>0) then spr.dx=1 end
		--if (dx<=0) then spr.dx=-1 end
		
		--dy = math.random()-0.5
		--if (dy>0) then spr.dy=1 end
		--if (dy<=0) then spr.dy=-1 end
		spr.dy = math.sin(math.random(-math.pi,math.pi))
		table.insert(sprites,spr)
	end
end

function _scanline( line )
	
end

function _update( deltatime )
	Utils:CodeProfilerStart("Update()")
	-- body
	for i,spr in ipairs(sprites) do
		
		spr.x = spr.x + spr.dx
		spr.y = spr.y + spr.dy
		if (spr.x>256 or spr.x<0) then 
			spr.dx=-spr.dx 
		end
		if (spr.y>256 or spr.y<0) then 
			spr.dy=-spr.dy 
		end
		
	end
	Utils:CodeProfilerEnd("Update()")
end

function _draw( ... )

	Utils:CodeProfilerStart("Draw()")

	Utils:CodeProfilerStart("Clear()")
	Display:Clear(3)
	Utils:CodeProfilerEnd("Clear()")

	Utils:CodeProfilerStart("Sprites")
	for i,spr in ipairs(sprites) do
		Sprites:DrawSprite(spr.id,spr.x,spr.y,false,false)
	end
	Utils:CodeProfilerEnd("Sprites")

	Text:Font(0)
	Text:Draw(10,10,"Sprite API test ...",2)

	Utils:CodeProfilerStart("Flip()")
	Display:Flip()	
	Utils:CodeProfilerEnd("Flip()")

	Utils:CodeProfilerEnd("Draw()")
end
function _close( ... )
	-- body
end