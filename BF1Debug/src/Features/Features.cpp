#include "../framework.h"

Features* features = new Features;

ImColor yellow_color = ImColor(255, 255, 0, 255);
ImColor blue_color = ImColor(51, 153, 255, 255);
ImColor white_color = ImColor(255, 255, 255, 255);
ImColor white_color_50 = ImColor(110, 110, 128, 128);

float room_scale = 1.0f;

bool is_use_vehicle_mode = false;

void ShowLoaction()
{
	ClientPlayer* pClientPlayer = GetLocalPlayer();
	if (!IsValidPtr(pClientPlayer))
		return;

	// 压栈颜色样式，临时修改为当前颜色设置 
	ImGui::PushStyleColor(ImGuiCol_WindowBg, ImVec4());
	// 压栈变量样式，临时修改为当前变量设置 
	ImGui::PushStyleVar(ImGuiStyleVar_WindowBorderSize, 0);

	ImGuiWindowFlags window_flags =
		ImGuiWindowFlags_NoTitleBar |
		ImGuiWindowFlags_NoSavedSettings |
		ImGuiWindowFlags_NoInputs;

	ImGui::Begin("Show Loaction", NULL, window_flags);

	ImGuiIO& io = ImGui::GetIO();

	ImGui::SetWindowPos(ImVec2(), ImGuiCond_Always);
	ImGui::SetWindowSize(ImVec2(io.DisplaySize.x, io.DisplaySize.y), ImGuiCond_Always);

	ClientVehicleEntity* pClientVehicleEntity = pClientPlayer->clientVehicleEntity;
	ClientSoldierEntity* pClientSoldierEntity = pClientPlayer->clientSoldierEntity;

	if (IsValidPtr(pClientVehicleEntity))
	{
		// 载具 在自己脚底画圆
		Matrix4x4 transform;
		pClientVehicleEntity->GetTransform(&transform);

		Vector3 location(
			transform.M41, transform.M42, transform.M43
		);

		draw->AddCircle3D(location, global->points, global->radius, yellow_color);
		draw->AddText2(location, yellow_color, "+");
		draw->AddYawLine(transform, global->line_length, yellow_color);
	}
	else if (IsValidPtr(pClientSoldierEntity))
	{
		// 士兵 在自己脚底画圆
		Matrix4x4 transform = pClientSoldierEntity->transform;
		Vector3 location(
			transform.M41, transform.M42, transform.M43
		);

		draw->AddCircle3D(location, global->points, global->radius, yellow_color);
		draw->AddText2(location, yellow_color, "+");
		draw->AddYawLine(transform, global->line_length, yellow_color);
	}

	if (pClientPlayer->teamId == 1)
	{
		for (int i = 0; i < core->spawnPos.Team1PosList.size(); i++)
		{
			Vector3 location(
				core->spawnPos.Team1PosList[i].M41,
				core->spawnPos.Team1PosList[i].M42,
				core->spawnPos.Team1PosList[i].M43
			);

			if (i < 54)
			{
				draw->AddCircle3D(location, global->points, global->radius, yellow_color);

				auto indexStr = std::to_string(i);
				draw->AddText2(location, white_color, indexStr.c_str());

				draw->AddYawLine(core->spawnPos.Team1PosList[i], global->line_length, yellow_color);
			}
			else
			{
				draw->AddCircle3D(location, global->points, global->radius, white_color);

				auto indexStr = std::to_string(i);
				draw->AddText2(location, blue_color, indexStr.c_str());

				draw->AddYawLine(core->spawnPos.Team1PosList[i], global->line_length, white_color);
			}
		}
	}
	else if (pClientPlayer->teamId == 2)
	{
		for (int i = 0; i < core->spawnPos.Team2PosList.size(); i++)
		{
			Vector3 location(
				core->spawnPos.Team2PosList[i].M41,
				core->spawnPos.Team2PosList[i].M42,
				core->spawnPos.Team2PosList[i].M43
			);

			if (i < 54)
			{
				draw->AddCircle3D(location, global->points, global->radius, yellow_color);

				auto indexStr = std::to_string(i);
				draw->AddText2(location, white_color, indexStr.c_str());

				draw->AddYawLine(core->spawnPos.Team2PosList[i], global->line_length, yellow_color);
			}
			else
			{
				draw->AddCircle3D(location, global->points, global->radius, white_color);

				auto indexStr = std::to_string(i);
				draw->AddText2(location, blue_color, indexStr.c_str());

				draw->AddYawLine(core->spawnPos.Team2PosList[i], global->line_length, white_color);
			}
		}
	}

	/*
	   在DrawList对象上调用PushClipRectFullScreen()方法，其作用是设置一个新的裁剪矩形，该矩形覆盖了整个屏幕
	   （或更准确地说，是ImGui的渲染目标）。裁剪矩形用于限制绘制命令的渲染区域，只有落在裁剪矩形内的部分才会被实际渲染到屏幕上。

	   在这个特定的调用中，PushClipRectFullScreen()通过创建一个覆盖整个屏幕的裁剪矩形，实际上是在告诉绘制系统：“在接下来的绘制命令中，
	   不需要进行额外的裁剪，因为所有的内容都应该被渲染。”然而，这个调用通常不会单独出现，而是与后续的绘制命令和PopClipRect()
	   （用于撤销最近的裁剪矩形设置）结合使用，以实现更复杂的绘制逻辑。
	*/
	ImGui::GetCurrentWindow()->DrawList->PushClipRectFullScreen();

	ImGui::End();

	// 弹出颜色样式，恢复到之前的颜色设置 
	ImGui::PopStyleColor();
	// 弹出变量样式，恢复到之前的变量设置 
	ImGui::PopStyleVar();
}

