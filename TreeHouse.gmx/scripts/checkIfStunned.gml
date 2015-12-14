if(stunTimer != -1)
    stunTimer = max(0,stunTimer-1);
print(stunTimer + stunned);
if(stunTimer = 0)
{
    stunned = false;
}

if(stunned)
{
    image_blend = c_blue;
    dx = 0;
    dy = 0;
}
else
    image_blend = c_white;
