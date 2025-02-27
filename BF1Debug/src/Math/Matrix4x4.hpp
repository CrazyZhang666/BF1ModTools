#pragma once

using json = nlohmann::ordered_json;

struct Matrix4x4
{
	float M11, M12, M13, M14;
	float M21, M22, M23, M24;
	float M31, M32, M33, M34;
	float M41, M42, M43, M44;

	static Matrix4x4 from_json(const json& jsonData) {
		return Matrix4x4{
			jsonData["M11"].get<float>(),
			jsonData["M12"].get<float>(),
			jsonData["M13"].get<float>(),
			jsonData["M14"].get<float>(),

			jsonData["M21"].get<float>(),
			jsonData["M22"].get<float>(),
			jsonData["M23"].get<float>(),
			jsonData["M24"].get<float>(),

			jsonData["M31"].get<float>(),
			jsonData["M32"].get<float>(),
			jsonData["M33"].get<float>(),
			jsonData["M34"].get<float>(),

			jsonData["M41"].get<float>(),
			jsonData["M42"].get<float>(),
			jsonData["M43"].get<float>(),
			jsonData["M44"].get<float>()
		};
	}

	json to_json() const {
		json jsonData;

		jsonData["M11"] = M11;
		jsonData["M12"] = M12;
		jsonData["M13"] = M13;
		jsonData["M14"] = M14;

		jsonData["M21"] = M21;
		jsonData["M22"] = M22;
		jsonData["M23"] = M23;
		jsonData["M24"] = M24;

		jsonData["M31"] = M31;
		jsonData["M32"] = M32;
		jsonData["M33"] = M33;
		jsonData["M34"] = M34;

		jsonData["M41"] = M41;
		jsonData["M42"] = M42;
		jsonData["M43"] = M43;
		jsonData["M44"] = M44;

		return jsonData;
	}
};