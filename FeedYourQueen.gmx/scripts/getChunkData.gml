var xx = argument0;
var yy = argument1;

data = ds_grid_create(blocksAlongChunk,blocksAlongChunk);

if(chunkExists(xx,yy)){
    str = chunkMap[? getChunkId(xx,yy)];
    ds_grid_read(data,str);
}else{
    generateChunk();
    chunkMap[? getChunkId(xx,yy)] =
        ds_grid_write(data);
}

return data;
