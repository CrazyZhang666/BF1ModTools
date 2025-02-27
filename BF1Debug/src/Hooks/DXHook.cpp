#include "../framework.h"

DXHook* dxHook = new DXHook;

ID3D11Device* pd3dDevice = nullptr;
ID3D11DeviceContext* pd3dDeviceContext = nullptr;
IDXGISwapChain* pSwapChain = nullptr;
ID3D11RenderTargetView* pRenderTargetView = nullptr;

LPVOID pPresent = nullptr;
LPVOID pResizeBuffers = nullptr;

Present pOrgPresent = nullptr;
ResizeBuffers pOrgResizeBuffers = nullptr;

WNDPROC pWndProc = nullptr;

bool initialized = false;

// 初始化 ImGui
void InitImGui()
{
	std::cout << "[+] 正在初始化 ImGui ..." << std::endl;

	// 创建 ImGui 上下文
	ImGui::CreateContext();

	// 设置 ImGui 样式
	ImGui::StyleColorsDark();

	// 设置 平台/渲染器 后端
	ImGui_ImplWin32_Init(dxHook->hWindow);
	ImGui_ImplDX11_Init(pd3dDevice, pd3dDeviceContext);
	ImGui_ImplDX11_CreateDeviceObjects();

	// 设置字体等...
	auto& io = ImGui::GetIO();
	io.Fonts->AddFontDefault();

	// 创建文件夹
	CreateDirectory(L".\\ImGui\\", NULL);
	// 自定义ImGui配置文件路径
	io.IniFilename = ".\\ImGui\\imgui.ini";
	io.LogFilename = ".\\ImGui\\imgui_log.txt";
}

// 窗口回调函数实现
LRESULT CALLBACK HKWndProc(const HWND hWnd, UINT uMsg, WPARAM wParam, LPARAM lParam)
{
	// 确保只调用一次
	static auto once = []()
		{
			std::cout << __FUNCTION__ << " > 第一次调用" << std::endl;
			return true;
		}();

	// 把窗口消息传入到 ImGui
	if (ImGui_ImplWin32_WndProcHandler(hWnd, uMsg, wParam, lParam))
		return TRUE;

	return CallWindowProc(pWndProc, hWnd, uMsg, wParam, lParam);
}

// Present Hook 函数实现
HRESULT __fastcall HKPresent(IDXGISwapChain* pSwapChain, UINT syncInterval, UINT flags)
{
	// 检查是否已初始化
	if (!initialized)
	{
		// 尝试获取交换链设备
		if (SUCCEEDED(pSwapChain->GetDevice(__uuidof(ID3D11Device), (void**)&pd3dDevice)))
		{
			// 获取设备上下文
			pd3dDevice->GetImmediateContext(&pd3dDeviceContext);

			// 创建纹理后置缓冲
			ID3D11Texture2D* pBackBuffer;
			// 获取后置缓冲交换链
			pSwapChain->GetBuffer(0, __uuidof(ID3D11Texture2D), (LPVOID*)&pBackBuffer);
			// 初始化渲染目标视图
			pd3dDevice->CreateRenderTargetView(pBackBuffer, NULL, &pRenderTargetView);
			// 释放后置缓冲
			pBackBuffer->Release();

			// 更新已初始化变量值
			initialized = true;

			std::cout << __FUNCTION__ << " > 成功接收到渲染视图" << std::endl;
		}
		else
		{
			// 调用原始 Present 函数
			return pOrgPresent(pSwapChain, syncInterval, flags);
		}
	}

	// 确保只调用一次
	static auto once = []()
		{
			// 初始化 ImGui
			InitImGui();
			// 只调用一次
			pWndProc = (WNDPROC)SetWindowLongPtr(dxHook->hWindow, GWLP_WNDPROC, (LONG_PTR)HKWndProc);

			std::cout << "[+] pWndProc: " << pWndProc << std::endl;
			std::cout << __FUNCTION__ << " > 第一次调用" << std::endl;

			return true;
		}();

	ImGui_ImplDX11_NewFrame();
	ImGui_ImplWin32_NewFrame();
	ImGui::NewFrame();

	/////////////////////////////

	features->Run();

	/////////////////////////////

	ImGui::Render();

	pd3dDeviceContext->OMSetRenderTargets(1, &pRenderTargetView, nullptr);
	ImGui_ImplDX11_RenderDrawData(ImGui::GetDrawData());

	// 调用原始 Present 函数
	return pOrgPresent(pSwapChain, syncInterval, flags);
}

