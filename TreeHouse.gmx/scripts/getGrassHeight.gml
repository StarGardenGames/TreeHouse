var xx = argument0;
var yy = argument1;

var grassId = string(xx)+"_"+string(yy);
if(!ds_map_exists(grassCutTime,grassId)){
    var cutTime;
    var scale = .75;
    var noise = perlinNoise2D(xx/scale, yy/scale, 100);
    if(noise > 40){
        var frame = (noise - 40) div 15;
        frame = min(numGrassFrames-1,frame);
        var timeGrown = (frame / (numGrassFrames-1)) * totalGrowthTime;
        cutTime = current_time - (timeGrown*1000);
        cutTime += irandom(10000);
    }else{
        cutTime = -1;
    }
    
    grassCutTime[? grassId] = cutTime;
}

var cutTime = grassCutTime[? grassId];
var timePerFrame = (totalGrowthTime / numGrassFrames) * 1000;
if(cutTime == -1){
    return -1;
}else{
    return min(numGrassFrames-1,
        (current_time - cutTime) div timePerFrame);
}

