var i = (argument0 % blocksAlongChunk) + blocksAlongChunk;
var j = (argument1 % blocksAlongChunk) + blocksAlongChunk;

data[# i % blocksAlongChunk,j % blocksAlongChunk] = EMPTY;

