#pragma once
#include "model/Projectile.h"

class ProjectileDialog {
public:
    ProjectileDialog();
    bool exec(); // Show dialog, return true if user added projectile
    Projectile getProjectile() const;

private:
    Projectile projectile;
};