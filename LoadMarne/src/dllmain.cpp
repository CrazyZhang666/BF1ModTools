// dllmain.cpp : 定义 DLL 应用程序的入口点。
#include "framework.h"

std::wstring className = L"HwndWrapper[BF1MarneTools;;";
std::string offlineName = "战地风云1";

PVOID fpGetComputerNameA = NULL;

// Hook系统函数 GetComputerNameA
BOOL WINAPI HKGetComputerNameA(LPSTR lpBuffer, LPDWORD lpnSize)
{
	// 一个汉字占3字节，最大5个汉字，长度为16字符
	strcpy(lpBuffer, offlineName.c_str());

	return TRUE;
}

// 根据类名模糊查找，返回是否找到布尔值
bool FindWindowByClassName(const std::wstring& className)
{
	HWND hwnd = nullptr;
	while ((hwnd = FindWindowExW(nullptr, hwnd, nullptr, nullptr)) != nullptr)
	{
		wchar_t buffer[256];
		GetClassNameW(hwnd, buffer, sizeof(buffer) / sizeof(wchar_t));

		// 使用wcsstr函数进行模糊匹配
		if (wcsstr(buffer, className.c_str()) != nullptr)
		{
			return true;
		}
	}

	return false;
}

// 去除首尾空白符
std::string& Trim(std::string& str)
{
	if (str.empty())
		return str;

	str.erase(0, str.find_first_not_of(" "));
	str.erase(str.find_last_not_of(" ") + 1);
	return str;
}

// 开始Hook
void DetourHookOn()
{
	// 恢复之前状态，避免反复拦截
	DetourRestoreAfterWith();
	// 开始劫持（开始事务）
	DetourTransactionBegin();
	// 刷新当前的线程
	DetourUpdateThread(GetCurrentThread());

	/////////////////////

	// 将拦截的函数附加到原函数的地址上,这里可以拦截多个函数

	fpGetComputerNameA = DetourFindFunction("kernel32.dll", "GetComputerNameA");
	DetourAttach(&fpGetComputerNameA, HKGetComputerNameA);

	/////////////////////

	// 提交修改，立即HOOk（结束事务）
	DetourTransactionCommit();
}

// 卸载Hook
void DetourHookOff()
{
	// 开始事务
	DetourTransactionBegin();
	// 更新线程信息
	DetourUpdateThread(GetCurrentThread());

	/////////////////////

	// 将拦截的函数从原函数的地址上解除，这里可以解除多个函数

	DetourDetach(&fpGetComputerNameA, HKGetComputerNameA);

	/////////////////////

	// 结束事务
	DetourTransactionCommit();
}

// 内存补丁
void MemPath(__int64 patch, BYTE value)
{
	DWORD oldProtect;
	BYTE* lpAddress;

	lpAddress = (BYTE*)patch;
	VirtualProtect(lpAddress, 1, PAGE_EXECUTE_READWRITE, &oldProtect);
	*lpAddress = value;
	VirtualProtect(lpAddress, 1, oldProtect, &oldProtect);
}

// 核心方法
void Core()
{
	// 绕过EAAC启动限制
	HMODULE user32Ptr = GetModuleHandle(L"user32.dll");
	if (user32Ptr != nullptr)
		MemPath((DWORD_PTR)GetProcAddress(user32Ptr, "MessageBoxTimeoutA"), 0xC3);

	HMODULE kernelbasePtr = GetModuleHandle(L"kernelbase.dll");
	if (kernelbasePtr != nullptr)
		MemPath((DWORD_PTR)GetProcAddress(kernelbasePtr, "TerminateProcess"), 0xC3);

	//////////////////////////////////////////////////////////////////////////////////

	// 关闭显卡驱动检测
	MemPath(0x1410365CC, 0x90);
	MemPath(0x1410365CD, 0x90);
	MemPath(0x1410365D2, 0x90);
	MemPath(0x1410365D3, 0x90);
	MemPath(0x14031CDA1, 0x84);

	// 解锁完整命令控制台
	MemPath(0x1403396F1, 0x90);
	MemPath(0x1403396F2, 0x90);
	MemPath(0x1403396F3, 0x90);
	MemPath(0x1403396F4, 0x90);
	MemPath(0x1403396F5, 0x90);
	MemPath(0x1403396F6, 0x90);

	//////////////////////////////////////////////////////////////////////////////////

	// 查找战地1模组工具箱窗口
	bool isFindWin = FindWindowByClassName(className);
	if (!isFindWin)
		return;

	// 获取数据文件夹路径    
	PWSTR programDataPath;
	HRESULT hr = SHGetKnownFolderPath(FOLDERID_ProgramData, 0, NULL, &programDataPath);
	if (!SUCCEEDED(hr))
		return;

	// 构建 马恩DLL 文件路径
	std::filesystem::path dllPath = std::filesystem::path(programDataPath) / "BF1MarneTools" / "Marne" / "Marne.dll";
	// 检查文件是否存在
	if (!std::filesystem::exists(dllPath))
		return;

	// 加载 马恩Dll
	LoadLibraryW(dllPath.wstring().c_str());

	//////////////////////////////////////////////////////////////////////////////////

	// 判断是以客户端模式运行，还是服务端模式运行

	// 获取命令行字符串
	LPSTR cmdLine = GetCommandLineA();
	// 将命令行字符串转换为std::string
	std::string cmdLineString(cmdLine);
	// 使用std::string::find查找子字符串
	size_t pos = cmdLineString.find("-mserver");
	// 如果找到了子字符串，find方法会返回它在字符串中的位置（从0开始）  
	// 如果没有找到，它会返回std::string::npos 
	if (pos != std::string::npos)
	{
		// 代表是以服务端模式运行

		Sleep(20);
		// 刷新控制台状态
		std::cout.clear();

		return;
	}

	/////////////////////////////////////////

	// 构建 玩家名称 文件路径
	std::filesystem::path namePath = std::filesystem::path(programDataPath) / "BF1MarneTools" / "Config" / "PlayerName.txt";

	// 以二进制模式打开文件以确保正确处理所有字符
	std::ifstream fileRead(namePath, std::ios::in | std::ios::binary);
	// 检查文件是否成功打开
	if (fileRead.is_open())
	{
		std::string content;
		// 读取第一行文本 
		std::getline(fileRead, content);
		// 去除首尾空白符
		Trim(content);
		// 关闭文件  
		fileRead.close();

		// 判断是否为空字符串
		auto isValidName = !content.empty();
		// 判断玩家自定义用户名是否有效
		if (isValidName)
		{
			// 复制为玩家名称
			offlineName = content;
		}
	}

	Sleep(20);
	// 开始Hook
	DetourHookOn();
}

// Dll主线程
DWORD WINAPI MainThread(LPVOID lpThreadParameter)
{
	// 核心方法
	Core();

	return TRUE;
}

// Dll加载入口
BOOL APIENTRY DllMain(HMODULE hModule, DWORD ul_reason_for_call, LPVOID lpReserved)
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
