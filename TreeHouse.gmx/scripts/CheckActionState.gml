distance = point_distance(x,y,oPlayer.x,oPlayer.y);
//check for transitions
prevState = state;
switch(state){
    case STATE_IDLE:
        if(distance < agroRange)
            state = STATE_AGRO; 
        break;
    case STATE_AGRO:
        if(distance <= attackRange)
            state = STATE_PREPARING;
        if(distance > agroRange)
            state = STATE_IDLE;
        if(enemyType == RANGED_ENEMY)
            if(distance < retreatRange-20)
                state = STATE_RETREATING;
        break;
    case STATE_PREPARING: 
        if(distance > attackRange*1.2)
            state = STATE_AGRO;
        if(distance > agroRange*1.2)
            state = STATE_IDLE;
        if(prepareTimer==0)
        {
            if(enemyType == CHARGING_ENEMY)
                state = STATE_INCHARGE;
            if(enemyType == RANGED_ENEMY)
            {
                if(distance < retreatRange * .8)
                    state = STATE_RETREATING;
            }
        }

        break;
    case STATE_RETREATING:
        if(distance > retreatRange)
            state = STATE_AGRO;
        break;
    case STATE_INCHARGE:
        if(chargeTimer==0)
            state = STATE_RESTING;
        break;
    case STATE_RESTING:
        if(restTimer==0){
            state = STATE_PREPARING;
        }
        break;
}

//initiate state
if(prevState != state || newGame){
    prepareTimer = -1;
    chargeTimer = -1;
    restTimer = -1;
    switch(state){
        case STATE_IDLE:
            alarm[0] = room_speed*(3 - random(1.5));
            //image_blend = c_green;
            break;
        case STATE_PREPARING:
            if(enemyType == RANGED_ENEMY)
                alarm[2] = room_speed * .8;
            dx = 0;
            dy = 0;
            prepareTimer = room_speed * .5;
            //image_blend = c_purple;
            break;
        case STATE_INCHARGE:
            dir = point_direction(x,y,oPlayer.x,oPlayer.y);
            dx = lengthdir_x(enemyMoveSpeed*3.5, dir);
            dy = lengthdir_y(enemyMoveSpeed*3.5, dir);
            chargeTimer = room_speed * 1;
            //image_blend = c_red;
            break;
        case STATE_RESTING:
            dx = 0;
            dy = 0;
            //image_blend = c_blue;
            restTimer = room_speed * 2;
            break;
    }
    newGame = false;
}

//update state
switch(state){
    case STATE_AGRO:
        SeekPlayer();
        break;
    case STATE_RETREATING:
        FleeFromPlayer();
    case STATE_PREPARING:
        if(enemyType == RANGED_ENEMY && shooting){
            if(frame < 11){
                shotThisCycle = false;
            }
            if(frame >= 11 && !shotThisCycle){
                instance_create(x,y,oProjectile);
                shotThisCycle = true;
            }
        }
        break;
}
