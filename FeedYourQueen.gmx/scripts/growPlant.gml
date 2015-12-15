//global grid space
var xx = argument0;
var yy = argument1;

var px = (xx + plantOffset)/plantScale;
var py = (yy + plantOffset)/plantScale;
var noise = perlinNoise2D(px,py, 100);
var isPlant = noise > 70;
var isBigPlant = noise > 80;
if(isBigPlant){
    setBlockInChunk( xx, yy, createBlockValue(PLANT,1));
}else if(isPlant){
    setBlockInChunk( xx, yy, createBlockValue(PLANT,0));
}
