var xx = (argument0 % blocksAlongChunk) + blocksAlongChunk;
var yy = (argument1 % blocksAlongChunk) + blocksAlongChunk;
var val = argument2;

data[# xx % blocksAlongChunk,
       yy % blocksAlongChunk] = val;
