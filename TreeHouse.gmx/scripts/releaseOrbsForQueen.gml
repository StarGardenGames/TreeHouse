var distToQueen = point_distance(x,y,oQueen.x,oQueen.y);
var orbRate = max(4,min(30,.2 * inv));
if(distToQueen < 100 * oQueen.scale && inv >= orbRate){    
    repeat(orbRate){
        instance_create(x,y,oOrb);
    }
    inv-=orbRate;
}
