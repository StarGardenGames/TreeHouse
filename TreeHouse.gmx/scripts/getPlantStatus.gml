
var xx = argument0 / gridSize;
var yy = argument1 / gridSize;

var plantId = string(xx)+"_"+string(yy);
if(!ds_map_exists(plantMap,plantId)){
    var px = (xx + plantOffset)/plantScale;
    var py = (yy + plantOffset)/plantScale;
    var noise = perlinNoise2D(px,py, 100);
    if(noise > 70){
        plantMap[? plantId] = 1;
    }else{
        plantMap[? plantId] = -1;
    }
}

return plantMap[? plantId];

