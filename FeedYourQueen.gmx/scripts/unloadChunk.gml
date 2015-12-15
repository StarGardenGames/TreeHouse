if(chunkLoaded(x,y)){
    ds_map_replace(chunkMap, getChunkId(x,y),
            ds_grid_write(data));
            
    ds_map_delete(chunkLoadedSet,getChunkId(x,y));
    
    ds_grid_destroy(data);
}
