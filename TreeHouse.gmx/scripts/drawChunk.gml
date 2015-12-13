//draw grass
for(var i = 0; i < ds_grid_width(data); i++){
    for(var j = 0; j < ds_grid_height(data); j++){
        var type = getBlockTypeInData(i,j)
        var frame = getBlockFrameInData(i,j);
        var drawX = x + i * gridSize;
        var drawY = y + j * gridSize;
        switch(type){
            case GRASS:
                draw_sprite(sGrass,frame,drawX,drawY);
                break;
            case CREEP:
                draw_sprite(sCreep,frame,drawX,drawY);
                break;
        }
    }
}
//draw plants
for(var i = 0; i < ds_grid_width(data); i++){
    for(var j = 0; j < ds_grid_height(data); j++){
        var type = getBlockTypeInData(i,j)
        var frame = getBlockFrameInData(i,j);
        var drawX = x + i * gridSize;
        var drawY = y + j * gridSize;
        if(type == PLANT){
            if(frame == 0){
                draw_sprite_ext(sPlant_small,windFrame,drawX,drawY,
                    1.5,1.5,0,$FFFFFF,1);
            }
            if(frame == 1){
                draw_sprite_ext(sPlant_large,windFrame,drawX,drawY,
                    1.5,1.5,0,$FFFFFF,1);
            }
        }
    }
}
