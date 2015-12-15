var xx = argument0;
var yy = argument1;

var value;

var i = (xx - x) div gridSize;
var j = (yy - y) div gridSize;
value = data[# i, j];

/*if(chunkLoaded(xx,yy)){
    with(chunkLoadedSet[? getChunkId(xx,yy)]){
        var i = (xx - x) div gridSize;
        var j = (yy - y) div gridSize;
        value = data[# i, j];
    }
}else{
    data = getChunkData(xx,yy);
    var i = (xx - getChunkCoord(xx)) div gridSize;
    var j = (yy - getChunkCoord(yy)) div gridSize;
    value = data[# i,j];
    ds_grid_destroy(data);
}*/
return value;
