// File: src/ui/MainWindow.cpp
// This is the main window. If you close it, everything disappears. Like my motivation.

#include "MainWindow.h"
#include "ProjectileDialog.h"
#include "MapDialog.h"
#include "logic/ArtilleryCalculator.h"
#include <windows.h>
#include <string>
#include <algorithm>

// Window procedure forward declaration
LRESULT CALLBACK MainWndProc(HWND hwnd, UINT msg, WPARAM wParam, LPARAM lParam);

static MainWindow* g_mainWindow = nullptr;

MainWindow::MainWindow()
    : hwnd(nullptr), width(1024), height(768),
      mapZoom(1.0), mapOffsetX(0), mapOffsetY(0),
      weaponSet(false), targetSet(false), dragging(false),
      selectedProjectile(0), selectedMap(0)
{
    g_mainWindow = this;
    // Add a default projectile and map for demonstration
    Projectile p; p.name = "3BK-10"; p.velocity = 900; p.imagePath = "";
    projectiles.push_back(p);
    Map m; m.name = "Muddy Fields"; m.sizeX = 2000; m.sizeY = 2000; m.imagePath = "";
    maps.push_back(m);
}

void MainWindow::run() {
    createWindow();
    messageLoop();
}

void MainWindow::createWindow() {
    WNDCLASS wc = { 0 };
    wc.lpfnWndProc = MainWndProc;
    wc.hInstance = GetModuleHandle(nullptr);
    wc.lpszClassName = L"ArtilleryMainWindow";
    RegisterClass(&wc);

    hwnd = CreateWindowW(
        wc.lpszClassName, L"Artillery Master",
        WS_OVERLAPPEDWINDOW, CW_USEDEFAULT, CW_USEDEFAULT,
        width, height, nullptr, nullptr, wc.hInstance, nullptr);

    ShowWindow(hwnd, SW_SHOW);
}

void MainWindow::messageLoop() {
    MSG msg;
    while (GetMessage(&msg, nullptr, 0, 0)) {
        TranslateMessage(&msg);
        DispatchMessage(&msg);
    }
}

void MainWindow::render() {
    PAINTSTRUCT ps;
    HDC hdc = BeginPaint(hwnd, &ps);

    RECT clientRect;
    GetClientRect(hwnd, &clientRect);
    width = clientRect.right;
    height = clientRect.bottom;

    // Layout: center map and sidebar horizontally and vertically
    int mapW = 700, mapH = 600;
    int sidebarW = 400, sidebarH = 340;
    int spacing = 60;
    int totalW = mapW + spacing + sidebarW;
    int totalH = std::max(mapH, sidebarH);

    int centerX = width / 2;
    int centerY = height / 2;

    int mapX = centerX - totalW / 2;
    int mapY = centerY - totalH / 2;
    int sidebarX = mapX + mapW + spacing;
    int sidebarY = centerY - sidebarH / 2;

    // Draw background
    HBRUSH bgBrush = CreateSolidBrush(RGB(0,0,0));
    FillRect(hdc, &clientRect, bgBrush);
    DeleteObject(bgBrush);

    // Draw map area
    RECT mapRect = { mapX, mapY, mapX + mapW, mapY + mapH };
    HBRUSH mapBrush = CreateSolidBrush(RGB(96,96,96));
    FillRect(hdc, &mapRect, mapBrush);
    DeleteObject(mapBrush);
    renderMap(hdc, mapX, mapY, mapW, mapH);

    // Draw sidebar
    RECT sidebarRect = { sidebarX, sidebarY, sidebarX + sidebarW, sidebarY + sidebarH };
    HBRUSH sidebarBrush = CreateSolidBrush(RGB(0,0,0));
    FillRect(hdc, &sidebarRect, sidebarBrush);
    DeleteObject(sidebarBrush);
    renderSidebar(hdc, sidebarX, sidebarY, sidebarW, sidebarH);

    // Draw instructions centered below map
    int instrY = mapY + mapH + 20;
    renderInstructions(hdc, mapX, instrY, mapW);

    EndPaint(hwnd, &ps);
}

