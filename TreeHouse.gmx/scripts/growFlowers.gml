//global grid space
var xx = argument0;
var yy = argument1;

var blocksPerFlower = blocksAlongChunk * blocksAlongChunk * 30;

if(irandom(blocksPerFlower) == 0){
    var type = choose(PLANT_PURPLE, PLANT_RED, PLANT_GREEN);
    setBlockInChunk( xx, yy, createBlockValue(type,choose(0,1)));
}
