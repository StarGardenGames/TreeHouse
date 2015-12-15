var left = view_xview - (view_xview % chunkSize);
var top = view_yview - (view_yview % chunkSize);
var right = left + view_wview + chunkSize;
var bot = top + view_hview + chunkSize;

if(view_xview < 0) left -= chunkSize;
if(view_yview < 0) top -= chunkSize;

for(var i = left; i < right; i+= chunkSize){
    for(var j = top; j < bot; j+= chunkSize){
        if(!chunkLoaded(i,j)){
            instance_create(i,j,oChunk);
        }
    }
}
