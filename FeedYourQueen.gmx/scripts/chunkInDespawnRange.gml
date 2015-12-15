var xx = argument0;
var yy = argument1;

var leftSide = view_xview - chunkSize;
var rightSide = view_xview + view_wview + chunkSize;
var topSide = view_yview - chunkSize;
var botSide = view_yview + view_hview + chunkSize;

return !rectOverlap(
    leftSide, topSide, rightSide, botSide,
    xx,yy,xx+chunkSize,yy+chunkSize
);
