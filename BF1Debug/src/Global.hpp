#pragma once

// 是否按下键
#define IsKeyPress(vKey) (GetAsyncKeyState(vKey) & 0x8000)
// 线程休眠时间
#define ThreadSleep(ms) (std::this_thread::sleep_for(std::chrono::milliseconds(ms)))

class Global
{
public:
	bool isUnloadAll = false;

	HMODULE hModule = NULL;
};

extern Global* global;