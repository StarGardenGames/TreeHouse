if(!instance_exists(oPlayer))
    exit;

for(var i = 0; i <= gridSize; i+= gridSize){
    for(var j = 0; j <= gridSize; j+= gridSize){
        var xx = oPlayer.x-16 + i;
        var yy = oPlayer.y-16 + j;
        var type = getBlockType(xx,yy);
        switch(type){
        case PLANT:
            setBlockType(xx,yy,GRASS,getBlockFrame(xx,yy));   
            break;
        }
    }
}
