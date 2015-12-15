//room space
/*var i = (argument0-x) div gridSize;
var j = (argument1-y) div gridSize;
return data[# i, j] % 10;*/

var xx = argument0;
var yy = argument1;

var val = getValueForBlock(xx,yy);

if(val == NULL){
    return NULL;
}else{
    return val % 10;
}
