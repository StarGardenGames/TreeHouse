if(!instance_exists(oPlayer))
    exit;

for(var i = 0; i <= gridSize; i+= gridSize){
    for(var j = 0; j <= gridSize; j+= gridSize){
        var xx = oPlayer.x-16 + i;
        var yy = oPlayer.y-16 + j;
        var type = getBlockType(xx,yy);
        var frame = getBlockFrame(xx,yy);
        switch(type){
        case PLANT:
            var orbCount;
            if(frame == 0){ orbCount = 3; }
            if(frame == 1){ orbCount = 5; }
            setBlockType(xx,yy,GRASS,frame);
            var px = oPlayer.x-16;
            var py = oPlayer.y-16;
            px -= px%gridSize;
            py -= py%gridSize;
            repeat(orbCount){
                instance_create(px,py,oOrb);
            } 
            break;
        case PLANT_RED:
        case PLANT_GREEN:
        case PLANT_PURPLE:
            var orbs = 10;
            var hungOrbs = 5;
            setBlockType(xx,yy,EMPTY,0);
            var px = oPlayer.x-16;
            var py = oPlayer.y-16;
            px -= px%gridSize;
            py -= py%gridSize;
            repeat(orbs){
                instance_create(px,py,oOrb);
            }
            repeat(hungOrbs){
                instance_create(px,py,oHungerOrb);
            }
            break;
        }
    }
}
