#pragma once
#include <vector>
#include "model/Projectile.h"
#include "model/Map.h"
#include "model/Firemission.h"

// Forward declare Win32 types for minimal GDI windowing
struct HWND__;
typedef struct HWND__* HWND;

class MainWindow {
public:
    MainWindow();
    void run();

private:
    void createWindow();
    void messageLoop();
    void render();
    void renderSidebar(HDC hdc, int sidebarX, int sidebarY, int sidebarW, int sidebarH);
    void renderMap(HDC hdc, int mapX, int mapY, int mapW, int mapH);
    void renderInstructions(HDC hdc, int x, int y, int w);
    void handleInput(UINT msg, WPARAM wParam, LPARAM lParam);

    // Win32 window handles
    HWND hwnd;
    int width, height;

    // Map view state
    double mapZoom;
    int mapOffsetX, mapOffsetY;
    POINT weaponPos, targetPos;
    bool weaponSet, targetSet;
    bool dragging;
    POINT dragStart, dragOffset;

    // Data
    std::vector<Projectile> projectiles;
    std::vector<Map> maps;
    std::vector<Firemission> firemissions;

    // UI state
    int selectedProjectile;
    int selectedMap;

    // Helper
    void openProjectileDialog();
    void openMapDialog();
    void saveFiremission();
    void loadFiremission();
    void importFiremission(); // Added this for import
};