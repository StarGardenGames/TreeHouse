if(invincibilityTimer != -1)
    invincibilityTimer = max(0,invincibilityTimer-1);
if(invincibilityTimer % 3 == 0)
{
    visible = false;
}
if(invincibilityTimer = 0) {
    invincible = false;
    visible = true;
}
