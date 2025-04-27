function ShowWorking()
	for i = 1, value_y do
		value_x = value_x + i
	end	

	if value_x < value_y then
		DoorLua:Nonsense(WhiteDoors, false)
	end
end