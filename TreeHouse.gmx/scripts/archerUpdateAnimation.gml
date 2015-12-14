//check for transitions
prevAnimState = animState;
switch(animState){
    case ANIM_MOVE:
        if(shooting) 
            animState = ANIM_SHOOT;
        
        if(dx == 0 && dy == 0) 
            animState = ANIM_IDLE;
        
        break;
    case ANIM_IDLE:
        if(shooting) 
            animState = ANIM_SHOOT;
        
        if(dx != 0 || dy != 0) 
            animState = ANIM_MOVE;
    case ANIM_SHOOT:
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
case ANIM_IDLE:
    frame = 0;    
    break;
case ANIM_SHOOT:
    image_angle = point_direction(x,y,oPlayer.x,oPlayer.y);
    if((frame + frameSpeed) > 
        sprite_get_number(sprites[animState])){
        shooting = false;
    }
default:       
}
