//check for transitions
prevAnimState = animState;
switch(state){ //charger follows state machine closely
    case STATE_IDLE:
    case STATE_AGRO:
        animState = ANIM_MOVE;
        break;
    case STATE_PREPARING:
        animState = ANIM_PREP;
        break;
    case STATE_INCHARGE:
        animState = ANIM_CHARGE;
        break;
    case STATE_RESTING:
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
case ANIM_CHARGE:
    image_angle = point_direction(0,0,dx,dy);
case ANIM_PREP:
case ANIM_IDLE:
    frame %= sprite_get_number(sprites[animState]);
    break; 
}
