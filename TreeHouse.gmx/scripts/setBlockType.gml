var xx = argument0;
var yy = argument1;
var type = argument2;
var frame = argument3;

if(object_index != oChunk){
    setValueForBlock(xx,yy,type * 10 + frame);
}else{
    var i = (xx-x) div gridSize;
    var j = (yy-y) div gridSize;
    data[# i, j] = type * 10 + frame;
}

if(type == CREEP){
    if(oQueen.scale!=1 && random(1) < .02 && !place_meeting(xx,yy,oCreepPod)
            && !place_meeting(xx,yy,oQueen)){
        instance_create(xx,yy,oCreepPod);
    }
}


