var enemyX = x;
var enemyY = y;
var enemySpriteHeight = sprite_height;
var enemySpriteWidth = sprite_width;
var currentState = state;
var enemyDamage = damage;

with(oPlayer)
{
    if(rectOverlap(enemyX,enemyY,enemyX+enemySpriteWidth,enemyY+enemySpriteHeight,
    oPlayer.x-16, oPlayer.y-16, oPlayer.x-16 + oPlayer.sprite_width, oPlayer.y-16 + oPlayer.sprite_height))
    {
        if(currentState = STATE_INCHARGE)
        {
            if(!invincible)
            {
                currentHP -= enemyDamage;
                invincibilityTimer = room_speed*2;
                invincible = true;
            }
            instance_create(x,y,oShakeControl);
        }
    }
}