void MainWindow::renderSidebar(HDC hdc, int sidebarX, int sidebarY, int sidebarW, int sidebarH) {
    SetBkMode(hdc, TRANSPARENT);
    SetTextColor(hdc, RGB(255,255,255));
    HFONT hFontBold = CreateFontA(18,0,0,0,FW_BOLD,FALSE,FALSE,FALSE,ANSI_CHARSET,OUT_DEFAULT_PRECIS,CLIP_DEFAULT_PRECIS,DEFAULT_QUALITY,DEFAULT_PITCH,"Segoe UI");
    HFONT hFont = CreateFontA(18,0,0,0,FW_NORMAL,FALSE,FALSE,FALSE,ANSI_CHARSET,OUT_DEFAULT_PRECIS,CLIP_DEFAULT_PRECIS,DEFAULT_QUALITY,DEFAULT_PITCH,"Segoe UI");
    HFONT oldFont = (HFONT)SelectObject(hdc, hFontBold);

    int labelX = sidebarX + 20, valueX = sidebarX + 180, y = sidebarY + 10, yStep = 36;

    // Stat labels and values
    const char* statNames[] = { "Elevation", "Azimuth", "Distance", "Time of flight" };
    char statValues[4][128];

    double distance = 0.0;
    double azimuth = 0.0;
    double elevLow = 0.0, elevHigh = 0.0;
    double tofLow = 0.0, tofHigh = 0.0;

    if (weaponSet && targetSet && !projectiles.empty()) {
        double dx = (targetPos.x - weaponPos.x) * mapZoom;
        double dy = (targetPos.y - weaponPos.y) * mapZoom;
        distance = sqrt(dx*dx + dy*dy);
        azimuth = ArtilleryCalculator::calculateAzimuth(weaponPos.x, weaponPos.y, targetPos.x, targetPos.y);
        double velocity = projectiles[selectedProjectile].velocity;
        elevLow = ArtilleryCalculator::calculateElevation(distance, velocity, false);
        elevHigh = ArtilleryCalculator::calculateElevation(distance, velocity, true);
        tofLow = ArtilleryCalculator::calculateTimeOfFlight(distance, velocity, false);
        tofHigh = ArtilleryCalculator::calculateTimeOfFlight(distance, velocity, true);
    }

    snprintf(statValues[0], 128, "%.2f\xB0 or %.2f\xB0", elevLow, elevHigh);
    snprintf(statValues[1], 128, "%.2f\xB0", azimuth);
    snprintf(statValues[2], 128, "%.2f meters / %.2f miles", distance, distance / 1609.34);
    snprintf(statValues[3], 128, "%.2f or %.2f seconds", tofLow, tofHigh);

    for (int i = 0; i < 4; ++i) {
        TextOutA(hdc, labelX, y, statNames[i], (int)strlen(statNames[i]));
        SelectObject(hdc, hFont);
        TextOutA(hdc, valueX, y, statValues[i], (int)strlen(statValues[i]));
        SelectObject(hdc, hFontBold);
        y += yStep;
    }

    // Projectile label and combo
    TextOutA(hdc, labelX, y, "Projectile", 9);
    RECT projRect = { valueX, y-4, valueX+120, y+24 };
    DrawEdge(hdc, &projRect, EDGE_SUNKEN, BF_RECT);
    if (!projectiles.empty()) {
        TextOutA(hdc, valueX+4, y, projectiles[selectedProjectile].name.c_str(), (int)projectiles[selectedProjectile].name.size());
    } else {
        SetTextColor(hdc, RGB(0,255,0));
        TextOutA(hdc, valueX+4, y, "Add More Projectile", 19);
        SetTextColor(hdc, RGB(255,255,255));
    }
    // Add More Projectile clickable area
    RECT addProjRect = { valueX+4, y, valueX+120, y+20 };

    y += yStep;

    // Map label and combo
    TextOutA(hdc, labelX, y, "Map", 3);
    RECT mapRect = { valueX, y-4, valueX+120, y+24 };
    DrawEdge(hdc, &mapRect, EDGE_SUNKEN, BF_RECT);
    if (!maps.empty()) {
        std::string mapDisplay = maps[selectedMap].name;
        if (!maps[selectedMap].unit.empty()) {
            mapDisplay += " (" + maps[selectedMap].unit + ")";
        }
        TextOutA(hdc, valueX+4, y, mapDisplay.c_str(), (int)mapDisplay.size());
    } else {
        SetTextColor(hdc, RGB(0,255,0));
        TextOutA(hdc, valueX+4, y, "Add More Map", 12);
        SetTextColor(hdc, RGB(255,255,255));
    }
    // Add More Map clickable area
    RECT addMapRect = { valueX+4, y, valueX+120, y+20 };

    y += yStep + 10;

    // Firemission save/load
    TextOutA(hdc, labelX, y, "Target Registration Point:", 25);
    RECT saveRect = { valueX, y, valueX+60, y+24 };
    RECT loadRect = { valueX+70, y, valueX+130, y+24 };
    DrawEdge(hdc, &saveRect, EDGE_RAISED, BF_RECT);
    DrawEdge(hdc, &loadRect, EDGE_RAISED, BF_RECT);
    TextOutA(hdc, valueX+10, y+4, "Save", 4);
    TextOutA(hdc, valueX+80, y+4, "Load", 4);

    SelectObject(hdc, oldFont);
    DeleteObject(hFontBold);
    DeleteObject(hFont);
}

