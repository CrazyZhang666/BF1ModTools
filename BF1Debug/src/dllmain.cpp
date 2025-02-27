// dllmain.cpp : 定义 DLL 应用程序的入口点。
#include "framework.h"

// 枚举窗口
BOOL CALLBACK EnumWindowsProc(HWND hwnd, LPARAM lParam)
{
	DWORD dwCurProcessId = *((DWORD*)lParam);
	DWORD dwProcessId = 0;

	GetWindowThreadProcessId(hwnd, &dwProcessId);
	if (dwProcessId == dwCurProcessId && GetParent(hwnd) == NULL)
	{
		*((HWND*)lParam) = hwnd;
		return FALSE;
	}
	return TRUE;
}

// 获取主窗口句柄
HWND GetMainWindowHwnd()
{
	DWORD dwCurrentProcessId = GetCurrentProcessId();
	if (!EnumWindows(EnumWindowsProc, (LPARAM)&dwCurrentProcessId))
	{
		return (HWND)dwCurrentProcessId;
	}
	return NULL;
}

// Dll主线程
DWORD WINAPI MainThread(LPVOID lpThreadParameter)
{
	HMODULE hModule = (HMODULE)lpThreadParameter;

	global->hModule = hModule;
	dxHook->hWindow = GetMainWindowHwnd();

	AllocConsole();
	FILE* pFile = NULL;
	freopen_s(&pFile, "CONOUT$", "w", stdout);

	SetConsoleTitleW(L"BF1 ImGui DX11 Hook v0.1");
	EnableMenuItem(GetSystemMenu(GetConsoleWindow(), false), SC_CLOSE, MF_BYCOMMAND | MF_DISABLED | MF_GRAYED);
	SetConsoleOutputCP(CP_UTF8);

	std::cout.clear();

	LPSTR cmdLine = GetCommandLineA();
	std::cout << "命令行: " << cmdLine << std::endl;
	std::cout << std::endl;

	std::cout << "当前进程句柄: " << global->hModule << std::endl;
	std::cout << "当前窗口句柄: " << dxHook->hWindow << std::endl;
	std::cout << std::endl;

	std::cout << "按 END 键卸载注入" << std::endl;
	std::cout << std::endl;

	//////////////////////////////////////////

	dxHook->InitHook();

	bool isKey1Down = false;
	bool isKey2Down = false;
	bool isKey3Down = false;

	// 当按下 END 键时卸载DLL
	do
	{
		// Num1键 按下事件，防止重复触发
		if (IsKeyPress(VK_F1))
		{
			if (!isKey1Down)
			{
				isKey1Down = true;
				core->Add();
			}
		}
		else
			if (isKey1Down)
				isKey1Down = false;

		// Num2键 按下事件，防止重复触发
		if (IsKeyPress(VK_F2))
		{
			if (!isKey2Down)
			{
				isKey2Down = true;
				core->Delect();
			}
		}
		else
			if (isKey2Down)
				isKey2Down = false;

		// Num3键 按下事件，防止重复触发
		if (IsKeyPress(VK_F3))
		{
			if (!isKey3Down)
			{
				isKey3Down = true;
				core->Delect2();
			}
		}
		else
			if (isKey3Down)
				isKey3Down = false;

		ThreadSleep(20);
	} while (!IsKeyPress(VK_END));

	dxHook->ClearHook();

	//////////////////////////////////////////

	// 释放控制台
	if (pFile)
	{
		std::cout << "[+] 释放控制台成功" << std::endl;

		fclose(pFile);
		pFile = NULL;
		FreeConsole();
	}

	ThreadSleep(20);

	// 释放DLL并且退出线程
	FreeLibraryAndExitThread(hModule, 0);
	return TRUE;
}

// Dll加载入口
BOOL APIENTRY DllMain(HMODULE hModule, DWORD  ul_reason_for_call, LPVOID lpReserved)
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
		DisableThreadLibraryCalls(hModule);
		if (HANDLE handle = CreateThread(NULL, NULL, (LPTHREAD_START_ROUTINE)MainThread, hModule, NULL, NULL))
			CloseHandle(handle);
		break;
	case DLL_THREAD_ATTACH:
		break;
	case DLL_THREAD_DETACH:
		break;
	case DLL_PROCESS_DETACH:
		break;
	}
	return TRUE;
}