#pragma once
#include "model/Map.h"

class MapDialog {
public:
    MapDialog();
    bool exec(); // Show dialog, return true if user added map
    Map getMap() const;

private:
    Map map;
    // Add a member to store grid size input
    double gridSize;
};