void MainWindow::renderMap(HDC hdc, int mapX, int mapY, int mapW, int mapH) {
    // Draw map image if available (not shown here)
    // Draw grid based on current map's gridSize and zoom/pan

    if (maps.empty()) return;
    const Map& currentMap = maps[selectedMap];
    double gridSize = currentMap.gridSize; // in map units (e.g., meters)
    double zoom = mapZoom;
    int offsetX = mapOffsetX;
    int offsetY = mapOffsetY;

    // Calculate how many grid lines to draw based on zoom and pan
    HPEN gridPen = CreatePen(PS_SOLID, 1, RGB(200, 200, 200));
    HPEN oldPen = (HPEN)SelectObject(hdc, gridPen);

    // Convert map units to screen pixels
    double unitsPerPixel = 1.0 / zoom;
    double pixelsPerGrid = gridSize * zoom;

    // Find the first grid line to draw (left/top)
    int startX = (int)(-offsetX / pixelsPerGrid) * (int)pixelsPerGrid;
    int startY = (int)(-offsetY / pixelsPerGrid) * (int)pixelsPerGrid;

    for (int x = startX; x < mapW; x += (int)pixelsPerGrid) {
        int sx = mapX + x + offsetX % (int)pixelsPerGrid;
        MoveToEx(hdc, sx, mapY, nullptr);
        LineTo(hdc, sx, mapY + mapH);
    }
    for (int y = startY; y < mapH; y += (int)pixelsPerGrid) {
        int sy = mapY + y + offsetY % (int)pixelsPerGrid;
        MoveToEx(hdc, mapX, sy, nullptr);
        LineTo(hdc, mapX + mapW, sy);
    }

    SelectObject(hdc, oldPen);
    DeleteObject(gridPen);

    // Draw weapon/target positions if set (convert map coords to screen)
    if (weaponSet) {
        int wx = mapX + (int)(weaponPos.x * zoom) + offsetX;
        int wy = mapY + (int)(weaponPos.y * zoom) + offsetY;
        Ellipse(hdc, wx - 6, wy - 6, wx + 6, wy + 6);
    }
    if (targetSet) {
        int tx = mapX + (int)(targetPos.x * zoom) + offsetX;
        int ty = mapY + (int)(targetPos.y * zoom) + offsetY;
        Ellipse(hdc, tx - 6, ty - 6, tx + 6, ty + 6);
    }

    // Draw line between weapon and target
    if (weaponSet && targetSet) {
        HPEN linePen = CreatePen(PS_SOLID, 2, RGB(255,255,255));
        oldPen = (HPEN)SelectObject(hdc, linePen);
        int wx = mapX + (int)(weaponPos.x * zoom) + offsetX;
        int wy = mapY + (int)(weaponPos.y * zoom) + offsetY;
        int tx = mapX + (int)(targetPos.x * zoom) + offsetX;
        int ty = mapY + (int)(targetPos.y * zoom) + offsetY;
        MoveToEx(hdc, wx, wy, nullptr);
        LineTo(hdc, tx, ty);
        SelectObject(hdc, oldPen);
        DeleteObject(linePen);
    }
}

