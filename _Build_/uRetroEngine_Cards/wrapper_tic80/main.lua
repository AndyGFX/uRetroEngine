
Library:Require('tic-80.lua')

-- title:  pslib
-- author: Viza
-- desc:   An advenced particle system library for the VIC-80
-- script: lua


--==================================================================================--
-- INTRO/DOCS ======================================================================--
--==================================================================================--

--[[

Hi!
First of all, don't be intimidated by the size of the source, it is very 
straightforward to understand (hopefully :) )

pslib is:
- data driven: to create a particle system you fill out a couple of tables, then handle it to pslib
- modular: You only need to include the modules you use in your cart
- easily extendable: Can't achieve the particle system behaviour you imagined? Just write a simple new module

So how it works?

A particle system is consist of 4 main parts:
- emit timer(s) dictate WHEN to emit a particle
- emitter(s) define WHERE to create the particle, and the initial SPEED
- drawfunc(s) know how to DRAW a particle
- affector(s) know how to MOVE a particle

To create a particle system, call make_psystem, and then fill the emittimers, emitters,
drawfuncs, and affectors tables with your parameters. See the SAMPLE PARTICLE SYSTEMS section
below for examples.

After that you need to call update_psystems in your TIC function to... uhm... update the 
particle systems, and draw_ps or draw_psystems to draw them.

These are the basics, but actually there are not much more to it. :)
You can find more information below in the comments about the main functions or modules.

If you have questions, feel free to contact me at:
email: viza@ccatgames, twitter: @viza, web: blog.ccatgames.com

--]]



function lerp(a, b, t)
	return a + (b - a) * t
end
--==================================================================================--
-- PARTICLE SYSTEM LIBRARY =========================================================--
--==================================================================================--

particle_systems = {}

-- Call this, to create an empty particle system, and then fill the emittimers, emitters,
-- drawfuncs, and affectors tables with your parameters.
function make_psystem(minlife, maxlife, minstartsize, maxstartsize, minendsize, maxendsize)
	local ps = {
	-- global particle system params

	-- if true, automatically deletes the particle system if all of it's particles died
	autoremove = true,

	minlife = minlife,
	maxlife = maxlife,

	minstartsize = minstartsize,
	maxstartsize = maxstartsize,
	minendsize = minendsize,
	maxendsize = maxendsize,

	-- container for the particles
	particles = {},

	-- emittimers dictate when a particle should start
	-- they called every frame, and call emit_particle when they see fit
	-- they should return false if no longer need to be updated
	emittimers = {},

	-- emitters must initialize p.x, p.y, p.vx, p.vy
	emitters = {},

	-- every ps needs a drawfunc
	drawfuncs = {},

	-- affectors affect the movement of the particles
	affectors = {},
	}

	table.insert(particle_systems, ps)

	return ps
end

-- Call this to update all particle systems
function update_psystems()
	local timenow = time()
	for key,ps in pairs(particle_systems) do
		update_ps(ps, timenow)
	end
end

