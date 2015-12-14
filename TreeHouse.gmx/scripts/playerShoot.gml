if(!playerPerformingAction() && mouse_check_button_pressed(mb_right))
    playerShooting = true;

if(playerShooting){
    if(dx == 0 && dy == 0){
        if(frame <= 2 && frame + frameSpeed > 2)
           ShootPlayerProjectile();
    }else{
        ShootPlayerProjectile();
        playerShooting = false;
    }
}
