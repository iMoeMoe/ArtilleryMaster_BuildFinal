#pragma once
#include <string>

class Map {
public:
    std::string name;
    std::string imagePath;
    int sizeX;
    int sizeY;
    std::string unit; // "Kilometres" or "Miles" (for Americans)
    double gridSize = 100.0; // size of one grid square in map units (meters, default 100)
};