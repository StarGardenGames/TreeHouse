var startX = view_xview - view_xview % gridSize;
var startY = view_yview - view_yview % gridSize;
if(startX < 0) startX -= gridSize;
if(startY < 0) startY -= gridSize;
var endX = view_xview + view_wview;
var endY = view_yview + view_hview;
for(var i = startX; i < endX; i += gridSize){
    for(var j = startY; j < endY; j += gridSize){
        var status = getPlantStatus(i, j)
        if(status != NULL){
            draw_sprite(sPlant,status,i,j);
        }
    }
}