void MainWindow::renderInstructions(HDC hdc, int x, int y, int w) {
    SetBkMode(hdc, TRANSPARENT);
    SetTextColor(hdc, RGB(200,200,200));
    const char* instr = "LMB to set your position. RMB to set the target's position. Hold MMB to move the map around, and scroll wheel to zoom.";
    SIZE sz;
    GetTextExtentPoint32A(hdc, instr, (int)strlen(instr), &sz);
    int instrX = x + (w - sz.cx) / 2;
    TextOutA(hdc, instrX, y, instr, (int)strlen(instr));

	// Remind me to remove this later
    SetTextColor(hdc, RGB(0,191,255));
    const char* link = "download.artillery-calculator.com.";
    GetTextExtentPoint32A(hdc, link, (int)strlen(link), &sz);
    int linkX = x + (w - sz.cx) / 2;
    TextOutA(hdc, linkX, y + 24, link, (int)strlen(link));
    SetTextColor(hdc, RGB(255,255,255));
}

void MainWindow::handleInput(UINT msg, WPARAM wParam, LPARAM lParam) {
    RECT clientRect;
    GetClientRect(hwnd, &clientRect);
    int mapW = 700, mapH = 600;
    int sidebarW = 400, spacing = 60;
    int totalW = mapW + spacing + sidebarW;
    int totalH = std::max(mapH, 340);
    int mapX = clientRect.right / 2 - totalW / 2;
    int mapY = clientRect.bottom / 2 - totalH / 2;

    int mx = LOWORD(lParam), my = HIWORD(lParam);

    // Adjust for pan/zoom
    double zoom = mapZoom;
    int offsetX = mapOffsetX;
    int offsetY = mapOffsetY;
    int relX = (int)((mx - mapX - offsetX) / zoom);
    int relY = (int)((my - mapY - offsetY) / zoom);

    switch (msg) {
    case WM_LBUTTONDOWN:
        if (mx >= mapX && mx < mapX + mapW && my >= mapY && my < mapY + mapH) {
            weaponPos.x = relX;
            weaponPos.y = relY;
            weaponSet = true;
            InvalidateRect(hwnd, nullptr, FALSE);
        }
        // Check if click is on projectile combo or "Add More Projectile"
        // For simplicity, always open dialog if clicked in that area
        else if (mx > mapX + mapW + spacing + 180 && mx < mapX + mapW + spacing + 300 &&
                 my > mapY + 10 + 4*36 - 4 && my < mapY + 10 + 4*36 + 20) {
            openProjectileDialog();
            InvalidateRect(hwnd, nullptr, FALSE);
        }
        // Map combo/add more map
        else if (mx > mapX + mapW + spacing + 180 && mx < mapX + mapW + spacing + 300 &&
                 my > mapY + 10 + 5*36 - 4 && my < mapY + 10 + 5*36 + 20) {
            openMapDialog();
            InvalidateRect(hwnd, nullptr, FALSE);
        }
        // Firemission save/load
        else if (mx > mapX + mapW + spacing + 180 && mx < mapX + mapW + spacing + 240 &&
                 my > mapY + 10 + 6*36 + 10 && my < mapY + 10 + 6*36 + 34) {
            saveFiremission();
        }
        else if (mx > mapX + mapW + spacing + 250 && mx < mapX + mapW + spacing + 310 &&
                 my > mapY + 10 + 6*36 + 10 && my < mapY + 10 + 6*36 + 34) {
            loadFiremission();
        }
        break;
    case WM_RBUTTONDOWN:
        if (mx >= mapX && mx < mapX + mapW && my >= mapY && my < mapY + mapH) {
            targetPos.x = relX;
            targetPos.y = relY;
            targetSet = true;
            InvalidateRect(hwnd, nullptr, FALSE);
        }
        break;
    case WM_MOUSEWHEEL:
        {
            int delta = GET_WHEEL_DELTA_WPARAM(wParam);
            double oldZoom = mapZoom;
            mapZoom += (delta > 0) ? 0.1 : -0.1;
            mapZoom = std::max(0.1, std::min(5.0, mapZoom));
            // Optional: zoom to mouse position
            InvalidateRect(hwnd, nullptr, FALSE);
        }
        break;
    case WM_MBUTTONDOWN:
        dragging = true;
        dragStart.x = mx;
        dragStart.y = my;
        dragOffset.x = mapOffsetX;
        dragOffset.y = mapOffsetY;
        break;
    case WM_MBUTTONUP:
        dragging = false;
        break;
    case WM_MOUSEMOVE:
        if (dragging && (wParam & MK_MBUTTON)) {
            int dx = mx - dragStart.x;
            int dy = my - dragStart.y;
            mapOffsetX = dragOffset.x + dx;
            mapOffsetY = dragOffset.y + dy;
            InvalidateRect(hwnd, nullptr, FALSE);
        }
        break;
    }
}

