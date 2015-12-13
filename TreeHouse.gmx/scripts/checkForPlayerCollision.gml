var enemyX = x;
var enemyY = y;
var enemySpriteHeight = sprite_height;
var enemySpriteWidth = sprite_width;

with(oPlayer)
{
    if(rectOverlap(enemyX,enemyY,enemyX+enemySpriteWidth,enemyY+enemySpriteHeight,
    oPlayer.x-16, oPlayer.y-16, oPlayer.x-16 + oPlayer.sprite_width, oPlayer.y-16 + oPlayer.sprite_height))
    {
        instance_destroy();
    }
}
