var i = 100037;
if(instance_exists(i) && 
    getChunkId(i.x,i.y) == 
    getChunkId(x,y)){
    print(id);    
}
if(chunkLoaded(x,y)){
    ds_map_replace(chunkMap, getChunkId(x,y),
            ds_grid_write(data));
            
    ds_map_delete(chunkLoadedSet,getChunkId(x,y));
    
    ds_grid_destroy(data);
}
