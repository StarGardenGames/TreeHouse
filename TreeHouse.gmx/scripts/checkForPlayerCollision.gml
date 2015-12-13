var enemyX = x;
var enemyY = y;
var enemySpriteHeight = sprite_height;
var enemySpriteWidth = sprite_width;

with(oPlayer)
{
    if(rectOverlap(enemyX,enemyY,enemyX+enemySpriteWidth,enemyY+enemySpriteHeight,
    oPlayer.x, oPlayer.y, oPlayer.x + oPlayer.sprite_width, oPlayer.y + oPlayer.sprite_height))
    {
        instance_destroy();
    }
}
