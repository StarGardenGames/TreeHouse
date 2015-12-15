switch(chargeState) {
    case STATE_PREPARING:
        dx = 0;
        dy = 0;
        if(!alarmSet)
            alarm[1] = room_speed * .25;
        alarmSet = true;
        break;
    case STATE_INCHARGE:
        if(!alarmSet)
            alarm[3] = room_speed * .5;
        alarmSet = true;
        break;
    case STATE_RESTING:
        dx = 0;
        dy = 0;
        if(!alarmSet)
            alarm[4] = room_speed * 2; 
        alarmSet = true;
        break;
}
