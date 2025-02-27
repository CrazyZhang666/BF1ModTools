#pragma once

using json = nlohmann::ordered_json;

struct SpawnPos
{
	std::string MapName;
	std::string GameMode;
	std::vector<Matrix4x4> Team1PosList;
	std::vector<Matrix4x4> Team2PosList;

	void from_json(const json& jsonData) {
		MapName = "";
		GameMode = "";

		Team1PosList.clear();
		auto team1List = jsonData["Team1PosList"].get<std::vector<json>>();
		for (auto& matrixJson : team1List) {
			Team1PosList.push_back(Matrix4x4::from_json(matrixJson));
		}

		Team2PosList.clear();
		auto team2List = jsonData["Team2PosList"].get<std::vector<json>>();
		for (auto& matrixJson : team2List) {
			Team2PosList.push_back(Matrix4x4::from_json(matrixJson));
		}
	}

	json to_json() const {
		json jsonData;

		jsonData["MapName"] = MapName;
		jsonData["GameMode"] = GameMode;

		jsonData["Team1PosList"] = json::array();
		for (const auto& matrix : Team1PosList) {
			jsonData["Team1PosList"].push_back(matrix.to_json());
		}

		jsonData["Team2PosList"] = json::array();
		for (const auto& matrix : Team2PosList) {
			jsonData["Team2PosList"].push_back(matrix.to_json());
		}

		return jsonData;
	}
};

class Core
{
public:
	int seleTeam1Index = -1;
	int seleTeam2Index = -1;
	SpawnPos spawnPos;

public:
	void Add();
	void Delect();
	void Delect2();
	void Read();
	void Save();
	void Clear();
};

extern Core* core;