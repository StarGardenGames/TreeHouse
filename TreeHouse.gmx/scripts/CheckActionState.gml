distance = point_distance(x,y,oPlayer.x,oPlayer.y);
//check for transitions
prevState = state;
switch(state){
    case STATE_IDLE:
        if(distance < agroRange)
            state = STATE_AGRO; 
        break;
    case STATE_AGRO:
        if(distance < attackRange)
            state = STATE_ATTACK;
        if(distance > agroRange)
            state = STATE_IDLE;
        break;
    case STATE_ATTACK:
        if(distance > attackRange)
            state = STATE_AGRO;
        if(distance > agroRange)
            state = STATE_IDLE;
        break;
}

//initiate state
if(prevState != state || newGame){
    switch(state){
        case STATE_IDLE:
            image_blend = c_green;
            break;
        case STATE_AGRO:
            image_blend = c_yellow;
            break;
        case STATE_ATTACK:
            image_blend = c_red;
            break;
    }
    newGame = false;
}
//update state
switch(state){
    case STATE_IDLE:
        SetWanderAlarm();
        break;
    case STATE_AGRO:
        SeekPlayer();
        break;
    case STATE_ATTACK:
        UseEnemyAbilities();
        break;
}
