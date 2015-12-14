if(place_meeting(x,y,oQueen)){
    var dir = point_direction(x,y,oQueen.x,oQueen.y);
    x-=lengthdir_x(5,dir);
    y-=lengthdir_y(5,dir);  
}

