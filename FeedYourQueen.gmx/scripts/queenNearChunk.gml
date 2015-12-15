var halfWidth = oQueen.sprite_width / 2;
var halfHeight = oQueen.sprite_height / 2;
return rectOverlap(
    oQueen.x-halfWidth, oQueen.y-halfHeight, oQueen.x + halfWidth, oQueen.y + halfHeight,
    x-chunkSize,y-chunkSize,x+chunkSize*2, y+chunkSize*2
);
