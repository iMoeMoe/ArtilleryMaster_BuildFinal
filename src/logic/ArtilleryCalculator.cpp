#include "ArtilleryCalculator.h"
#include <cmath>

// Returns elevation angle in degrees for given distance, velocity, and arch type
double ArtilleryCalculator::calculateElevation(double distance, double velocity, bool highArch) {
    const double g = 9.80665;
    double v2 = velocity * velocity;
    double arg = (g * distance) / v2;
    if (arg > 1.0) return 0.0; // out of range so if someone try to shoot too far, don't let them
    double theta = 0.5 * asin(arg);
    if (highArch)
        theta = M_PI_2 - theta;
    return theta * 180.0 / M_PI;
}

double ArtilleryCalculator::calculateAzimuth(double x0, double y0, double x1, double y1) {
    double dx = x1 - x0;
    double dy = y1 - y0;
    double angle = atan2(dx, -dy) * 180.0 / M_PI;
    if (angle < 0) angle += 360.0;
    return angle;
}

double ArtilleryCalculator::calculateTimeOfFlight(double distance, double velocity, bool highArch) {
    double theta = calculateElevation(distance, velocity, highArch) * M_PI / 180.0;
    double t = distance / (velocity * cos(theta));
    return t;
}