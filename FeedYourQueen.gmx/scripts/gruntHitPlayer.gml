var distance 
if(!instance_exists(oPlayer))
    exit;

var dist = point_distance(x,y,oPlayer.x,oPlayer.y);

if(dist < 50){
    var dir = point_direction(x,y,oPlayer.x,oPlayer.y);
    var diff = abs(dir - image_angle);
    diff = min(diff,360 - diff);
    if(diff < 40){
        with(oPlayer){
        if(!invincible){
            currentHP -= 10;
            invincibilityTimer = room_speed*2;
            invincible = true;
        }
    }
    }
}
