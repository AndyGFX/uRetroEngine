
 --[[

scanline
X btn
X btnp
  clip
X cls
X circ
X circb
  exit
  font
X line
  map
  memcpy
  memset
  mget
  mouse
  mset
  music
  peek
  peek4
X pix
  pmem
  poke
  poke4
X print
  rect
  rectb
  sfx
X spr
  sync
X time
  trace
  tri

 ]]


 DB16 = 
    "140c1c".."442434".."30346d".."4e4a4e".."854c30".."346524".."d04648".."757161"..
    "597dce".."d27d2c".."8595a1".."6daa2c".."d2aa99".."6dc2ca".."dad45e".."deeed6"

 PICO8 = 
    "000000".."7e2553".."1d2b53".."5f574f".."ab5236".."008751".."ff004d".."83769c"..
    "ff77a8".."ffa300".."c2c3c7".."00e756".."ffccaa".."29adff".."fff024".."fff1e8"

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

function print(text,x,y,color)
	Text:Draw(x,y,text,color)
end

function circ(x,y,radius,color)		
	Graphics:DrawCircle(x,y,radius,color,true)
end

function circb(x,y,radius,color)		
	Graphics:DrawCircle(x,y,radius,color,false)
end

function pix(x,y,color)
	Graphics:PutPixel(x,y,color)
end

function line(x1,y1,x2,y2,color)
	Graphics:DrawLine(x1,y1,x2,y2,color)
end

function spr(id,x,y,w,h,flipX,flipY)
	Sprites:DrawSprite(id,x,y,flipX or false,flipY or false)	
end
function btnp(id)

	-- LEFT
	if (id==2) then
		return Input:ButtonDown(KEY_LEFT)
	end

	-- RIGHT
	if (id==3) then
		return Input:ButtonDown(KEY_RIGHT)
	end
	-- UP
	if (id==0) then
		return Input:ButtonDown(KEY_UP)
	end

	-- DOWN
	if (id==1) then
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
	if (id==2) then
		return Input:ButtonPressed(KEY_LEFT)
	end

	-- RIGHT
	if (id==3) then
		return Input:ButtonPressed(KEY_RIGHT)
	end
	-- UP
	if (id==0) then
		return Input:ButtonPressed(KEY_UP)
	end

	-- DOWN
	if (id==1) then
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