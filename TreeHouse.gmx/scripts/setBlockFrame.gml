//room space
var i = (argument0 - x) div gridSize;
var j = (argument1 - y) div gridSize;
var frame = argument2;

data[# i, j] = data[# i,j] - (data[# i,j] % 10) + frame;
