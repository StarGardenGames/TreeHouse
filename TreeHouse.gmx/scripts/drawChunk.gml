for(var i = 0; i < ds_grid_width(data); i++){
    for(var j = 0; j < ds_grid_height(data); j++){
        var spr = NULL;
        switch(getBlockTypeInData(i,j)){
            case GRASS: spr = sGrass; break;
            case PLANT: spr = sPlant; break;
        }
        if(spr != NULL){
            draw_sprite(
                spr,getBlockFrameInData(i,j),
                x + i * gridSize,
                y + j * gridSize);
        }
    }
}
