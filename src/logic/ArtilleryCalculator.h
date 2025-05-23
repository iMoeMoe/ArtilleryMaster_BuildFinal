#pragma once
#include "model/Projectile.h"

class ArtilleryCalculator {
public:
    static double calculateElevation(double distance, double velocity, bool highArch);
    static double calculateAzimuth(double x0, double y0, double x1, double y1);
    static double calculateTimeOfFlight(double distance, double velocity, bool highArch);
};