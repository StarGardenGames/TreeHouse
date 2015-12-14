//check for transitions
prevAnimState = animState;
if(!instance_exists(oPlayer)) animState = ANIM_MOVE;
switch(animState){
    case ANIM_MOVE:
        if(!instance_exists(oPlayer)){
        
        }
        break;
    case ANIM_MELEE:
        if((frame + frameSpeed) > 
                sprite_get_number(sprites[animState]))
            animState = ANIM_IDLE;
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
case ANIM_MOVE:
    image_angle = point_direction(0,0,dx,dy);
    frame %= sprite_get_number(sprites[animState]);
    break;
case ANIM_MELEE:
    break;
default:       
}
