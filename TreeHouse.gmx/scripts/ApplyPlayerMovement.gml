var moveSpeed = 9;
var slowDownFrames = 5;
var skidCoef = moveSpeed/slowDownFrames;
var inputx = keyboard_check(ord('D')) - keyboard_check(ord('A'));
var inputy = keyboard_check(ord('S'))- keyboard_check(ord('W'));

if(point_distance(0,0,inputx,inputy) == sqrt(2))
{
    moveSpeed = moveSpeed/sqrt(2);
}

if(inputy != 0)
    dy = inputy*moveSpeed;
if(inputx != 0)
    dx = inputx*moveSpeed;

if(dy != 0 && inputy == 0)
{
    if(dy > skidCoef)
        dy -= skidCoef;
    else if(dy < -skidCoef)
        dy += skidCoef;
    else
        dy = 0;
        
}
if(dx != 0 && inputx == 0)
{
    if(dx > skidCoef)
        dx -= skidCoef;
    else if(dx < -skidCoef)
        dx += skidCoef;
    else
        dx = 0;
}

y += dy;
x += dx;


