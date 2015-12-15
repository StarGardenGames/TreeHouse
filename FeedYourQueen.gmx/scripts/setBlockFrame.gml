//room space
var xx = argument0;
var yy = argument1;
/*var i = (argument0 - x) div gridSize;
var j = (argument1 - y) div gridSize;*/
var frame = argument2;

//data[# i, j] = data[# i,j] - (data[# i,j] % 10) + frame;
var val = getValueForBlock(xx,yy);
if(val == NULL)
    return NULL
else
    setValueForBlock(xx,yy,val - (val % 10) + frame);
