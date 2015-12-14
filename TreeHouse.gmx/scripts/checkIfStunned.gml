if(stunTimer != -1)
    stunTimer = max(0,stunTimer-1);

if(stunTimer = 0)
{
    stunned = false;
}

if(stunned)
{
    attacking = false;
    state = STATE_IDLE;
    animState = ANIM_IDLE;
    image_blend = $FFAAAA;
    dx = 0;
    dy = 0;
}
else
    image_blend = c_white;
