if(!instance_exists(oPlayer))
    exit;
return rectOverlap(
    oPlayer.x-16, oPlayer.y-16, oPlayer.x + 16, oPlayer.y + 16,
    x,y,x+chunkSize, y+chunkSize
);