// ResizeBuffers Hook 函数实现
HRESULT __fastcall HKResizeBuffers(IDXGISwapChain* pChain, UINT BufferCount, UINT Width, UINT Height, DXGI_FORMAT NewFormat, UINT Flags)
{
	// 确保只调用一次
	static auto once = []()
		{
			std::cout << __FUNCTION__ << " > 第一次调用" << std::endl;
			return true;
		}();

	pRenderTargetView->Release();
	pRenderTargetView = nullptr;

	// 更新已初始化变量值
	initialized = false;

	ImGui_ImplDX11_CreateDeviceObjects();
	ImGui_ImplDX11_InvalidateDeviceObjects();

	return pOrgResizeBuffers(pChain, BufferCount, Width, Height, NewFormat, Flags);
}

// 初始化 Hook
void DXHook::InitHook()
{
	// 初始化 MinHook  
	if (MH_Initialize() != MH_OK)
	{
		std::cout << "[x] 初始化 MinHook 失败" << std::endl;
		return;
	}
	std::cout << "[+] 初始化 MinHook 成功" << std::endl;

	DXGI_SWAP_CHAIN_DESC sd;
	ZeroMemory(&sd, sizeof(sd));

	sd.BufferDesc.Width = 0;
	sd.BufferDesc.Height = 0;
	sd.BufferDesc.RefreshRate.Numerator = 60;
	sd.BufferDesc.RefreshRate.Denominator = 1;
	sd.BufferDesc.Format = DXGI_FORMAT_R8G8B8A8_UNORM;
	sd.BufferDesc.ScanlineOrdering = DXGI_MODE_SCANLINE_ORDER_UNSPECIFIED;
	sd.BufferDesc.Scaling = DXGI_MODE_SCALING_UNSPECIFIED;

	sd.SampleDesc.Count = 1;
	sd.SampleDesc.Quality = 0;

	sd.BufferUsage = DXGI_USAGE_RENDER_TARGET_OUTPUT;
	sd.BufferCount = 1;
	sd.OutputWindow = hWindow;
	sd.Windowed = TRUE;
	sd.SwapEffect = DXGI_SWAP_EFFECT_DISCARD;
	sd.Flags = DXGI_SWAP_CHAIN_FLAG_ALLOW_MODE_SWITCH;

	UINT createDeviceFlags = 0;
	D3D_FEATURE_LEVEL featureLevel;
	const D3D_FEATURE_LEVEL featureLevelArray[2] = { D3D_FEATURE_LEVEL_11_0, D3D_FEATURE_LEVEL_10_0 };

	HRESULT res = D3D11CreateDeviceAndSwapChain(nullptr, D3D_DRIVER_TYPE_HARDWARE, nullptr, createDeviceFlags, featureLevelArray, ARRAYSIZE(featureLevelArray), D3D11_SDK_VERSION, &sd, &pSwapChain, &pd3dDevice, &featureLevel, &pd3dDeviceContext);

	if (FAILED(res))
	{
		std::cout << "[x] 创建 D3D 设备和交换链失败: " << res << std::endl;
		return;
	}

	std::cout << "[+] 创建 D3D 设备和交换链成功: " << res << std::endl;

	std::cout << "[+] pd3dDevice: " << pd3dDevice << std::endl;
	std::cout << "[+] pd3dDeviceContext: " << pd3dDeviceContext << std::endl;
	std::cout << "[+] pSwapChain: " << pSwapChain << std::endl;

	void** pVTableSwapChain = *reinterpret_cast<void***>(pSwapChain);

	pPresent = reinterpret_cast<LPVOID>(pVTableSwapChain[IDXGISwapChainvTable::PRESENT]);
	pResizeBuffers = reinterpret_cast<LPVOID>(pVTableSwapChain[IDXGISwapChainvTable::RESIZE_BUFFERS]);

	// 输出这两个地址
	std::cout << "[+] pPresent: " << pPresent << std::endl;
	std::cout << "[+] pResizeBuffers: " << pResizeBuffers << std::endl;

	// 创建 Present Hook
	if (MH_CreateHook(pPresent, &HKPresent, (LPVOID*)&pOrgPresent) != MH_OK)
	{
		std::cout << "[x] 创建 Present Hook 失败" << std::endl;
		return;
	}
	std::cout << "[+] 创建 Present Hook 成功" << std::endl;
	// 启用 Present Hook
	if (MH_EnableHook(pPresent) != MH_OK)
	{
		std::cout << "[x] 启用 Present Hook 失败" << std::endl;
		return;
	}
	std::cout << "[+] 启用 Present Hook 成功" << std::endl;

	// 创建 ResizeBuffers Hook
	if (MH_CreateHook(pResizeBuffers, &HKResizeBuffers, (LPVOID*)&pOrgResizeBuffers) != MH_OK)
	{
		std::cout << "[x] 创建 ResizeBuffers Hook 失败" << std::endl;
		return;
	}
	std::cout << "[+] 创建 ResizeBuffers Hook 成功" << std::endl;
	// 启用 ResizeBuffers Hook
	if (MH_EnableHook(pResizeBuffers) != MH_OK)
	{
		std::cout << "[x] 启用 ResizeBuffers Hook 失败" << std::endl;
		return;
	}
	std::cout << "[+] 启用 ResizeBuffers Hook 成功" << std::endl;
}