// Win32 window procedure
LRESULT CALLBACK MainWndProc(HWND hwnd, UINT msg, WPARAM wParam, LPARAM lParam) {
    if (!g_mainWindow) return DefWindowProc(hwnd, msg, wParam, lParam);

    switch (msg) {
    case WM_PAINT:
        g_mainWindow->render();
        return 0;
    case WM_LBUTTONDOWN:
    case WM_RBUTTONDOWN:
    case WM_MBUTTONDOWN:
    case WM_MBUTTONUP:
    case WM_MOUSEMOVE:
    case WM_MOUSEWHEEL:
        g_mainWindow->handleInput(msg, wParam, lParam);
        return 0;
    case WM_DESTROY:
        PostQuitMessage(0);
        return 0;
    }
    return DefWindowProc(hwnd, msg, wParam, lParam);
}

void MainWindow::openProjectileDialog() {
    ProjectileDialog dialog;
    if (dialog.exec()) {
        projectiles.push_back(dialog.getProjectile());
        selectedProjectile = (int)projectiles.size() - 1;
    }
}

void MainWindow::openMapDialog() {
    MapDialog dialog;
    if (dialog.exec()) {
        maps.push_back(dialog.getMap());
        selectedMap = (int)maps.size() - 1;
    }
}

void MainWindow::saveFiremission() {
    Firemission fm;
    fm.name = "SavedMission";
    fm.weaponX = weaponPos.x;
    fm.weaponY = weaponPos.y;
    fm.targetX = targetPos.x;
    fm.targetY = targetPos.y;
    fm.selectedProjectile = selectedProjectile;
    fm.selectedMap = selectedMap;
    firemissions.push_back(fm);
    MessageBoxA(hwnd, "Firemission saved!", "Info", MB_OK);
}

void MainWindow::loadFiremission() {
    if (!firemissions.empty()) {
        // For demonstration, just load the first firemission
        const Firemission& fm = firemissions.front();
        weaponPos.x = fm.weaponX;
        weaponPos.y = fm.weaponY;
        targetPos.x = fm.targetX;
        targetPos.y = fm.targetY;
        selectedProjectile = fm.selectedProjectile;
        selectedMap = fm.selectedMap;
        weaponSet = true;
        targetSet = true;
        InvalidateRect(hwnd, nullptr, FALSE);
        MessageBoxA(hwnd, "Firemission loaded!", "Info", MB_OK);
    }
}

// Optional: importFiremission for loading from file (stub)
void MainWindow::importFiremission() {
    // Implement file dialog and deserialization as needed
    MessageBoxA(hwnd, "Import not implemented.", "Info", MB_OK);
}

// To-Do List (All Done!)
// - Made the window show up (duh).
// - Connected all the signals. Like a social network, but for widgets.
// - Added comments so future me knows what's going on.