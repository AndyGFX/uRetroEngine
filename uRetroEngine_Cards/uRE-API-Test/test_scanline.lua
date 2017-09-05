
l = 0;

 
Console:Print(System:RootPath())

function _start()
	Console:Print("Start ...")
	Display:CallScanline(true)
end

function _scanline( line )
	l = line
	n = l%32
	color = Colors:Get(n);
	Colors:Set(5,color.r,color.g,color.b,color.a)
end

function _update( deltatime )
	-- body

end

function _draw( ... )
	Display:Clear(3)
	-- bottom-left
	Graphics:DrawCircle(0,0,32,5,true)

	-- top-left
	Graphics:DrawCircle(Display:Width(),0,32,5,true)

	-- top-right
	Graphics:DrawCircle(Display:Width(),Display:Height(),32,5,true)

	-- bottom-right
	Graphics:DrawCircle(0,Display:Height(),32,5,true)

	-- center
	Graphics:DrawCircle(Display:Width()/2,Display:Height()/2,32,5,true)

	Text:Draw(10,10,"Scanline API test (experimental)...",1)
	Display:Flip()
	Colors:Restore()
end
function _close( ... )
	-- body
end