// 清理 Hook
void DXHook::ClearHook()
{
	global->isUnloadAll = true;
	ThreadSleep(20);

	// 禁用 Present Hook
	if (MH_DisableHook(pPresent) == MH_OK)
		std::cout << "[+] 禁用 Present Hook 成功" << std::endl;
	else
		std::cout << "[x] 禁用 Present Hook 失败" << std::endl;
	// 移除 Present Hook
	if (MH_RemoveHook(pPresent) == MH_OK)
		std::cout << "[+] 移除 Present Hook 成功" << std::endl;
	else
		std::cout << "[x] 移除 Present Hook 失败" << std::endl;
	ThreadSleep(20);

	// 禁用 ResizeBuffers Hook
	if (MH_DisableHook(pResizeBuffers) == MH_OK)
		std::cout << "[+] 禁用 ResizeBuffers Hook 成功" << std::endl;
	else
		std::cout << "[x] 禁用 ResizeBuffers Hook 失败" << std::endl;
	// 移除 ResizeBuffers Hook
	if (MH_RemoveHook(pResizeBuffers) == MH_OK)
		std::cout << "[+] 移除 ResizeBuffers Hook 成功" << std::endl;
	else
		std::cout << "[x] 移除 ResizeBuffers Hook 失败" << std::endl;
	ThreadSleep(20);

	// 卸载 MinHook
	if (MH_Uninitialize() == MH_OK)
		std::cout << "[+] 卸载 MinHook 成功" << std::endl;
	else
		std::cout << "[x] 卸载 MinHook 失败" << std::endl;
	ThreadSleep(20);

	ImGui_ImplDX11_Shutdown();
	ImGui_ImplWin32_Shutdown();
	ImGui::DestroyContext();
	ThreadSleep(20);

	if (pRenderTargetView)
	{
		pRenderTargetView->Release();
		pRenderTargetView = nullptr;

		std::cout << "[+] 卸载 mainRenderTargetView 成功" << std::endl;
		ThreadSleep(20);
	}

	if (pSwapChain)
	{
		pSwapChain->Release();
		pSwapChain = nullptr;

		std::cout << "[+] 卸载 pSwapChain 成功" << std::endl;
		ThreadSleep(20);
	}

	if (pd3dDeviceContext)
	{
		pd3dDeviceContext->Release();
		pd3dDeviceContext = nullptr;

		std::cout << "[+] 卸载 pd3dDeviceContext 成功" << std::endl;
		ThreadSleep(20);
	}

	if (pd3dDevice)
	{
		pd3dDevice->Release();
		pd3dDevice = nullptr;

		std::cout << "[+] 卸载 pd3dDevice 成功" << std::endl;
		ThreadSleep(20);
	}

	SetWindowLongPtr(hWindow, GWLP_WNDPROC, (LONG_PTR)pWndProc);
	ThreadSleep(20);
}
