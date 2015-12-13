if(!instance_exists(oPlayer))
    exit;
return rectOverlap(
    oPlayer.x, oPlayer.y, oPlayer.x + 32, oPlayer.y + 32,
    x,y,x+chunkSize, y+chunkSize
);
