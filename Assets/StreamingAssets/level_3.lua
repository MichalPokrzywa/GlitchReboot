--
function While(con)
    CheckResult(not con)
end
----
if not (platform1 == nil or platform2 == nil or platform3 == nil) then
--
x = platform1
y = platform2
z = platform3
for i = 1, x do
    y = y + i
end
While(x + y != z)
--
end--