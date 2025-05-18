#pragma once

#pragma execution_character_set("utf-8")

#pragma warning(disable:4996)

#define WIN32_LEAN_AND_MEAN

#include <windows.h>
#include <string>
#include <filesystem>
#include <ShlObj_core.h>
#include <fstream>
#include <iostream>

#include "detours.h"

#pragma comment(lib, "detours.lib")

#pragma comment(linker, "/export:DirectInput8Create=C:\\WINDOWS\\System32\\dinput8.DirectInput8Create,@1")
#pragma comment(linker, "/export:DllCanUnloadNow=C:\\WINDOWS\\System32\\dinput8.DllCanUnloadNow,@2")
#pragma comment(linker, "/export:DllGetClassObject=C:\\WINDOWS\\System32\\dinput8.DllGetClassObject,@3")
#pragma comment(linker, "/export:DllRegisterServer=C:\\WINDOWS\\System32\\dinput8.DllRegisterServer,@4")
#pragma comment(linker, "/export:DllUnregisterServer=C:\\WINDOWS\\System32\\dinput8.DllUnregisterServer,@5")
#pragma comment(linker, "/export:GetdfDIJoystick=C:\\WINDOWS\\System32\\dinput8.GetdfDIJoystick,@6")

typedef BOOL(WINAPI* OldGetComputerNameA)(LPSTR lpBuffer, LPDWORD lpnSize);