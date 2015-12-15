var bul = instance_create(
    x + lengthdir_x(10,image_angle),
    y + lengthdir_y(10,image_angle),
    oPlayerProjectile
);

bul.direction = image_angle;
bul.image_angle = image_angle;
bul.speed = 20;
