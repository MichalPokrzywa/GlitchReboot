--
function While(con)
    CheckResult(not con)
end
----
if not (x == nil or y == nil or z == nil) then
--
for i = 1, x do
    y = y + i
end
While(x + y != z)
--
end--