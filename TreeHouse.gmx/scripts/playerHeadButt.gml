if(!playerPerformingAction() && mouse_check_button_pressed(mb_left)){
    attacking = true;
    dx = 0;
    dy = 0;
}

if(attacking){
    if(oPlayer.frame < 11)
    {
        attackedThisCycle = false;
    }
    
    if(oPlayer.frame >= 11 &&  !attackedThisCycle)
    {
        ApplyHeadbuttDamage();
        attackedThisCycle = true;
    }
}
