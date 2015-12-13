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
        }
    }
}