-- updates individual particle systems
-- most of the time, you don't have to deal with this, the above function is sufficient
-- but you can call this if you want (for example fast forwarding a particle system before first draw)
function update_ps(ps, timenow)
	for key,et in pairs(ps.emittimers) do
		local keep = et.timerfunc(ps, et.params)
		if (keep==false) then
			table.remove(ps.emittimers, key)
		end
	end

	for key,p in pairs(ps.particles) do
		p.phase = (timenow-p.starttime)/(p.deathtime-p.starttime)

		for key,a in pairs(ps.affectors) do
			a.affectfunc(p, a.params)
		end

		p.x = p.x + p.vx
		p.y = p.y + p.vy

		local dead = false
		if (p.x<0 or p.x>Display:Width() or p.y<0 or p.y>Display:Height()) then
			dead = true
		end

		if (timenow>=p.deathtime) then
			dead = true
		end

		if (dead==true) then
			table.remove(ps.particles, key)
		end
	end

	if (ps.autoremove==true and #ps.particles<=0) then
		local psidx = -1
		for pskey,pps in pairs(particle_systems) do
			if pps==ps then
				table.remove(particle_systems, pskey)
				return
			end
		end
	end
end

-- draw a single particle system
function draw_ps(ps, params)
	for key,df in pairs(ps.drawfuncs) do
		df.drawfunc(ps, df.params)
	end
end

-- draws all particle system
-- This is just a convinience function, you probably want to draw the individual particles,
-- if you want to control the draw order in relation to the other game objects for example
function draw_psystems()
	for key,ps in pairs(particle_systems) do
		draw_ps(ps)
	end
end

-- This need to be called from emitttimers, when they decide it is time to emit a particle
function emit_particle(psystem)
	local p = {}

	local ecount = nil
	local e = psystem.emitters[math.random(#psystem.emitters)]
	e.emitfunc(p, e.params)

	p.phase = 0
	p.starttime = time()
	p.deathtime = time()+frnd(psystem.maxlife-psystem.minlife)+psystem.minlife

	p.startsize = frnd(psystem.maxstartsize-psystem.minstartsize)+psystem.minstartsize
	p.endsize = frnd(psystem.maxendsize-psystem.minendsize)+psystem.minendsize

	table.insert(psystem.particles, p)
end

function frnd(max)
	return math.random()*max
end


--================================================================--
-- MODULES =======================================================--
--================================================================--

-- You only need to copy the modules you actually use to your program


-- EMIT TIMERS ==================================================--

-- Spawns a bunch of particles at the same time, then removes itself
-- params:
-- num - the number of particle to spawn
function emittimer_burst(ps, params)
	for i=1,params.num do
		emit_particle(ps)
	end
	return false
end

-- Emits a particle every "speed" time
-- params:
-- speed - time between particle emits
function emittimer_constant(ps, params)
	if (params.nextemittime<=time()) then
		emit_particle(ps)
		params.nextemittime = params.nextemittime + params.speed		
	end
	return true
end

-- EMITTERS =====================================================--

-- Emits particles from a single point
-- params:
-- x,y - the coordinates of the point
-- minstartvx, minstartvy and maxstartvx, maxstartvy - the start velocity is randomly chosen between these values
function emitter_point(p, params)
	p.x = params.x
	p.y = params.y

	p.vx = frnd(params.maxstartvx-params.minstartvx)+params.minstartvx
	p.vy = frnd(params.maxstartvy-params.minstartvy)+params.minstartvy
end

-- Emits particles from the surface of a rectangle
-- params:
-- minx,miny and maxx, maxy - the corners of the rectangle
-- minstartvx, minstartvy and maxstartvx, maxstartvy - the start velocity is randomly chosen between these values
function emitter_box(p, params)
	p.x = frnd(params.maxx-params.minx)+params.minx
	p.y = frnd(params.maxy-params.miny)+params.miny

	p.vx = frnd(params.maxstartvx-params.minstartvx)+params.minstartvx
	p.vy = frnd(params.maxstartvy-params.minstartvy)+params.minstartvy
end

-- AFFECTORS ====================================================--

-- Constant force applied to the particle troughout it's life
-- Think gravity, or wind
-- params: 
-- fx and fy - the force vector
function affect_force(p, params)
	p.vx = p.vx + params.fx
	p.vy = p.vy + params.fy
end

-- A rectangular region, if a particle happens to be in it, apply a constant force to it
-- params: 
-- zoneminx, zoneminy and zonemaxx, zonemaxy - the corners of the rectangular area
-- fx and fy - the force vector
function affect_forcezone(p, params)
	if (p.x>=params.zoneminx and p.x<=params.zonemaxx and p.y>=params.zoneminy and p.y<=params.zonemaxy) then
		p.vx = p.vx + params.fx
		p.vy = p.vy + params.fy
	end
end

-- A rectangular region, if a particle happens to be in it, the particle stops
-- params: 
-- zoneminx, zoneminy and zonemaxx, zonemaxy - the corners of the rectangular area
function affect_stopzone(p, params)
	if (p.x>=params.zoneminx and p.x<=params.zonemaxx and p.y>=params.zoneminy and p.y<=params.zonemaxy) then
		p.vx = 0
		p.vy = 0
	end
end

-- A rectangular region, if a particle cames in contact with it, it bounces back
-- params: 
-- zoneminx, zoneminy and zonemaxx, zonemaxy - the corners of the rectangular area
-- damping - the velocity loss on contact
function affect_bouncezone(p, params)
	if (p.x>=params.zoneminx and p.x<=params.zonemaxx and p.y>=params.zoneminy and p.y<=params.zonemaxy) then
		p.vx = -p.vx*params.damping
		p.vy = -p.vy*params.damping
	end
end

-- A point in space which pulls (or pushes) particles in a specified radius around it
-- params:
-- x,y - the coordinates of the affector
-- radius - the size of the affector
-- strength - push/pull force - proportional with the particle distance to the affector coordinates
function affect_attract(p, params)
	if (math.abs(p.x-params.x)+math.abs(p.y-params.y)<params.mradius) then
		p.vx = p.vx + (p.x-params.x)*params.strength
		p.vy = p.vy + (p.y-params.y)*params.strength
	end
end

-- Moves particles around in a sin/cos wave or circulary. Directly modifies the particle position
-- params:
-- speed - the effect speed
-- xstrength, ystrength - the amplituse around the x and y axes
function affect_orbit(p, params)
	params.phase = params.phase + params.speed
	p.x = p.x + math.sin(params.phase)*params.xstrength
	p.y = p.y + math.cos(params.phase)*params.ystrength
end

-- DRAW FUNCS ===================================================--

-- Filled circle particle drawer, the particle animates it's size and color trough it's life
-- params:
-- colors array - indexes to the palette, the particle goes trough these in order trough it's lifetime
-- startsize and endsize is coming from the particle system parameters, not the draw func params!
function draw_ps_fillcirc(ps, params)
	local i = 0
	for key,p in pairs(ps.particles) do
		c = math.floor(lerp(0,1,p.phase*1000)*#params.colors)+1	
		r = (2-lerp(0,1,p.phase*1000))*p.startsize+p.phase*p.endsize
		circ(p.x,p.y,r,params.colors[c] or 0)
		
	end
end

-- Single pixel particle, which animates trough the given colors
-- params:
-- colors array - indexes to the palette, the particle goes trough these in order trough it's lifetime
function draw_ps_pixel(ps, params)
	
	for key,p in pairs(ps.particles) do
		c = math.floor(lerp(0,1,p.phase*1000)*#params.colors)+1			
		pix(p.x,p.y,params.colors[c] or 0)
		
	end
end

-- Draws a line between the particle's previous and current position, kind of "motion blur" effect
-- params:
-- colors array - indexes to the palette, the particle goes trough these in order trough it's lifetime
function draw_ps_streak(ps, params)
	for key,p in pairs(ps.particles) do
		--c = math.floor(p.phase*#params.colors)+1
		c = math.floor(lerp(0,1,p.phase*1000)*#params.colors)+1			
		line(p.x,p.y,p.x-p.vx,p.y-p.vy,params.colors[c] or 0)		
	end
end

-- Animates trough the given frames with the given speed
-- params:
-- frames array - indexes to sprite tiles
function draw_ps_animspr(ps, params)
	params.currframe = params.currframe + params.speed
	if (params.currframe>#params.frames) then
		params.currframe = 1
	end
	for key,p in pairs(ps.particles) do
		-- pal(7,params.colors[math.floor(p.endsize)])
		spr(params.frames[math.floor(params.currframe+p.startsize)%#params.frames],p.x,p.y,0,0)
	end
	-- pal()
end

-- Maps the given frames to the life of the particle
-- params:
-- frames array - indexes to sprite tiles
function draw_ps_agespr(ps, params)
	
	for key,p in pairs(ps.particles) do
		--local f = math.floor(p.phase*#params.frames)+1
		local f = math.floor(#params.frames*(p.phase*1000))+1
		
		spr(params.frames[f],p.x,p.y,0,0)
	end
end

-- Each particle is randomly chosen from the given frames
-- params:
-- frames array - indexes to sprite tiles
function draw_ps_rndspr(ps, params)
	for key,p in pairs(ps.particles) do
		-- pal(7,params.colors[math.floor(p.endsize)])
		spr(params.frames[math.floor(p.startsize)],p.x,p.y,0,0)
	end
	-- pal()
end


--==================================================================================--
-- SAMPLES PARTICLE SYSTEMS ========================================================--
--==================================================================================--
function make_bubbles_ps()
	local ps = make_psystem(500,3000, 1,9,0.5,0.5)
	
	ps.autoremove = false
	table.insert(ps.emittimers,
		{
			timerfunc = emittimer_constant,
			params = {nextemittime = time(), speed = 0.2}
		}
	)
	table.insert(ps.emitters, 
		{
			emitfunc = emitter_box,
			params = { minx = 0, maxx = 240, miny = 100, maxy= 110, minstartvx = 0, maxstartvx = 0, minstartvy = -1.50, maxstartvy=-0.2 }
		}
	)
	table.insert(ps.drawfuncs,
		{
			drawfunc = draw_ps_agespr,
			params = { frames = {16,16,17,17,17,18,18,18,18,18,18,18,18,18,18,19} }
		}
	)
	table.insert(ps.affectors,
		{ 
			affectfunc = affect_orbit,
			params = { phase = 0, speed = 0.001, xstrength = 0.5, ystrength = 0 }
		}
	)
end

function make_magicsparks_ps(ex,ey)
	local ps = make_psystem(300,1700, 1,5,1,5)
	
	table.insert(ps.emittimers,
		{
			timerfunc = emittimer_burst,
			params = { num = 10}
		}
	)
	table.insert(ps.emitters, 
		{
			emitfunc = emitter_box,
			params = { minx = ex-8, maxx = ex+8, miny = ey-8, maxy= ey+8, minstartvx = -1.5, maxstartvx = 1.5, minstartvy = -3, maxstartvy=-2 }
		}
	)
	table.insert(ps.drawfuncs,
		{
			drawfunc = draw_ps_rndspr,
			params = { frames = {32,33,34,35,36} }
			-- params = { frames = {32,33,34,35,36}, colors = {8,9,11,12,14} }
		}
	)
	table.insert(ps.affectors,
		{ 
			affectfunc = affect_force,
			params = { fx = 0, fy = 0.3 }
		}
	)

end

function make_butterflies_ps(ex,ey)
	local ps = make_psystem(2000,3000, 1,9,1,5)
	
	table.insert(ps.emittimers,
		{
			timerfunc = emittimer_burst,
			params = { num = 10}
		}
	)
	table.insert(ps.emitters, 
		{
			emitfunc = emitter_box,
			params = { minx = ex-16, maxx = ex+16, miny = ey-8, maxy= ey+8, minstartvx = 0, maxstartvx = 0, minstartvy = -1, maxstartvy= -0.5 }
		}
	)
	table.insert(ps.drawfuncs,
		{
			drawfunc = draw_ps_animspr,
			params = { frames = {22,23,24,23}, speed = 0.2, currframe = 1 }
			-- params = { frames = {22,23,24,23}, speed = 0.5, colors = {8,9,11,12,14}, currframe = 1 }
		}
	)
	table.insert(ps.affectors,
		{ 
			affectfunc = affect_forcezone,
			params = { fx = -0.05, fy = 0.0, zoneminx = 64, zonemaxx = 127, zoneminy = 64, zonemaxy = 100 }
		}
	)
	table.insert(ps.affectors,
		{ 
			affectfunc = affect_forcezone,
			params = { fx = 0.05, fy = 0.0, zoneminx = 0, zonemaxx = 64, zoneminy = 30, zonemaxy = 70 }
		}
	)
end

function make_3dwarp_ps()
	local ps = make_psystem(1000,2000, 1,2,0.5,0.5)
	ps.autoremove = false
	table.insert(ps.emittimers,
		{
			timerfunc = emittimer_constant,
			params = {nextemittime = time(), speed = 0.001}
		}
	)
	table.insert(ps.emitters, 
		{
			emitfunc = emitter_box,
			params = { minx = 118, maxx = 122, miny = 63, maxy= 67, minstartvx = 0, maxstartvx = 0, minstartvy = 0, maxstartvy=0 }
		}
	)
	table.insert(ps.affectors, 
		{
			affectfunc = affect_attract,
			params = { x = 120, y = 65, mradius = 64, strength = 0.01 }
		}
	)
	table.insert(ps.drawfuncs,
		{
			drawfunc = draw_ps_streak,
			params = { colors = {2,2,2,2,2,1,1,1,1,1,10,15,1,10,10,10,15,10,10,15,10,15,15} }
		}
	)
end

function make_starfield_ps()
	local ps = make_psystem(4000,6000, 1,2,0.5,0.5)
	ps.autoremove = false
	table.insert(ps.emittimers,
		{
			timerfunc = emittimer_constant,
			params = {nextemittime = time(), speed = 0.01}
		}
	)
	table.insert(ps.emitters, 
		{
			emitfunc = emitter_box,
			params = { minx = 235, maxx = 240, miny = 0, maxy= 136, minstartvx = -2.0, maxstartvx = -0.5, minstartvy = 0, maxstartvy=0 }
		}
	)
	table.insert(ps.drawfuncs,
		{
			drawfunc = draw_ps_pixel,
			params = { colors = {15,10,15,10,15,10,10,15,10,15,15,10,10,15} }
		}
	)
end

function make_waterfall_ps(ex,ey)
	local ps = make_psystem(1500,2000, 1,2,0.5,0.5)
	ps.autoremove = false
	table.insert(ps.emittimers,
		{
			timerfunc = emittimer_constant,
			params = {nextemittime = time(), speed = 0.01}
		}
	)
	table.insert(ps.emitters, 
		{
			emitfunc = emitter_box,
			params = { minx = ex-8, maxx = ex+8, miny = ey, maxy= ey+1, minstartvx = -0.5, maxstartvx = 0.5, minstartvy = 0, maxstartvy=0 }
		}
	)
	table.insert(ps.drawfuncs,
		{
			drawfunc = draw_ps_streak,
			params = { colors = {15,13,2,13,13,2,13,2,2,15,15,15} }
		}
	)
	table.insert(ps.affectors,
		{ 
			affectfunc = affect_force,
			params = { fx = 0, fy = 0.3 }
		}
	)
	table.insert(ps.affectors,
		{ 
			affectfunc = affect_bouncezone,
			params = { damping = 0.2, zoneminx = 40, zonemaxx = 200, zoneminy = 100, zonemaxy = 136 }
		}
	)
end

function make_blood_ps(ex,ey)
	local ps = make_psystem(2000,3000, 1,2,0.5,0.5)
	
	table.insert(ps.emittimers,
		{
			timerfunc = emittimer_burst,
			params = { num = 30}
		}
	)
	table.insert(ps.emitters, 
		{
			emitfunc = emitter_point,
			params = { x = ex, y = ey, minstartvx = 1, maxstartvx = 3, minstartvy = -3, maxstartvy=-2 }
		}
	)
	table.insert(ps.drawfuncs,
		{
			drawfunc = draw_ps_pixel,
			params = { colors = {6} }
		}
	)
	table.insert(ps.affectors,
		{ 
			affectfunc = affect_force,
			params = { fx = 0, fy = 0.15 }
		}
	)
	table.insert(ps.affectors,
		{ 
			affectfunc = affect_stopzone,
			params = { zoneminx = 0, zonemaxx = 240, zoneminy = 100, zonemaxy = 127 }
		}
	)
end

function make_sparks_ps(ex,ey)
	local ps = make_psystem(300,700, 1,2, 0.5,0.5)

	table.insert(ps.emittimers,
		{
			timerfunc = emittimer_burst,
			params = { num = 10 }
		}
	)
	table.insert(ps.emitters,
		{
			emitfunc = emitter_point,
			params = { x = ex, y = ey, minstartvx = -1.5, maxstartvx = 1.5, minstartvy = -3, maxstartvy=-2 }
		}
	)
	table.insert(ps.drawfuncs,
		{
			drawfunc = draw_ps_fillcirc,
			params = { colors = {15,14,12,9,4,3} }
		}
	)
	table.insert(ps.affectors,
		{
			affectfunc = affect_force,
			params = { fx = 0, fy = 0.3 }
		}
	)
end

function make_explosparks_ps(ex,ey)
	local ps = make_psystem(300,700, 1,2,0.5,0.5)
	
	table.insert(ps.emittimers,
		{
			timerfunc = emittimer_burst,
			params = { num = 10}
		}
	)
	table.insert(ps.emitters, 
		{
			emitfunc = emitter_point,
			params = { x = ex, y = ey, minstartvx = -1.5, maxstartvx = 1.5, minstartvy = -1.5, maxstartvy=1.5 }
		}
	)
	table.insert(ps.drawfuncs,
		{
			drawfunc = draw_ps_pixel,
			params = { colors = {12,10,1,4,1,2} }
		}
	)
	table.insert(ps.affectors,
		{ 
			affectfunc = affect_force,
			params = { fx = 0, fy = 0.1 }
		}
	)
end

function make_explosion_ps(ex,ey)
	local ps = make_psystem(100,500, 9,14,1,3)
	
	table.insert(ps.emittimers,
		{
			timerfunc = emittimer_burst,
			params = { num = 4 }
		}
	)
	table.insert(ps.emitters, 
		{
			emitfunc = emitter_box,
			params = { minx = ex-4, maxx = ex+4, miny = ey-4, maxy= ey+4, minstartvx = 0, maxstartvx = 0, minstartvy = 0, maxstartvy=0 }
		}
	)
	table.insert(ps.drawfuncs,
		{
			drawfunc = draw_ps_fillcirc,
			params = { colors = {15,0,14,9,9,4} }
		}
	)
end

function make_smoke_ps(ex,ey)
	local ps = make_psystem(200,2000, 1,3, 6,9)
	
	ps.autoremove = false

	table.insert(ps.emittimers,
		{
			timerfunc = emittimer_constant,
			params = {nextemittime = time(), speed = 0.2}
		}
	)
	table.insert(ps.emitters, 
		{
			emitfunc = emitter_box,
			params = { minx = ex-4, maxx = ex+4, miny = ey, maxy= ey+2, minstartvx = 0, maxstartvx = 0, minstartvy = 0, maxstartvy=0 }
		}
	)
	table.insert(ps.drawfuncs,
		{
			drawfunc = draw_ps_fillcirc,
			params = { colors = {1,3,2} }
		}
	)
	table.insert(ps.affectors,
		{ 
			affectfunc = affect_force,
			params = { fx = 0.003, fy = -0.009 }
		}
	)
end

function make_explosmoke_ps(ex,ey)
	local ps = make_psystem(1500,2000, 5,8, 17,18)

	table.insert(ps.emittimers,
		{
			timerfunc = emittimer_burst,
			params = { num = 1 }
		}
	)
	table.insert(ps.emitters, 
		{
			emitfunc = emitter_point,
			params = { x = ex, y = ey, minstartvx = 0, maxstartvx = 0, minstartvy = 0, maxstartvy=0 }
		}
	)
	table.insert(ps.drawfuncs,
		{
			drawfunc = draw_ps_fillcirc,
			params = { colors = {2} }
		}
	)
	table.insert(ps.affectors,
		{ 
			affectfunc = affect_force,
			params = { fx = 0.003, fy = -0.01 }
		}
	)
end

--==================================================================================--
-- DEMOS ===========================================================================--
--==================================================================================--

function sparks_demo()
	make_sparks_ps(frnd(220)+10,frnd(116)+10)
end

function explo_demo()
	make_explosion_ps(frnd(220)+10,frnd(116)+10)
end

function richexplo_demo()
	local rx = frnd(220)+10
	local ry = frnd(116)+10
	make_explosmoke_ps(rx,ry)
	make_explosparks_ps(rx,ry)
	make_explosion_ps(rx,ry)
end

function blood_demo()
	make_blood_ps(frnd(64),frnd(90)+10)
end

function smoke_demo()
	make_smoke_ps(frnd(220)+10,frnd(90)+10)
end

function waterfall_demo()
	make_waterfall_ps(frnd(220)+10,frnd(50)+10)
end

function starfield_demo()
	make_starfield_ps()
end

function warp_demo()
	make_3dwarp_ps()
end

function magicsparks_demo()
	make_magicsparks_ps(frnd(220)+10,frnd(116)+10)
end

function butterflies_demo()
	make_butterflies_ps(frnd(220)+10,frnd(54)+64)
end

function bubbles_demo()
	make_bubbles_ps()
end

demos = {
	{name = "sparks", desc = "", createfunc = sparks_demo },
	{name = "explosion", desc = "", createfunc = explo_demo },
	{name = "rich explosion", createfunc = richexplo_demo, desc = "multiple particle systems" },
	{name = "blood", createfunc = blood_demo, desc = "stopzone affector" },
	{name = "smoke", createfunc = smoke_demo, desc = "continuos particle system" },
	{name = "waterfall", createfunc = waterfall_demo, desc = "streak draw bouncezone affector" },
	{name = "starfield", createfunc = starfield_demo, desc = "" },
	{name = "3d warp", createfunc = warp_demo, desc = "attract affector" },
	{name = "magic sparks", createfunc = magicsparks_demo, desc = "rndspr" },
	{name = "bubbles", createfunc = bubbles_demo, desc = "agespr, orbit affector" },
	{name = "butterflies", createfunc = butterflies_demo, desc = "animspr, forcezone affector" },
}
currdemo = 1


--==================================================================================--
-- INIT ============================================================================--
--==================================================================================--

function deleteallps()
	for key,ps in pairs(particle_systems) do
		particle_systems[key] = nil
	end
end




function OnStart()
	Display:TargetFrameRate(60) 
	demos[currdemo].createfunc()
	SetPalette(DB16)
end

function OnUpdate(deltaTime)
	if btnp(3) then
		currdemo = currdemo + 1
		if (currdemo>#demos) then
		 	currdemo = 1
		 end 
		 deleteallps()
		 demos[currdemo].createfunc()
	end
	if btnp(2) then
		currdemo = currdemo - 1
		if (currdemo<=0) then
		 	currdemo = #demos
		 end 
		 deleteallps()
		 demos[currdemo].createfunc()
	end
	if btnp(5) then
		demos[currdemo].createfunc()
	end

	update_psystems()
	
	cls(0)
	
	draw_psystems()

	print(demos[currdemo].name,0,0,7)
	print(demos[currdemo].desc,0,8,7)
	print("left/right to change demo", 0, Display:Height()-8, 5)
	print("x to spawn particle system",0,Display:Height()-16,5)


	Display:Flip()
end

function OnClose()
end
		        