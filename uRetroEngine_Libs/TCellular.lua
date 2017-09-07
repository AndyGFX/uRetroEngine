--[[


V1.0

Initialization:

Library:Require("class.lua")
Library:Require("TCellular.lua")

Generate map:

function TCellular:Generate(w,h,wallChance,birthLimit,deathLimit,smoothCount)

Argumets:

- w = width
- h = height
- wallChange = probability for wall
- birthLimit = count for birth cell (wall)
- deathLimit = count for empty cell
- smoothCount = count of stepSimulation loop


Example:

	cave = class(TCellular)
	cave:Generate(128,64,0.6,3,5,5) 

result is stored in 1d vector in field _data, you can get value list via cave:GetMapData() or cave:Map(x,y) to point at [x,y]

]]


TCellular = class()


---------------------------------------------------------------
-- TCellular Methods ------------------------------------------
---------------------------------------------------------------

---------------------------------------------------------------
-- Conver 2d [x,y]  map position to index of 1d vector
---------------------------------------------------------------
function TCellular:Pos(x,y)
	return ((y-1)*self._w + (x-1))
end

---------------------------------------------------------------
-- Generate empty map
---------------------------------------------------------------
function TCellular:GenerateEmptyMap()
	for x=1,self._w do
		for y=1,self._h do
			self._data[self:Pos(x,y)]=0
		end
	end
end

---------------------------------------------------------------
-- Random fill map
---------------------------------------------------------------
function TCellular:RandomFillMap()	
	for x=1,self._w do
		for y=1,self._h do
			self._data[self:Pos(x,y)]=math.floor(math.random(0,2))
		end
	end
end

---------------------------------------------------------------
-- Calculate neighbors count
---------------------------------------------------------------
function TCellular:CountLivingNeighbors(x,y)
	local count = 0

	for i=-1,1 do
		for j=-1,1 do
			local nb_x = i+x
			local nb_y = j+y
			if (nb_x < 1 or nb_y < 1 or nb_x >= self._w or nb_y >= self._h) then count=count+1 end
            if (self._data[self:Pos(nb_x, nb_y)] == 0) then  count=count+1 end
		end
	end
	return count
end

---------------------------------------------------------------
-- Apply cellular rules to map
---------------------------------------------------------------
function TCellular:StepSimulation()

	local newMap = {}
	for cx=1,self._w do
		for cy=1,self._h do

			local livingNeighbors = self:CountLivingNeighbors(cx, cy)
			
			if (self._data[self:Pos(cx, cy)] == 1) then
                
                    if (livingNeighbors < self._deathLimit) then
                        newMap[self:Pos(cx, cy)] = 1
                    else
                        newMap[self:Pos(cx, cy)] = 0
                    end
                
                else
                
                    if (livingNeighbors > self._birthLimit) then
                        newMap[self:Pos(cx, cy)] = 0
                    else
                        newMap[self:Pos(cx, cy)] = 1
                    end
                end
            
		end
	end
	self._data = newMap
end

---------------------------------------------------------------
-- generate cellular automata
---------------------------------------------------------------
-- w = width
-- h = height
-- wallChange = probability for wall
-- birthLimit = count for birth cell (wall)
-- deathLimit = count for empty cell
-- smoothCount = count of stepSimulation loop
function TCellular:Generate(w,h,wallChance,birthLimit,deathLimit,smoothCount)

	self._w = w
	self._h = h
	self._wallSpawnChance = wallChance
	self._birthLimit = birthLimit;
    self._deathLimit = deathLimit;
    self._data = {}

    --Generate clean map
	--self:GenerateEmptyMap();

	--Generate random wall cells
    self:RandomFillMap();
	
	-- Simulate Step => smooth
	for i = 1,smoothCount do
		self:StepSimulation()
    end
end

---------------------------------------------------------------
-- Get map value at cell[X,Y]
---------------------------------------------------------------
function TCellular:Map(x,y)
	return self._data[self:Pos(x,y)]
end

---------------------------------------------------------------
-- Get map value at cell[X,Y]
---------------------------------------------------------------
function TCellular:Set(x,y,value)
	self._data[self:Pos(x,y)] = value
end
---------------------------------------------------------------
-- Get map data as array
---------------------------------------------------------------
function TCellular:GetMapData()
	return self._data;
end

---------------------------------------------------------------
-- Preview generated map on display
---------------------------------------------------------------
function TCellular:PreviewMap(ox,oy)
	
	for xm=1,cave._w do
		for ym=1,cave._h do
			if (cave:Map(xm,ym)==1) then
				Graphics:PutPixel(ox+xm,oy+ym,5)
			else
				Graphics:PutPixel(ox+xm,oy+ym,1)
			end
		end
	end
end

---------------------------------------------------------------
-- Dump map data to log file
---------------------------------------------------------------
function TCellular:Dump()
	for xm=1,cave._w do
		local line = "";
		for ym=1,cave._h do
			line = line..cave:Map(xm,ym);
		end
		print(line);
	end
end
