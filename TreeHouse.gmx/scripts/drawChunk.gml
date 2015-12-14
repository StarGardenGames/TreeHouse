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
        switch(type){
        case PLANT:
            if(frame == 0){
                draw_sprite(sPlant_small,windFrame,drawX,drawY);
            }
            if(frame == 1){
                draw_sprite(sPlant_large,windFrame,drawX,drawY);
            }
            break;
        case PLANT_RED:
            draw_sprite_ext(sPlantPurple,0,drawX,drawY,
                1.5,1.5,0,$FFFFFF,1);
            break;
        case PLANT_GREEN:
            draw_sprite_ext(sPlantGreen,0,drawX,drawY,
                1.5,1.5,0,$FFFFFF,1);
            break;
        case PLANT_PURPLE:
            draw_sprite_ext(sPlantRed,0,drawX+gridSize/2,drawY+gridSize/2,
                1.5,1.5,frame * 180,$FFFFFF,1);
            
            break;
        }
    }
}
