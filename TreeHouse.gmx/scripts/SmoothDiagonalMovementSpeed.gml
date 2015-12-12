var dx = keyboard_check(ord('D')) - keyboard_check(ord('A'));
var dy = keyboard_check(ord('S'))- keyboard_check(ord('W'));

if(point_distance(0,0,dx,dy) == sqrt(2))
{
    x += 2.5;
    y += 2.5;
}
