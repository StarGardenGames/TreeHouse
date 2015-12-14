var gridX = x div gridSize;
var gridY = y div gridSize;
for(var i = 0; i < ds_grid_width(data); i++){
    for(var j = 0; j < ds_grid_height(data); j++){
        var curX = gridX + i;
        var curY = gridY + j;
        emptyBlock(curX, curY);
        growGrass(curX, curY);
        growPlant(curX, curY);
        growFlowers(curX, curY);
    }
}
