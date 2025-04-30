#pragma once

// 是否按下键
#define IsKeyPress(vKey) (GetAsyncKeyState(vKey) & 0x8000)
// 线程休眠时间
#define ThreadSleep(ms) (std::this_thread::sleep_for(std::chrono::milliseconds(ms)))

class Global
{
public:
	bool isUnloadAll = false;
	bool isShowMenu = true;

	float points = 18.0f;		// 组成3D圆需要的点数量
	float radius = 0.8f;		// 3D圆的半径
	float line_length = 1.6f;	// 半径的2倍

	HMODULE hModule = NULL;
};

extern Global* global;