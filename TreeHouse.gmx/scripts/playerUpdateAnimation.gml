//check for transitions
prevAnimState = animState;
switch(animState){
    case ANIM_IDLE:
    case ANIM_MOVE:
        var mag = point_distance(0,0,dx,dy);
        if(mag > .2 && animState == ANIM_IDLE){
            animState = ANIM_MOVE;
        }
        if(mag < .2 && animState == ANIM_MOVE){
            animState = ANIM_IDLE;
        }
    
        if(attacking)
            animState = ANIM_MELEE;
        
        if(playerShooting && animState != ANIM_MOVE)
            animState = ANIM_SHOOT;
        
        if(dying)
            animState = ANIM_DYING;
            
        var dist = point_distance(x,y,oQueen.x,oQueen.y);
        if(recalling)
            animState = ANIM_RETURN;
        
        
        break;
    case ANIM_MELEE:
        if((frame + frameSpeed) > 
                sprite_get_number(sprites[animState]))
            animState = ANIM_MOVE;
        break;
    case ANIM_SHOOT:
        if((frame + frameSpeed) > 
                sprite_get_number(sprites[animState])){
            var mag = point_distance(0,0,dx,dy);
            if(mag > .2){
                animState = ANIM_MOVE;
            }else{
                animState = ANIM_IDLE;
            }
        }
        break;
    case ANIM_RETURN:
        if((frame + frameSpeed) > 
                sprite_get_number(sprites[animState]))
            animState = ANIM_MOVE;
        break;
}

//initiate state
if(prevAnimState != animState || newGame){
    frame = 0;
    sprite_index = sprites[animState];
    newGame = false;
    switch(animState){
        case ANIM_MELEE: frame = 7; break;
    }
}

//update state
frame += frameSpeed;

switch(animState){
case ANIM_RETURN:
    var dist = point_distance(x,y,oQueen.x,oQueen.y);
    if(recalling){
        if(frame>=25) frame-=24;
        frame+= 2* frameSpeed
    }else{
        if(frame < 32) frame = 32;
    }
    image_angle = point_direction(x,y,oQueen.x,oQueen.y);
    break;
    
case ANIM_IDLE:
    frame = 0;
    image_angle = point_direction(x,y,mouse_x,mouse_y);
    break;
    
case ANIM_MOVE:
    image_angle = point_direction(x,y,mouse_x,mouse_y);
    frame+= frameSpeed;
    frame %= sprite_get_number(sprite_index);
    break;
    
case ANIM_MELEE:
    if((frame + frameSpeed) > 
        sprite_get_number(sprites[animState]))
    attacking = false;
    break;
    
case ANIM_SHOOT:
    if((frame + frameSpeed) > 
        sprite_get_number(sprites[animState]))
    playerShooting = false;
    break;
    
case ANIM_DYING:
    if((frame + frameSpeed) > 
            sprite_get_number(sprites[animState]))
        dead = true;
    break;
default:
        
}