void ShowSelfInfo()
{
	ClientPlayer* pClientPlayer = GetLocalPlayer();
	if (!IsValidPtr(pClientPlayer))
		return;

	///////////////////////////////////////////

	ImGuiWindowFlags window_flags =
		ImGuiWindowFlags_NoDecoration |
		ImGuiWindowFlags_AlwaysAutoResize |
		ImGuiWindowFlags_NoSavedSettings |
		ImGuiWindowFlags_NoMove;

	const ImGuiViewport* viewport = ImGui::GetMainViewport();

	// 左下角
	ImGui::SetNextWindowPos(ImVec2(10, viewport->WorkSize.y - 10), ImGuiCond_Always, ImVec2(0, 1));
	ImGui::SetNextWindowBgAlpha(0.5f);

	ImGui::Begin("Show Self Info", NULL, window_flags);

	ClientVehicleEntity* pClientVehicleEntity = pClientPlayer->clientVehicleEntity;
	ClientSoldierEntity* pClientSoldierEntity = pClientPlayer->clientSoldierEntity;

	if (IsValidPtr(pClientVehicleEntity))
	{
		// 载具
		Matrix4x4 transform;
		pClientVehicleEntity->GetTransform(&transform);

		ImGui::Text("M11: %f", transform.M11);
		ImGui::Text("M12: %f", transform.M12);
		ImGui::Text("M13: %f", transform.M13);
		ImGui::Text("M14: %f", transform.M14);

		ImGui::Separator();
		ImGui::Text("M21: %f", transform.M21);
		ImGui::Text("M22: %f", transform.M22);
		ImGui::Text("M23: %f", transform.M23);
		ImGui::Text("M24: %f", transform.M24);

		ImGui::Separator();
		ImGui::Text("M31: %f", transform.M31);
		ImGui::Text("M32: %f", transform.M32);
		ImGui::Text("M33: %f", transform.M33);
		ImGui::Text("M34: %f", transform.M34);

		ImGui::Separator();
		ImGui::Text("M41: %f", transform.M41);
		ImGui::Text("M42: %f", transform.M42);
		ImGui::Text("M43: %f", transform.M43);
		ImGui::Text("M44: %f", transform.M44);

		ImGui::Separator();
	}
	else if (IsValidPtr(pClientSoldierEntity))
	{
		// 士兵
		Matrix4x4 transform = pClientSoldierEntity->transform;

		ImGui::Text("M11: %f", transform.M11);
		ImGui::Text("M12: %f", transform.M12);
		ImGui::Text("M13: %f", transform.M13);
		ImGui::Text("M14: %f", transform.M14);

		ImGui::Separator();
		ImGui::Text("M21: %f", transform.M21);
		ImGui::Text("M22: %f", transform.M22);
		ImGui::Text("M23: %f", transform.M23);
		ImGui::Text("M24: %f", transform.M24);

		ImGui::Separator();
		ImGui::Text("M31: %f", transform.M31);
		ImGui::Text("M32: %f", transform.M32);
		ImGui::Text("M33: %f", transform.M33);
		ImGui::Text("M34: %f", transform.M34);

		ImGui::Separator();
		ImGui::Text("M41: %f", transform.M41);
		ImGui::Text("M42: %f", transform.M42);
		ImGui::Text("M43: %f", transform.M43);
		ImGui::Text("M44: %f", transform.M44);

		ImGui::Separator();
		ImGui::Text("Yaw: %f", pClientSoldierEntity->authorativeYaw);
		ImGui::Text("CosY: %f", cos(pClientSoldierEntity->authorativeYaw));
		ImGui::Text("SinY: %f", sin(pClientSoldierEntity->authorativeYaw));

		ImGui::Separator();
	}

	ImGui::Text("Team: %d", pClientPlayer->teamId);

	ClientGameContext* pClientGameContext = ClientGameContext::GetInstance();
	if (IsValidPtr(pClientGameContext))
	{
		Level* pLevel = pClientGameContext->level;
		if (IsValidPtr(pLevel))
		{
			ImGui::Text("Level: %s", pLevel->name);

			GameMode* pGameMode = pLevel->gameMode;
			if (IsValidPtr(pGameMode))
				ImGui::Text("Mode: %s", pGameMode->name);
		}
	}

	ImGuiIO& io = ImGui::GetIO();
	ImGui::Text("FPS: %.1f (%.3f ms)", io.Framerate, 1000.0f / io.Framerate);

	ImGui::End();
}

