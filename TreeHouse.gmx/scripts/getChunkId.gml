var xx = argument0;
var yy = argument1;

xx = getChunkCoord(xx) div chunkSize;
yy = getChunkCoord(yy) div chunkSize;

return string(xx)+"_"+string(yy);
