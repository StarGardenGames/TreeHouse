//read in arguments
var o1x1=argument0;
var o1y1=argument1;
var o1x2=argument2;
var o1y2=argument3;
var o2x1=argument4;
var o2y1=argument5;
var o2x2=argument6;
var o2y2=argument7;
//computer output
if(     
    o1x2>o2x1 &&
    o1x1<o2x2 &&
    o1y2>o2y1 &&
    o1y1<o2y2 
)
    return true;
else 
    return false;
