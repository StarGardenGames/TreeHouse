var xx = argument0;

if(xx < 0) xx-=chunkSize;

xx -= xx % chunkSize;

return xx;