void ShowRadar()
{
	ClientPlayer* pClientPlayer = GetLocalPlayer();
	if (!IsValidPtr(pClientPlayer))
		return;

	ImGuiWindowFlags window_flags =
		ImGuiWindowFlags_NoDecoration |
		ImGuiWindowFlags_NoSavedSettings |
		ImGuiWindowFlags_NoMove;

	const int pos = 10;
	const int size = 500;

	// 左上角
	ImGui::SetNextWindowPos(ImVec2(pos, pos), ImGuiCond_Always);
	ImGui::SetNextWindowSize(ImVec2(size, size));
	ImGui::SetNextWindowBgAlpha(0.5f);

	ImGui::Begin("Show Radar", NULL, window_flags);

	// 雷达地图十字线
	draw->AddLine(ImVec2(pos, pos + size / 2), ImVec2(pos + size, pos + size / 2), white_color_50);
	draw->AddLine(ImVec2(pos + size / 2, pos), ImVec2(pos + size / 2, pos + size), white_color_50);

	ClientVehicleEntity* pClientVehicleEntity = pClientPlayer->clientVehicleEntity;
	ClientSoldierEntity* pClientSoldierEntity = pClientPlayer->clientSoldierEntity;
	Matrix4x4 transform{};

	if (IsValidPtr(pClientVehicleEntity))
	{
		// 载具
		pClientVehicleEntity->GetTransform(&transform);
	}
	else if (IsValidPtr(pClientSoldierEntity))
	{
		// 士兵
		transform = pClientSoldierEntity->transform;
	}

	// 绘制雷达2D坐标
	// 当玩家士兵为有效指针时才绘制
	if (IsValidPtr(pClientSoldierEntity))
	{
		if (pClientPlayer->teamId == 1)
		{
			for (int i = 0; i < core->spawnPos.Team1PosList.size(); i++)
			{
				Vector3 location(
					core->spawnPos.Team1PosList[i].M41,
					core->spawnPos.Team1PosList[i].M42,
					core->spawnPos.Team1PosList[i].M43
				);

				Vector2 radar2d = RadarRotatePoint(transform, core->spawnPos.Team1PosList[i],
					pos, pos, size, size, pClientSoldierEntity->authorativeYaw, room_scale);

				draw->AddCircle(ImVec2(radar2d.X, radar2d.Y), 3.f, yellow_color, 0.f);

				auto indexStr = std::to_string(i);
				draw->AddText(radar2d, white_color, indexStr.c_str());
			}
		}
		else if (pClientPlayer->teamId == 2)
		{
			for (int i = 0; i < core->spawnPos.Team2PosList.size(); i++)
			{
				Vector3 location(
					core->spawnPos.Team2PosList[i].M41,
					core->spawnPos.Team2PosList[i].M42,
					core->spawnPos.Team2PosList[i].M43
				);

				Vector2 radar2d = RadarRotatePoint(transform, core->spawnPos.Team2PosList[i],
					pos, pos, size, size, pClientSoldierEntity->authorativeYaw, room_scale);

				draw->AddCircle(ImVec2(radar2d.X, radar2d.Y), 3.f, yellow_color, 0.f);

				auto indexStr = std::to_string(i);
				draw->AddText(radar2d, white_color, indexStr.c_str());
			}
		}
	}

	ImGui::End();
}

