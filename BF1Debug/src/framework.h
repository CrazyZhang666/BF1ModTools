#pragma once

#pragma warning (disable: 4018)
#pragma warning (disable: 4244)
#pragma warning (disable: 4267)
#pragma warning (disable: 4312)
#pragma warning (disable: 6011)

#pragma execution_character_set("utf-8")  

#define WIN32_LEAN_AND_MEAN
#define _USE_MATH_DEFINES

#include <windows.h>

#include <iostream>
#include <thread>
#include <filesystem>
#include <cmath>
#include <string>
#include <vector>
#include <regex>
#include <fstream>

#include "Global.hpp"

#include <d3d11.h>
#pragma comment(lib, "d3d11.lib")

#include "MinHook/include/MinHook.h"

#include "ImGui/imgui.h"
#include "ImGui/imgui_internal.h"
#include "ImGui/imgui_impl_dx11.h"
#include "ImGui/imgui_impl_win32.h"

#include "Json/json.hpp"

#include "Math/Vector2.hpp"
#include "Math/Vector3.hpp"
#include "Math/Matrix4x4.hpp"

#include "Hooks/DXHook.hpp"

#include "SDK/sdk.hpp"
#include "SDK/FrostBite.hpp"
#include "Draw/Draw.hpp"
#include "Core/Core.hpp"
#include "Features/Features.hpp"