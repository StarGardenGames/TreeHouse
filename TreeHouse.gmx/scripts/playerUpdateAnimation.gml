//check for transitions
prevAnimState = animState;
switch(animState){
    case ANIM_MOVE:
        if(mouse_check_button_pressed(mb_left))
            animState = ANIM_MELEE;
        
        if(mouse_check_button_pressed(mb_right))
            animState = ANIM_SHOOT;
            
        var dist = point_distance(x,y,oQueen.x,oQueen.y);
        if(keyboard_check_pressed(ord('R')) && dist > 100 * oQueen.scale)
            animState = ANIM_RETURN; 
        
        break;
    case ANIM_MELEE:
        if((frame + frameSpeed) > 
                sprite_get_number(sprites[animState]))
            animState = ANIM_MOVE;
        break;
    case ANIM_SHOOT:
        if((frame + frameSpeed) > 
                sprite_get_number(sprites[animState]))
            animState = ANIM_MOVE;
        break;
    case ANIM_RETURN:
        if((frame + frameSpeed) > 
                sprite_get_number(sprites[animState]))
            animState = ANIM_MOVE;
        break;
}

//initiate state
if(prevAnimState != animState){
    frame = 0;
    sprite_index = sprites[animState];
}

//update state
frame += frameSpeed;

switch(animState){
case ANIM_RETURN:
    var dist = point_distance(x,y,oQueen.x,oQueen.y);
    if(keyboard_check(ord('R')) && dist > 100 * oQueen.scale){
        frame = 0;
    }else{
        if(frame == 0) frame = 10;
        frame += frameSpeed*2;
    }
    image_angle = point_direction(x,y,oQueen.x,oQueen.y);
    break;
case ANIM_MOVE:
    image_angle = point_direction(x,y,mouse_x,mouse_y);
    break;
default:

        
}
