
r = 0;


function _start()
	Console:Print("Start ...")
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

	-- bottom-left
	Graphics:DrawCircle(0,0,32,5,true)
	Graphics:DrawRectangle(32,32,64,64,8,true)
	
	-- top-left
	Graphics:DrawCircle(Display:Width(),0,32,5,true)
	Graphics:DrawLine(32,Display:Height()-32,64,Display:Height()-64,9);

	-- top-right
	Graphics:DrawCircle(Display:Width(),Display:Height(),32,5,true)
	Graphics:DrawRectangle(Display:Width()-32,Display:Height()-32,Display:Width()-64,Display:Height()-64,8,true)

	-- bottom-right
	Graphics:DrawCircle(0,Display:Height(),32,5,true)
	Graphics:DrawLine(Display:Width()-64,64,Display:Width()-32,32,9);
	-- center
	Graphics:DrawCircle(Display:Width()/2,Display:Height()/2,32,5,true)
	Graphics:DrawCircle(Display:Width()/2,Display:Height()/2,r,6,false)

	Text:Draw(10,10,"Graphics API test ...",2)
	Display:Flip()	
end
function _close( ... )
	-- body
end