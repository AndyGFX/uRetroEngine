
 DB16 = 
    "140c1c".."442434".."30346d".."4e4a4e".."854c30".."346524".."d04648".."757161"..
    "597dce".."d27d2c".."8595a1".."6daa2c".."d2aa99".."6dc2ca".."dad45e".."deeed6"

 PICO8a = 
    "000000".."7e2553".."1d2b53".."5f574f".."ab5236".."008751".."ff004d".."83769c"..
    "ff77a8".."ffa300".."c2c3c7".."00e756".."ffccaa".."29adff".."fff024".."fff1e8"

PICO8b = 
    "000000".."1d2b53".."7e2553".."008751".."ab5236".."5f574f".."c2c3c7".."fff1e8"..
  	"ff004d".."ffa300".."fff024".."00e756".."29adff".."83769c".."ff77a8".."ffccaa"

 ARNE16= 
    "000000".."1b2632".."005784".."493c2b".."a46422".."44891a".."be2633".."2f484e"..
    "31a2f2".."eb8931".."9d9d9d".."a3ce27".."e06f8b".."b2dcef".."f7e26b".."ffffff"

 EDG16 = 
    "193d3f".."3f2832".."743f39".."9e2835".."b86f50".."327345".."e53b44".."4f6781"..
    "0484d1".."fb922b".."afbfd2".."63c64d".."e4a672".."2ce8f4".."ffe762".."ffffff"

 A64 = 
    "000000".."4c3435".."313a91".."485454".."92562b".."509450".."b14863".."808078"..
    "7655a2".."8385cf".."9cabb1".."9ccc47".."cd9373".."8fbfd5".."bbc840".."ede6c8"

 C64 = 
    "000000".."574200".."40318d".."505050".."8b5429".."55a049".."883932".."787878"..
    "8b3f96".."7869c4".."9f9f9f".."94e089".."b86962".."67b6bd".."bfce72".."ffffff"


_CX = 0
_CY = 0
_C = 0

function cursor(x,y)
	_CX=tonumber(x)
	_CY=tonumber(y)
end

function Color(c)
	_C = tonumber(c)
end

function print(str,x,y,c)

	Text:Draw(x or _CX,y or _CY,tostring(str),c or _C)

end

function SetPalette(pal)	
for i=1,16 do
		color = "#ff"..pal:sub((i*6)-5,(i*6))
		Colors:SetFromHex(i-1,color)
	end	
end


function time()
	return System:TimeSinceStart()
end

function cls(color)
	Display:Clear(color or 0);
end


-- table

function foreach(t,func)    
    for i,v in ipairs(t) do func(v) end
end

function add(t,v)
    table.insert(t,v)
end
function del(t,v)
    local ids={}
    local n=0
    for i,val in ipairs(t) do 
        if v==val then
            add(ids,i)
            n=i
        end 
    end
    if n>0 then
        --table.remove(t,n)
        for y,id in ipairs(ids) do 
            table.remove(t,id)
        end
    end
end
function sub(text,startPos,endPos)
    return string.sub(text,startPos,endPos)
end

--math
srand=math.randomseed
sqrt=math.sqrt
abs=math.abs
min=math.min
max=math.max
flr=math.floor
pi=math.pi

function rnd(a)
 a=a or 1
 return math.random()*a
end

function sgn(a)
 if a>=0 then return 1 end
	return -1
end

function cos(a)
 return math.cos(2*pi*a)
end

function sin(a)
 return -math.sin(2*pi*a)
end

function atan2(a,b)
 b=b or 1
 return math.atan(a,b)/(2*pi)
end

function mid(a,b,c)
 if a<=b and a<=c then return max(a,min(b,c))
	elseif b<=a and b<=c then return max(b,min(a,c)) end
	return max(c,min(a,b))
end


function rectfill(x1,y1,x2,y2,color)
	Graphics:DrawRectangle(x1,y1,x2,y2,color,true)
end

function rect(x1,y1,x2,y2,color)
	Graphics:DrawRectangle(x1,y1,x2,y2,color,false)
