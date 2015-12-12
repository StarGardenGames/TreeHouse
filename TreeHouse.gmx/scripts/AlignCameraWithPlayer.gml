var tweenFactor = .1;

var viewCenterX, viewCenterY, 
    centerAxis, centerSprite, 
    enemyW, enemyH;
//dimensions
var targetW = 1280;
var targetH = 720;

//position
var targetX = x - view_wview/2 + sprite_width/2;
var targetY = y - view_hview/2 + sprite_height/2;

//tweening algorithm for smooth camera movement
view_wview += (targetW - view_wview) * tweenFactor;
view_hview += (targetH - view_hview) * tweenFactor;

view_xview += (targetX - view_xview) * tweenFactor;
view_yview += (targetY - view_yview) * tweenFactor;
