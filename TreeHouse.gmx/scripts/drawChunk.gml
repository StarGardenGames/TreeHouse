for(var i = 0; i < ds_grid_width(data); i++){
    for(var j = 0; j < ds_grid_height(data); j++){
        var type = getBlockTypeInData(i,j)
        var frame = getBlockFrameInData(i,j);
        var drawX = x + i * gridSize;
        var drawY = y + j * gridSize;
        switch(type){
            case GRASS: draw_sprite_ext(sGrass,frame,drawX,drawY,2,2,0,c_white,1); break;
            case PLANT: 
                drawX-=gridSize/4;
                drawY-=gridSize/4;
                if(frame == 0){
                    draw_sprite_ext(sPlant_small,windFrame,drawX,drawY,2,2,0,c_white,1);
                }
                if(frame == 1){
                    draw_sprite_ext(sPlant_large,windFrame,drawX,drawY,2,2,0,c_white,1);
                }
                break;
        }
        
    }
}
