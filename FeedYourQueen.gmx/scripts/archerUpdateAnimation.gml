//check for transitions
prevAnimState = animState;
if(!instance_exists(oPlayer)) animState = ANIM_MOVE;

if(animState != ANIM_DYING && currentHP < 0){
    animState = ANIM_DYING;
}
switch(animState){
    case ANIM_MOVE:
        if(instance_exists(oPlayer)){
            if(shooting) 
                animState = ANIM_SHOOT;
            
            if(dx == 0 && dy == 0) 
                animState = ANIM_IDLE;
        }
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
    if(dx!=0 || dy!=0)
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
    break;
case ANIM_DYING:
    frame = 0;
    image_alpha = max(0,image_alpha - .05);
    var r = color_get_red(image_blend);
    var g = color_get_green(image_blend);
    var b = color_get_blue(image_blend);
    image_blend = make_color_rgb(
        r, max(0,g-5), max(0,b-5));
    break;
}
