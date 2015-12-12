var randomizedMoveSpeed = enemyMoveSpeed - random(enemyMoveSpeed);
dir = random(360);
dx = lengthdir_x(randomizedMoveSpeed, dir);
dy = lengthdir_y(randomizedMoveSpeed, dir);
