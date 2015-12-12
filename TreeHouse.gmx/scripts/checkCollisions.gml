if(!instance_exists(oPlayer))
    exit;

var playerX = oPlayer.x - oPlayer.x % gridSize;
var playerY = oPlayer.y - oPlayer.y % gridSize;

for(var i = 0; i <= gridSize; i+= gridSize){
    for(var j = 0; j <= gridSize; j+= gridSize){
        var xx = oPlayer.x + i;
        var yy = oPlayer.y + j;
        var type = getBlockType(xx,yy);
        switch(type){
        case PLANT:
            if(getBlockFrame(xx,yy)==1){
                setBlockFrame(xx,yy,0);
            }
            break;
        }
    }
}
