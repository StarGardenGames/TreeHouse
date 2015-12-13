if(!instance_exists(oParentEnemy))
    exit;
var damage = currentPlayerDamage;
var enemy = instance_nearest(x,y,oParentEnemy);
var directionTowardsEnemy = point_direction(x, y, enemy.x + 16, enemy.y + 16);
var angleTowardsEnemy = abs(directionTowardsEnemy - oPlayer.image_angle);
if(angleTowardsEnemy > 180) angleTowardsEnemy = 360 - angleTowardsEnemy;

var facingTowardsEnemy = angleTowardsEnemy < 40;

var playerInAttackRange = point_distance(x, y, enemy.x + 16, enemy.y +16) <= 500;
if(facingTowardsEnemy && playerInAttackRange)
{
    print("HIT");
    with(enemy)
    {
        currentHP -= damage;
        if(currentHP < 0)
            instance_destroy();
        print(currentHP);
    }
}
