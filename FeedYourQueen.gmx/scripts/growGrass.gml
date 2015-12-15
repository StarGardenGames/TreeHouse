//global grid space
var xx = argument0;
var yy = argument1;

var numGrassFrames = 4;

var px = (xx + grassOffset)/grassScale;
var py = (yy + grassOffset)/grassScale;
var noise = perlinNoise2D(px,py, 100);
var isGrass = noise > 40;
if(isGrass){
    var height = round((noise - 40) / 12);
    height = min(numGrassFrames-1,height);
    setBlockInChunk( xx, yy, createBlockValue(GRASS,height) );
}
