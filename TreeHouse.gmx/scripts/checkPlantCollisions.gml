var playerX = oPlayer.x - oPlayer.x % gridSize;
var playerY = oPlayer.y - oPlayer.y % gridSize;

if(isPlant(playerX,playerY)){
    consumePlant(playerX,playerY);
}

if(isPlant(playerX+gridSize,playerY) && playerX!=oPlayer.x){
    consumePlant(playerX+gridSize,playerY);
}

if(isPlant(playerX,playerY+gridSize) && playerY!=oPlayer.y){
    consumePlant(playerX,playerY+gridSize);    
}

if(isPlant(playerX+gridSize,playerY+gridSize) && 
        playerX!=oPlayer.x && playerY!=oPlayer.y){
    consumePlant(playerX+gridSize,playerY+gridSize);    
    
}
