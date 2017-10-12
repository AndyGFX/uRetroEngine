
r = 0;


function _start()
	Console:Print("Start ...")

	Text:SetFont(0,0,7)
	Text:SetFont(1,16*8,7)
end

function _scanline( line )
	
end

function _update( deltatime )
	-- body

	r=r+1;
	if r>64 then
		r = 0;
	end

end

function _draw( ... )

	Display:Clear(3)

	Text:Font(0)
	Text:Draw(10,30,"Draw FONT 0  ...",5)	

	Text:Font(1)
	Text:Draw(10,40,"Draw FONT 1  ...")

	Text:Font(0)
	Text:Draw(10,50,"Draw FONT 0  ...",5,6)	

	Text:Font(1)
	Text:Draw(10,60,"Draw FONT 1  ...",6,7)
	
	Text:Font(0)
	Text:Draw(10,10,"Text API test ...",2)
	Display:Flip()	
end
function _close( ... )
	-- body
end