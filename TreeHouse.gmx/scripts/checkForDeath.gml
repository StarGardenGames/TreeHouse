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
}

if(dead){
    instance_destroy();
}
