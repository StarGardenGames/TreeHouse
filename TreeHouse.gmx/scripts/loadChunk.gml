chunkLoadedSet[? getChunkId(x,y)] = id;

data = ds_grid_create(blocksAlongChunk,blocksAlongChunk);

if(chunkExists(x,y)){
    str = chunkMap[? getChunkId(x,y)];
    ds_grid_read(data,str);
}else{
    generateChunk();
    chunkMap[? getChunkId(x,y)] =
        ds_grid_write(data);
}


