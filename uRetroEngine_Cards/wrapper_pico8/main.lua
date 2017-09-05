Library:Require('pico-8.lua')

SetPalette(PICO8b)

t=0

function OnStart()

--Tilemap:Create(128,32)

Tilemap:Clear()
-- pico-8 api wrapper test  // by AndyGFX

cls() -- clear screen
rect(0,0,127,127,1)

-- Music -----------------------------------------------------------------

-- play music from pattern 02
-- (loop back flag set in pat 13)
--! music(0)

-- play sound 0 on channel 3 
--! sfx(0, 3)

-- draw palette -----------------------------------------------------------------
x=3
rectfill(1,1,7,7,5)
for i=0,15 do
 print(i,x,2,i)
 x = x + 6 + flr(i/10)*4
end

-- gfx shapes -----------------------------------------------------------------


-- camera(-10,0) -- shift draws
rectfill(10,10,18,20,8)
rect(20,10,28,20,9)
circfill(35,15,5,10)
circ(45,15,5,11)
line(52,10,58,20,12)
pset(60,10,13)
pset(62,10,pget(60,10)+1)
clip(72,10,8,10)
rectfill(0,0,127,127,6)
clip()


-- sprites ------------------------------------------------------------------


spr(1,10,25)
spr(1,20,25, 1,1,true,false)

pal(15,12) -- remap 15 to 12

sspr(8,0,8,8, 30,25,24,8)
pal(15,0) -- draw solid black
pal(1,8)
sspr(8,0,8,8, 60,25,24,8)
pal() -- reset palette mapping

-- map -----------------------------------------------------------------

-- prepare tilemap manualy


mset(0,4,3,1)
mset(0,5,2,1)


mset(1,3,4,1)
mset(1,4,3,1)
mset(1,5,2,1)

mset(2,3,4,1)
mset(2,4,3,1)
mset(2,5,5,1)

mset(3,3,0,1)
mset(3,4,3,1)
mset(3,5,2,1)

mset(1,3,4,2)
mset(2,3,4,2)
mset(3,3,6,2)

-- map

mset(3,3,mget(1,3)+2)
map(0,3,10,35,4,3,1)
--fset(6,1,true) --pink flower
-- only cels with flag 2 set
map(0,3,50,35,4,3,2)


-- set cursor position and color
cursor(10,90)
Color(7)

-- math
cursor(10,70)
x = cos(0.125) + mid(1,2,3)
x = (x%1) + abs(-1)
print("x: "..x)


-- collections and strings -----------------------------------------------------------------
cursor(10,78)
a={}
add(a, 11) add(a, 12)
add(a, 13) add(a, 14)
del(a, 13)
str="a: "
--for i in all(a) do
for key,i in ipairs(a) do 
 str=str..i.." "
end
print(str)

-- foreach and anon functions -----------------------------------------------------------------

cursor(10,86)
total=0
foreach(a, 
 function(i)
  total=total+i
 end
)
print("total: "..total)





end

function Draw()



 -- show current value of t
 rectfill(0,92,30,98,5)
 print("t: "..t,1,93,7)

 -- show state of buttons

 print("buttons: ", 1,101,7) 

 for p=0,7 do 
 for i=0,5 do
  col=5
  if(btn(i,p)) then col=8+i end
  rectfill(40+i*10,100+p*3,
   48+i*10, 101+p*3, col)
 end
 end

end

function OnUpdate(deltaTime)
	t=t+1
	Draw()
	Display:Flip();
end

function OnClose()
end
		        