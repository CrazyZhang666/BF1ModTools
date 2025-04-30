#include "../framework.h"

Core* core = new Core;

// 添加当前坐标
void Core::Add()
{
	ClientPlayer* pClientPlayer = GetLocalPlayer();
	if (!IsValidPtr(pClientPlayer))
		return;

	ClientVehicleEntity* pClientVehicleEntity = pClientPlayer->clientVehicleEntity;
	ClientSoldierEntity* pClientSoldierEntity = pClientPlayer->clientSoldierEntity;

	if (IsValidPtr(pClientVehicleEntity))
	{
		// 载具
		Matrix4x4 transform;
		pClientVehicleEntity->GetTransform(&transform);

		if (pClientPlayer->teamId == 1)
		{
			// 添加到队伍1
			core->spawnPos.Team1PosList.push_back(transform);
			core->seleTeam1Index = core->spawnPos.Team1PosList.size() - 1;

			Beep(700, 75);
		}
		else if (pClientPlayer->teamId == 2)
		{
			// 添加到队伍2
			core->spawnPos.Team2PosList.push_back(transform);
			core->seleTeam2Index = core->spawnPos.Team2PosList.size() - 1;

			Beep(700, 75);
		}
	}
	else if (IsValidPtr(pClientSoldierEntity))
	{
		// 士兵
		Matrix4x4 transform = pClientSoldierEntity->transform;

		if (pClientPlayer->teamId == 1)
		{
			// 添加到队伍1
			core->spawnPos.Team1PosList.push_back(transform);
			core->seleTeam1Index = core->spawnPos.Team1PosList.size() - 1;

			Beep(700, 75);
		}
		else if (pClientPlayer->teamId == 2)
		{
			// 添加到队伍2
			core->spawnPos.Team2PosList.push_back(transform);
			core->seleTeam2Index = core->spawnPos.Team2PosList.size() - 1;

			Beep(700, 75);
		}
	}
}

// 删除当前选中坐标
void Core::Delect()
{
	ClientPlayer* pClientPlayer = GetLocalPlayer();
	if (!IsValidPtr(pClientPlayer))
		return;

	if (pClientPlayer->teamId == 1)
	{
		// 如果选择索引超出范围，则退出
		if (core->seleTeam1Index < 0 || core->seleTeam1Index >= core->spawnPos.Team1PosList.size())
			return;

		// 按照索引删除元素
		core->spawnPos.Team1PosList.erase(core->spawnPos.Team1PosList.begin() + core->seleTeam1Index);

		// 如果选择是最后一个元素，则后退一步
		if (core->seleTeam1Index == core->spawnPos.Team1PosList.size())
			core->seleTeam1Index--;

		Beep(600, 75);
	}
	else if (pClientPlayer->teamId == 2)
	{
		if (core->seleTeam2Index < 0 || core->seleTeam2Index >= core->spawnPos.Team2PosList.size())
			return;

		core->spawnPos.Team2PosList.erase(core->spawnPos.Team2PosList.begin() + core->seleTeam2Index);

		if (core->seleTeam2Index == core->spawnPos.Team2PosList.size())
			core->seleTeam2Index--;

		Beep(600, 75);
	}
}

// 删除距离我最近的坐标
void Core::Delect2()
{
	ClientPlayer* pClientPlayer = GetLocalPlayer();
	if (!IsValidPtr(pClientPlayer))
		return;

	ClientSoldierEntity* pClientSoldierEntity = pClientPlayer->clientSoldierEntity;
	if (!IsValidPtr(pClientSoldierEntity))
		return;

	Matrix4x4 transform = pClientSoldierEntity->transform;

	if (pClientPlayer->teamId == 1)
	{
		for (int i = 0; i < core->spawnPos.Team1PosList.size(); i++)
		{
			Matrix4x4 item = core->spawnPos.Team1PosList[i];

			float distance = sqrt(
				pow(transform.M41 - item.M41, 2.0f) +
				pow(transform.M42 - item.M42, 2.0f) +
				pow(transform.M43 - item.M43, 2.0f)
			);

			if (distance < global->radius)
			{
				core->spawnPos.Team1PosList.erase(core->spawnPos.Team1PosList.begin() + i);

				Beep(500, 75);
				break;
			}
		}
	}
	else if (pClientPlayer->teamId == 2)
	{
		for (int i = 0; i < core->spawnPos.Team2PosList.size(); i++)
		{
			Matrix4x4 item = core->spawnPos.Team2PosList[i];

			float distance = sqrt(
				pow(transform.M41 - item.M41, 2.0f) +
				pow(transform.M42 - item.M42, 2.0f) +
				pow(transform.M43 - item.M43, 2.0f)
			);

			if (distance < global->radius)
			{
				core->spawnPos.Team2PosList.erase(core->spawnPos.Team2PosList.begin() + i);

				Beep(500, 75);
				break;
			}
		}
	}
}

void Core::Read()
{
	try
	{
		ClientGameContext* pClientGameContext = ClientGameContext::GetInstance();
		if (!IsValidPtr(pClientGameContext))
			return;

		Level* pLevel = pClientGameContext->level;
		if (!IsValidPtr(pLevel))
			return;

		GameMode* pGameMode = pLevel->gameMode;
		if (!IsValidPtr(pGameMode))
			return;

		////////////////////////

		std::string levelName = pLevel->name;
		std::string gameMode = pGameMode->name;

		std::smatch matches;
		std::regex pattern("MP_[^/]*");
		if (!std::regex_search(levelName, matches, pattern))
			return;

		std::string jsonPath = "ImGui/" + matches[0].str() + ".json";
		std::cout << "读取的文件路径为: " << jsonPath << std::endl;

		std::ifstream fileRead(jsonPath);
		if (!fileRead.good())
			return;

		json jsonData;
		fileRead >> jsonData;
		fileRead.close();

		core->spawnPos.from_json(jsonData);

		////////////////////////

		Beep(700, 75);
	}
	catch (const std::exception& e)
	{
		std::cout << "读取Json文件发生异常: " << e.what() << std::endl;
	}
}

void Core::Save()
{
	try
	{
		ClientGameContext* pClientGameContext = ClientGameContext::GetInstance();
		if (!IsValidPtr(pClientGameContext))
			return;

		Level* pLevel = pClientGameContext->level;
		if (!IsValidPtr(pLevel))
			return;

		GameMode* pGameMode = pLevel->gameMode;
		if (!IsValidPtr(pGameMode))
			return;

		////////////////////////

		std::string levelName = pLevel->name;
		std::string gameMode = pGameMode->name;

		std::smatch matches;
		std::regex pattern("MP_[^/]*");
		if (!std::regex_search(levelName, matches, pattern))
			return;

		std::string jsonPath = "ImGui/" + matches[0].str() + ".json";
		std::cout << "保存的文件路径为: " << jsonPath << std::endl;

		core->spawnPos.MapName = levelName;
		core->spawnPos.GameMode = gameMode;

		json jsonData = spawnPos.to_json();

		std::ofstream fileWrite(jsonPath);
		if (!fileWrite.is_open())
			return;

		fileWrite << jsonData.dump(2);
		fileWrite.close();

		////////////////////////

		Beep(700, 75);
	}
	catch (const std::exception& e)
	{
		std::cout << "保存Json文件发生异常: " << e.what() << std::endl;
	}
}

void Core::Clear()
{
	core->spawnPos.MapName = "";
	core->spawnPos.GameMode = "";

	core->spawnPos.Team1PosList.clear();
	core->spawnPos.Team2PosList.clear();

	Beep(700, 75);
}