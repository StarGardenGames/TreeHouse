var distToQueen = point_distance(x,y,oQueen.x,oQueen.y);
var orbRate = max(4,min(30,.2 * inv));
if(distToQueen < 100 * oQueen.scale){
    if(inv >= orbRate){    
        repeat(orbRate){
            instance_create(x,y,oOrb);
        }
        inv-=orbRate;
    }
    orbRate = max(2,min(15,.2 * hungInv));
    if(hungInv >= orbRate){    
        repeat(orbRate){
            instance_create(x,y,oHungerOrb);
        }
        hungInv-=orbRate;
    }
}
