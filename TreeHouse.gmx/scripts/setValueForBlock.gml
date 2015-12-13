var xx = argument0;
var yy = argument1;
var newVal = argument2;

var i = (xx - x) div gridSize;
var j = (yy - y) div gridSize;        
data[# i, j] = newVal;
/*
if(chunkLoaded(xx,yy)){
    with(chunkLoadedSet[? getChunkId(xx,yy)]){
        var i = (xx - x) div gridSize;
        var j = (yy - y) div gridSize;        
        data[# i, j] = newVal;
    }
}else{
    data = getChunkData(xx,yy);
    var i = (xx - getChunkCoord(xx)) div gridSize;
    var j = (yy - getChunkCoord(yy)) div gridSize;
    data[# i,j]=newVal;
    ds_map_replace(chunkMap, getChunkId(x,y),
            ds_grid_write(data));
    ds_grid_destroy(data);   
}*/
