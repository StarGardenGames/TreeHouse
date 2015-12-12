switch(enemyType)
{
    case STANDARD_ENEMY:
        break;
    case RANGED_ENEMY:
        alarm[2] = room_speed * .5;
        alarmSet = true;
        break;
    case CHARGING_ENEMY:
        dx = 0;
        dy = 0;
        alarm[1] = room_speed * .25;
        alarmSet = true;
        break;
}
