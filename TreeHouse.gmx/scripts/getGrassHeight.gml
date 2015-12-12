var xx = argument0 / gridSize;
var yy = argument1 / gridSize;

var grassId = string(xx)+"_"+string(yy);
if(!ds_map_exists(grassHeights,grassId)){
    var height;
    var px = (xx + grassOffset)/grassScale;
    var py = (yy + grassOffset)/grassScale;
    var noise = perlinNoise2D(px,py, 100);
    if(noise > 40){
        height = round((noise - 40) / 12);
        height = min(numGrassFrames-1,height);
    }else{
        height = -1;
    }
    grassHeights[? grassId] = height;
}
return grassHeights[? grassId]


