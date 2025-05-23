#include "ProjectileDialog.h"
#include <windows.h>
#include <commdlg.h>
#include <string.h>

ProjectileDialog::ProjectileDialog() {
    projectile.name = "";
    projectile.velocity = 0.0;
    projectile.imagePath = "";
}

bool ProjectileDialog::exec() {
    // For demonstration, just prompt for name and velocity using MessageBox/InputBox
    char name[64] = "";
    char velocityStr[32] = "";
    if (MessageBoxA(NULL, "Add new projectile?", "Projectile", MB_OKCANCEL) != IDOK)
        return false;
    // Simulate input
    strcpy(name, "3BK-10");
    strcpy(velocityStr, "900");
    projectile.name = name;
    projectile.velocity = atof(velocityStr);
    projectile.imagePath = ""; // Not implemented
    return true;
}

Projectile ProjectileDialog::getProjectile() const {
    return projectile;
}