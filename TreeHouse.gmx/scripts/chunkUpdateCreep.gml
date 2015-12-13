for(var i = 0; i < ds_grid_width(data); i++){
    for(var j = 0; j < ds_grid_height(data); j++){
        var xx = x+i*gridSize+gridSize/2;
        var yy = y+j*gridSize+gridSize/2;
        if(point_distance(xx,yy,oQueen.x,oQueen.y) < 150 * oQueen.scale){
            if(getBlockType(xx,yy)!=CREEP){
                setBlockType(xx,yy,CREEP,irandom(3));
            }
        }
    }
}
