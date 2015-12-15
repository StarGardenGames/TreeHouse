if(!instance_exists(oParentEnemy))
    exit;
var damage = headbuttDamage;
var enemy = instance_nearest(x,y,oParentEnemy);
var directionTowardsEnemy = point_direction(x, y, enemy.x + 16, enemy.y + 16);
var angleTowardsEnemy = abs(directionTowardsEnemy - oPlayer.image_angle);
if(angleTowardsEnemy > 180) angleTowardsEnemy = 360 - angleTowardsEnemy;

var facingTowardsEnemy = angleTowardsEnemy < (30 * sprite_height/32);

var playerInAttackRange = point_distance(x, y, enemy.x, enemy.y) <= 100;
if(facingTowardsEnemy && playerInAttackRange)
{
    with(enemy)
    {
        currentHP -= damage;
        stunned = true;
        if(currentHP > 0)
            stunTimer = room_speed * maxHP/currentHP;
    }
}