end
--[[
function print(str, x, y, color)	
	Text:Draw(x,y,tostring(str),color)
end
]]
function circfill(x,y,radius,color)
	Graphics:DrawCircle(x,y,radius,color,true)
end

function circ(x,y,radius,color)	
	Graphics:DrawCircle(x,y,radius,color,false)
end

function line(x1,y1,x2,y2,color)
	Graphics:DrawLine(x1,y1,x2,y2,color)
end

function pset(x,y,color)
	Graphics:PutPixel(x,y,color)
end

function pget(x,y)
	return Graphics:GetPixel(x,y)
end


function spr(id,x,y,w,h,flipX,flipY)
	Sprites:DrawSprite(id,x,y,flipX or false,flipY or false)	
end

--[[
sspr sx sy sw sh dx dy [dw dh] [flip_x] [flip_y]

		Stretch rectangle from sprite sheet (sx, sy, sw, sh) // given in pixels
		and draw in rectangle (dx, dy, dw, dh)
		Colour 0 drawn as transparent by default (see palt())
		dw, dh defaults to sw, sh
		flip_x=true to flip horizontally
		flip_y=true to flip vertically
]]
function sspr(sx,sy,sw,sh,dx,dy,dw,dy,flipX,flipY)
	-- ToDo
	-- add command for draw pixels from sprite sheet region
end

function mset(x,y,v,layer)

	Tilemap:SetTile((layer or 1)-1,x,y,v,0,false,false)
end

function mget(x,y)
	return Tilemap:GetTileID(0,x,y)
end

--[[
map cel_x cel_y sx sy cel_w cel_h [layer]

		draw section of map (in cels) at screen position sx, sy (pixels)
		if layer is specified, only cels with the same flag number set are drawn
			// Bitfield. So 0x5 means draw sprites with bit 0 and bit 2 set.
			// defaults to all sprites
	
		exception: sprite 0 is always taken to mean empty.
	
		e.g. map(0,0, 20,20, 4,2)
		-> draws a 4x2 blocks of cels starting from 0,0 in the map, to the screen at 20,20
]]

function map( cell_x,cell_y,sx,sy,cell_w,cell_h,layer )	
	Tilemap:DrawLayer(layer-1,cell_x,cell_y,cell_w,cell_h,sx,sy,0,1)
end

function clip(x,y,w,h)
	Display:Clip(x or 0,y or 0 ,w or 128,h or 128)
end

function btnp(id)

	-- LEFT
	if (id==0) then
		return Input:ButtonDown(KEY_LEFT)
	end

	-- RIGHT
	if (id==1) then
		return Input:ButtonDown(KEY_RIGHT)
	end
	-- UP
	if (id==2) then
		return Input:ButtonDown(KEY_UP)
	end

	-- DOWN
	if (id==3) then
		return Input:ButtonDown(KEY_DOWN)
	end

	-- A {key z}
	if (id==4) then
		return Input:ButtonDown(KEY_A)
	end

	-- B {key x}
	if (id==5) then
		return Input:ButtonDown(KEY_B)
	end

	return false
end

function btn(id)

	-- LEFT
	if (id==0) then
		return Input:ButtonPressed(KEY_LEFT)
	end

	-- RIGHT
	if (id==1) then
		return Input:ButtonPressed(KEY_RIGHT)
	end
	-- UP
	if (id==2) then
		return Input:ButtonPressed(KEY_UP)
	end

	-- DOWN
	if (id==3) then
		return Input:ButtonPressed(KEY_DOWN)
	end

	-- A {key z}
	if (id==4) then
		return Input:ButtonPressed(KEY_A)
	end

	-- B {key x}
	if (id==5) then
		return Input:ButtonPressed(KEY_B)
	end

	return false
end

function pal(c0,c1,p)

	if (c0~=nil) and (c1~=nil) and (p~=nil) then
		Colors:Restore();
	else
		-- p is ignored due to uRE don't use separated palette for screen
		colorC0 = Colors:Get(c0);
		colorC1 = Colors:Get(c1);
		Colors:Set(c1,colorC0.r,colorC0.g,colorC0.b,colorC0.a)
	end
end