void ShowMenu()
{
	ClientPlayer* pClientPlayer = GetLocalPlayer();
	if (!IsValidPtr(pClientPlayer))
		return;

	ImGuiWindowFlags window_flags =
		ImGuiWindowFlags_NoDecoration |
		ImGuiWindowFlags_NoSavedSettings |
		ImGuiWindowFlags_NoMove;

	const ImGuiViewport* viewport = ImGui::GetMainViewport();

	// 右上角
	ImGui::SetNextWindowPos(ImVec2(viewport->WorkSize.x - 10, 10), ImGuiCond_Always, ImVec2(1, 0));
	ImGui::SetNextWindowSize(ImVec2(600, 700));
	ImGui::SetNextWindowBgAlpha(0.8f);

	ImGui::Begin("BF1 Player Spawn Point Record Tools", NULL, window_flags);

	// 左侧布局
	if (ImGui::BeginChild("left pane", ImVec2(310, 0), ImGuiChildFlags_Border))
	{
		if (pClientPlayer->teamId == 1)
		{
			// 如果未选择，但是有实际数据，则默认选择第一项
			if (core->seleTeam1Index == -1 && core->spawnPos.Team1PosList.size() != 0)
				core->seleTeam1Index = 0;

			for (int i = 0; i < core->spawnPos.Team1PosList.size(); i++)
			{
				std::string lableStr;

				lableStr.append("[");
				lableStr.append(std::to_string(i));
				lableStr.append("] ");
				lableStr.append(std::to_string(core->spawnPos.Team1PosList[i].M41));
				lableStr.append(", ");
				lableStr.append(std::to_string(core->spawnPos.Team1PosList[i].M42));
				lableStr.append(", ");
				lableStr.append(std::to_string(core->spawnPos.Team1PosList[i].M43));

				bool is_selected = (core->seleTeam1Index == i);
				if (ImGui::Selectable(lableStr.c_str(), is_selected))
					core->seleTeam1Index = i;

				if (is_selected)
				{
					ImGui::SetItemDefaultFocus();
					ImGui::ScrollToItem();
				}
			}
		}
		else if (pClientPlayer->teamId == 2)
		{
			// 如果未选择，但是有实际数据，则默认选择第一项
			if (core->seleTeam2Index == -1 && core->spawnPos.Team2PosList.size() != 0)
				core->seleTeam2Index = 0;

			for (int i = 0; i < core->spawnPos.Team2PosList.size(); i++)
			{
				std::string lableStr;

				lableStr.append("[");
				lableStr.append(std::to_string(i));
				lableStr.append("] ");
				lableStr.append(std::to_string(core->spawnPos.Team2PosList[i].M41));
				lableStr.append(", ");
				lableStr.append(std::to_string(core->spawnPos.Team2PosList[i].M42));
				lableStr.append(", ");
				lableStr.append(std::to_string(core->spawnPos.Team2PosList[i].M43));

				bool is_selected = (core->seleTeam2Index == i);
				if (ImGui::Selectable(lableStr.c_str(), is_selected))
					core->seleTeam2Index = i;

				if (is_selected)
				{
					ImGui::SetItemDefaultFocus();
					ImGui::ScrollToItem();
				}
			}
		}

		ImGui::EndChild();
	}

	ImGui::SameLine();

	// 右侧布局
	ImGui::BeginGroup();

	if (ImGui::BeginChild("item view", ImVec2(0, -ImGui::GetFrameHeightWithSpacing())), ImGuiChildFlags_Border)
	{
		if (pClientPlayer->teamId == 1)
		{
			ImGui::SeparatorText("Team1");
			ImGui::Text("index: %d", core->seleTeam1Index);
			ImGui::SameLine();
			ImGui::Text("count: %d", core->spawnPos.Team1PosList.size());

			ImGui::SliderFloat("room scale", &room_scale, 0.1f, 2.0f, "%.2f");

			ImGui::Checkbox("vehicle mode", &is_use_vehicle_mode);
			if (is_use_vehicle_mode)
			{
				global->points = 56.0f;			// 组成3D圆需要的点数量
				global->radius = 2.4f;			// 3D圆的半径
				global->line_length = 4.8f;		// 半径的2倍
			}
			else
			{
				global->points = 18.0f;			// 组成3D圆需要的点数量
				global->radius = 0.8f;			// 3D圆的半径
				global->line_length = 1.6f;		// 半径的2倍
			}

			// selectedIndex 必须是合法范围
			if (core->seleTeam1Index >= 0 && core->seleTeam1Index < core->spawnPos.Team1PosList.size())
			{
				ImGui::SeparatorText("Transform");
				ImGui::InputFloat("M11", &core->spawnPos.Team1PosList[core->seleTeam1Index].M11, 0.5f, 0.1f, "%f");
				ImGui::InputFloat("M12", &core->spawnPos.Team1PosList[core->seleTeam1Index].M12, 0.5f, 0.1f, "%f");
				ImGui::InputFloat("M13", &core->spawnPos.Team1PosList[core->seleTeam1Index].M13, 0.5f, 0.1f, "%f");
				ImGui::InputFloat("M14", &core->spawnPos.Team1PosList[core->seleTeam1Index].M14, 0.5f, 0.1f, "%f");
				ImGui::Separator();
				ImGui::InputFloat("M21", &core->spawnPos.Team1PosList[core->seleTeam1Index].M21, 0.5f, 0.1f, "%f");
				ImGui::InputFloat("M22", &core->spawnPos.Team1PosList[core->seleTeam1Index].M22, 0.5f, 0.1f, "%f");
				ImGui::InputFloat("M23", &core->spawnPos.Team1PosList[core->seleTeam1Index].M23, 0.5f, 0.1f, "%f");
				ImGui::InputFloat("M24", &core->spawnPos.Team1PosList[core->seleTeam1Index].M24, 0.5f, 0.1f, "%f");
				ImGui::Separator();
				ImGui::InputFloat("M31", &core->spawnPos.Team1PosList[core->seleTeam1Index].M31, 0.5f, 0.1f, "%f");
				ImGui::InputFloat("M32", &core->spawnPos.Team1PosList[core->seleTeam1Index].M32, 0.5f, 0.1f, "%f");
				ImGui::InputFloat("M33", &core->spawnPos.Team1PosList[core->seleTeam1Index].M33, 0.5f, 0.1f, "%f");
				ImGui::InputFloat("M34", &core->spawnPos.Team1PosList[core->seleTeam1Index].M34, 0.5f, 0.1f, "%f");
				ImGui::Separator();
				ImGui::InputFloat("M41", &core->spawnPos.Team1PosList[core->seleTeam1Index].M41, 0.5f, 0.1f, "%f");
				ImGui::InputFloat("M42", &core->spawnPos.Team1PosList[core->seleTeam1Index].M42, 0.5f, 0.1f, "%f");
				ImGui::InputFloat("M43", &core->spawnPos.Team1PosList[core->seleTeam1Index].M43, 0.5f, 0.1f, "%f");
				ImGui::InputFloat("M44", &core->spawnPos.Team1PosList[core->seleTeam1Index].M44, 0.5f, 0.1f, "%f");
			}
		}
		else if (pClientPlayer->teamId == 2)
		{
			ImGui::SeparatorText("Team2");
			ImGui::Text("index: %d", core->seleTeam2Index);
			ImGui::SameLine();
			ImGui::Text("count: %d", core->spawnPos.Team2PosList.size());

			ImGui::SliderFloat("room scale", &room_scale, 0.1f, 2.0f, "%.2f");

			// selectedIndex 必须是合法范围
			if (core->seleTeam2Index >= 0 && core->seleTeam2Index < core->spawnPos.Team2PosList.size())
			{
				ImGui::SeparatorText("Transform");
				ImGui::InputFloat("M11", &core->spawnPos.Team2PosList[core->seleTeam2Index].M11, 0.5f, 0.1f, "%f");
				ImGui::InputFloat("M12", &core->spawnPos.Team2PosList[core->seleTeam2Index].M12, 0.5f, 0.1f, "%f");
				ImGui::InputFloat("M13", &core->spawnPos.Team2PosList[core->seleTeam2Index].M13, 0.5f, 0.1f, "%f");
				ImGui::InputFloat("M14", &core->spawnPos.Team2PosList[core->seleTeam2Index].M14, 0.5f, 0.1f, "%f");
				ImGui::Separator();
				ImGui::InputFloat("M21", &core->spawnPos.Team2PosList[core->seleTeam2Index].M21, 0.5f, 0.1f, "%f");
				ImGui::InputFloat("M22", &core->spawnPos.Team2PosList[core->seleTeam2Index].M22, 0.5f, 0.1f, "%f");
				ImGui::InputFloat("M23", &core->spawnPos.Team2PosList[core->seleTeam2Index].M23, 0.5f, 0.1f, "%f");
				ImGui::InputFloat("M24", &core->spawnPos.Team2PosList[core->seleTeam2Index].M24, 0.5f, 0.1f, "%f");
				ImGui::Separator();
				ImGui::InputFloat("M31", &core->spawnPos.Team2PosList[core->seleTeam2Index].M31, 0.5f, 0.1f, "%f");
				ImGui::InputFloat("M32", &core->spawnPos.Team2PosList[core->seleTeam2Index].M32, 0.5f, 0.1f, "%f");
				ImGui::InputFloat("M33", &core->spawnPos.Team2PosList[core->seleTeam2Index].M33, 0.5f, 0.1f, "%f");
				ImGui::InputFloat("M34", &core->spawnPos.Team2PosList[core->seleTeam2Index].M34, 0.5f, 0.1f, "%f");
				ImGui::Separator();
				ImGui::InputFloat("M41", &core->spawnPos.Team2PosList[core->seleTeam2Index].M41, 0.5f, 0.1f, "%f");
				ImGui::InputFloat("M42", &core->spawnPos.Team2PosList[core->seleTeam2Index].M42, 0.5f, 0.1f, "%f");
				ImGui::InputFloat("M43", &core->spawnPos.Team2PosList[core->seleTeam2Index].M43, 0.5f, 0.1f, "%f");
				ImGui::InputFloat("M44", &core->spawnPos.Team2PosList[core->seleTeam2Index].M44, 0.5f, 0.1f, "%f");
			}
		}

		ImGui::SeparatorText("R & W");

		// 读取
		if (ImGui::Button("Read From Json"))
			ImGui::OpenPopup("Read Modal Dialog");

		if (ImGui::BeginPopupModal("Read Modal Dialog", NULL, ImGuiWindowFlags_AlwaysAutoResize))
		{
			ImGui::Text("Do you want to read this json ?");
			ImGui::Separator();

			if (ImGui::Button("OK", ImVec2(120, 0)))
			{
				core->Read();
				ImGui::CloseCurrentPopup();
			}

			ImGui::SameLine();
			if (ImGui::Button("Cancel", ImVec2(120, 0)))
				ImGui::CloseCurrentPopup();

			ImGui::EndPopup();
		}

		ImGui::SameLine();
		// 保存
		if (ImGui::Button("Save As Json"))
			core->Save();

		ImGui::SeparatorText("Warnning");

		// 清空
		if (ImGui::Button("[Warn] Clear All Pos Data"))
			ImGui::OpenPopup("Clear Modal Dialog");

		if (ImGui::BeginPopupModal("Clear Modal Dialog", NULL, ImGuiWindowFlags_AlwaysAutoResize))
		{
			ImGui::Text("Do you want to clear all pos data ?");
			ImGui::Separator();

			if (ImGui::Button("OK", ImVec2(120, 0)))
			{
				core->Clear();
				ImGui::CloseCurrentPopup();
			}

			ImGui::SameLine();
			if (ImGui::Button("Cancel", ImVec2(120, 0)))
				ImGui::CloseCurrentPopup();

			ImGui::EndPopup();
		}

		ImGui::EndChild();
	}

	// 增加
	if (ImGui::Button("Add(F1)"))
		core->Add();

	ImGui::SameLine();
	// 删除
	if (ImGui::Button("Delect(F2)"))
		ImGui::OpenPopup("Delect Modal Dialog");

	if (ImGui::BeginPopupModal("Delect Modal Dialog", NULL, ImGuiWindowFlags_AlwaysAutoResize))
	{
		ImGui::Text("Do you want to delect this location ?");
		ImGui::Separator();

		if (ImGui::Button("OK", ImVec2(120, 0)))
		{
			core->Delect();
			ImGui::CloseCurrentPopup();
		}

		ImGui::SameLine();
		if (ImGui::Button("Cancel", ImVec2(120, 0)))
			ImGui::CloseCurrentPopup();

		ImGui::EndPopup();
	}

	ImGui::SameLine();
	// 删除2
	if (ImGui::Button("Delect2(F3)"))
		core->Delect2();

	ImGui::EndGroup();

	ImGui::End();
}

void Features::Run()
{
	if (global->isUnloadAll)
		return;

	ShowLoaction();
	ShowSelfInfo();

	if (global->isShowMenu)
	{
		ShowRadar();
		ShowMenu();
	}
}