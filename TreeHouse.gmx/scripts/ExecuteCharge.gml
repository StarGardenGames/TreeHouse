dir = point_direction(x,y,oPlayer.x,oPlayer.y);
dx = lengthdir_x(enemyMoveSpeed*5, dir);
dy = lengthdir_y(enemyMoveSpeed*5, dir);

show_debug_message(dx);
show_debug_message(dy);
