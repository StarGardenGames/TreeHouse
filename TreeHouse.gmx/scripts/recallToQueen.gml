var queenDist = point_distance(x,y,oQueen.x,oQueen.y);
var thresh = 100*oQueen.scale;
recalling = keyboard_check(ord('R')) && queenDist > thresh;

if(recalling){
    var dir = point_direction(x,y,oQueen.x,oQueen.y);
    x += lengthdir_x(min(50,queenDist),dir);
    y += lengthdir_y(min(50,queenDist),dir);
}
