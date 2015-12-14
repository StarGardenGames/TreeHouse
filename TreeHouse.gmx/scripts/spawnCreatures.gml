var xx = argument0 * gridSize;
var yy = argument1 * gridSize;

if(random(1) < (1 / 2000)){
    var obj = instance_nearest(xx,yy,oParentEnemy);
    if(!instance_exists(oParentEnemy) ||
            point_distance(xx,yy,obj.x,obj.y) > 400){
        var theta = random(360);
        var enemyCount, enemyObj;
        enemyObj[0] = oGruntEnemy;
        enemyObj[1] = oArcherEnemy;
        enemyObj[2] = oChargeEnemy;
        for(var i = 0 ; i < 3; i++){enemyCount[i] = 0;}
        switch(irandom(4)){
        case 0: // grunt crowd
            enemyCount[0] = irandom_range(2,5); break;
        case 1: // arcer crowd
            enemyCount[1] = irandom_range(1,3); break;
        case 2: //loner charger
            enemyCount[2] = 1; break;
        case 3: // grunt & archer;
            enemyCount[0] = irandom_range(1,3);
            enemyCount[1] = 4 - enemyCount[0] + choose(0,1);
            break;
        case 4: //grunt & charger
            enemyCount[0] = irandom_range(1,2);
            enemyCount[2] = 1;
            break;
        }
        var numEnemies = enemyCount[0] + enemyCount[1] + enemyCount[2];
        for(var i = theta; i < theta + 360; i+= 360 / numEnemies){
            if(enemyCount[0] + enemyCount[1] + enemyCount[2] == 0)
                continue;
            var curEnemy;
            do{
                curEnemy = choose(0,1,2);
            }until(enemyCount[curEnemy]!=0);
            enemyCount[curEnemy]--;
            var dist = random_range(128,256);
            instance_create(
                xx + lengthdir_x(dist,i),
                yy + lengthdir_y(dist,i),
                enemyObj[curEnemy]
            );
        }
    }
}
