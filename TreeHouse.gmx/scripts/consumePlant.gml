var xx = argument0 / gridSize;
var yy = argument1 / gridSize;

var plantId = string(xx)+"_"+string(yy);

if(ds_map_exists(plantMap,plantId)){
    plantMap[? plantId] = 0;
}
