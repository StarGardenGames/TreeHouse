var xx = argument0;
var yy = argument1;

var grassId = string(xx)+"_"+string(yy);
if(!ds_map_exists(grassHeights,grassId)){
    var height;
    var scale = .75;
    var noise = perlinNoise2D(xx/scale, yy/scale, 100);
    if(noise > 40){
        height = (noise - 40) div 15;
        height = min(numGrassFrames-1,height);
    }else{
        height = -1;
    }
    grassHeights[? grassId] = height;
}

return grassHeights[? grassId];


