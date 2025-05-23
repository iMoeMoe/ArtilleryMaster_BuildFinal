#pragma once
#include <string>
#include "model/Projectile.h"
#include "model/Map.h"

struct TargetRegistrationPoint {
    std::string name;
    float weaponX, weaponY;
    float targetX, targetY;
};

class Firemission