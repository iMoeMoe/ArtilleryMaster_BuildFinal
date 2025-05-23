#include "MapDialog.h"
#include <windows.h>
#include <commdlg.h>
#include <string.h>

MapDialog::MapDialog() {
    map.name = "";
    map.sizeX = 1000;
    map.sizeY = 1000;
    map.imagePath = "";
    map.unit = "Kilometres";
    map.gridSize = 100.0; // default
}

bool MapDialog::exec() {
    // For demonstration, just prompt for name, size, and unit using MessageBox/InputBox
    char name[64] = "";
    if (MessageBoxA(NULL, "Add new map?", "Map", MB_OKCANCEL) != IDOK)
        return false;
    strcpy(name, "Muddy Fields");
    map.name = name;
    map.sizeX = 2000;
    map.sizeY = 2000;
    // Simulate unit selection
    int unitChoice = MessageBoxA(NULL, "Use Kilometres? (No = Miles)", "Map Unit", MB_YESNO);
    map.unit = (unitChoice == IDYES) ? "Kilometres" : "Miles";
    map.imagePath = ""; // Not implemented

    // Prompt for grid size
    char gridSizeStr[32] = {0};
    strcpy_s(gridSizeStr, "100");
    if (DialogBoxParamA(nullptr, nullptr, nullptr, nullptr, (LPARAM)gridSizeStr)) {
        map.gridSize = atof(gridSizeStr);
        if (map.gridSize <= 0) map.gridSize = 100.0;
    }

    return true;
}

Map MapDialog::getMap() const {
    return map;
}