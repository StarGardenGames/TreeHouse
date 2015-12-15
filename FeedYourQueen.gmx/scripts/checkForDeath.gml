if(currentHP <= 0) {
    dying = true;
}

if(dying){
    repeat(4){
        if(inv > 0){
            instance_create(x,y,oOrb)
            inv--;
        }
    }
    repeat(2){
        if(hungInv > 0){
            instance_create(x,y,oHungerOrb)
            hungInv--;
        }
    }
}

if(dead){
    instance_destroy();
}
