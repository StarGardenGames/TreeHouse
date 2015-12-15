//global grid space
var offsetX = argument0;
var offsetY = argument1;
var val = argument2;

offsetX %= blocksAlongChunk;
offsetY %= blocksAlongChunk;

if(offsetX < 0) offsetX += blocksAlongChunk;
if(offsetY < 0) offsetY += blocksAlongChunk;

data[# offsetX, offsetY] = val;
