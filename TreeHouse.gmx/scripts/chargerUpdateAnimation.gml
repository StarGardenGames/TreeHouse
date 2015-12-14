//check for transitions
prevAnimState = animState;
if(!instance_exists(oPlayer)) animState = ANIM_IDLE;
switch(state){ //charger follows state machine closely
    case STATE_IDLE:
    case STATE_AGRO:
        if(instance_exists(oPlayer))
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
    switch(animState){
    case ANIM_PREP:
        targetDir = point_direction(x,y,oPlayer.x,oPlayer.y);
        break;
    }
}

//update state
frame += frameSpeed;

switch(animState){
case ANIM_MOVE:
case ANIM_CHARGE:
    image_angle = point_direction(0,0,dx,dy);
case ANIM_IDLE:
    frame %= sprite_get_number(sprites[animState]);
    break; 
case ANIM_PREP:
    frame %= sprite_get_number(sprites[animState]);
    var dTheta = targetDir - image_angle;
    var seg = floor((dTheta / 180) + 2);
    var sig = seg == 0 || seg == 2;
    sig = (sig * 2) - 1;
    image_angle += sig * min(20, min(abs(dTheta),360 - abs(dTheta)));
    image_angle = (image_angle + 360) % 360;
    break;
}
