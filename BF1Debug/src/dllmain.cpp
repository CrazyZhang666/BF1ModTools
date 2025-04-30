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

	SetConsoleTitleW(L"BF1 Dev ImGui DX11 Hook v0.1");
	EnableMenuItem(GetSystemMenu(GetConsoleWindow(), false), SC_CLOSE, MF_BYCOMMAND | MF_DISABLED | MF_GRAYED);
	SetConsoleOutputCP(CP_UTF8);

	std::cout.clear();

	LPSTR cmdLine = GetCommandLineA();
	std::cout << "命令行: " << cmdLine << std::endl;
	std::cout << std::endl;

	std::cout << "当前进程句柄: " << global->hModule << std::endl;
	std::cout << "当前窗口句柄: " << dxHook->hWindow << std::endl;
	std::cout << std::endl;

	std::cout << "按 INS 键显示隐藏" << std::endl;
	std::cout << "按 END 键卸载注入" << std::endl;
	std::cout << std::endl;

	//////////////////////////////////////////

	dxHook->InitHook();

	bool isKeyInsDown = false;

	bool isKeyF1Down = false;
	bool isKeyF2Down = false;
	bool isKeyF3Down = false;

	// 当按下 END 键时卸载DLL
	do
	{
		// INS键 按下事件，防止重复触发
		if (IsKeyPress(VK_INSERT))
		{
			if (!isKeyInsDown)
			{
				isKeyInsDown = true;
				global->isShowMenu = !global->isShowMenu;
			}
		}
		else if (isKeyInsDown)
			isKeyInsDown = false;

		///////////////////////////

		// F1键 按下事件，防止重复触发
		if (IsKeyPress(VK_F1))
		{
			if (!isKeyF1Down)
			{
				isKeyF1Down = true;
				core->Add();
			}
		}
		else if (isKeyF1Down)
			isKeyF1Down = false;

		// F2键 按下事件，防止重复触发
		if (IsKeyPress(VK_F2))
		{
			if (!isKeyF2Down)
			{
				isKeyF2Down = true;
				core->Delect();
			}
		}
		else if (isKeyF2Down)
			isKeyF2Down = false;

		// F3键 按下事件，防止重复触发
		if (IsKeyPress(VK_F3))
		{
			if (!isKeyF3Down)
			{
				isKeyF3Down = true;
				core->Delect2();
			}
		}
		else if (isKeyF3Down)
			isKeyF3Down = false;

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