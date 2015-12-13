//draw grass
for(var i = 0; i < ds_grid_width(data); i++){
    for(var j = 0; j < ds_grid_height(data); j++){
        var type = getBlockTypeInData(i,j)
        var frame = getBlockFrameInData(i,j);
        var drawX = x + i * gridSize;
        var drawY = y + j * gridSize;
        if(type == GRASS){
            draw_sprite(sGrass,frame,drawX,drawY);
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
                draw_sprite(sPlant_small,windFrame,drawX,drawY);
            }
            if(frame == 1){
                draw_sprite(sPlant_large,windFrame,drawX,drawY);
            }
        }
    }
}
