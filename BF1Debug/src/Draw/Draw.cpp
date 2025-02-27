#include "../framework.h"

Draw* draw = new Draw;

void Draw::AddLine(const ImVec2& from, const ImVec2& to, const ImColor& color, float thickness)
{
	ImDrawList* drawList = ImGui::GetWindowDrawList();

	drawList->AddLine(from, to, ImGui::ColorConvertFloat4ToU32(color), thickness);
}

void Draw::AddText(Vector2 position, const ImColor& color, const char* text)
{
	ImDrawList* drawList = ImGui::GetWindowDrawList();

	ImGuiIO& io = ImGui::GetIO();
	ImFont* font = io.Fonts->Fonts[0];

	drawList->PushTextureID(io.Fonts->TexID);

	drawList->AddText(font, font->FontSize, ImVec2(position.X, position.Y), ImGui::ColorConvertFloat4ToU32(color), text);

	drawList->PopTextureID();
}

void Draw::AddText2(Vector3 position, const ImColor& color, const char* text)
{
	Vector2 pos2d;
	if (!WorldToScreen(position, pos2d))
		return;

	this->AddText(pos2d, color, text);
}

void Draw::AddCircle(const ImVec2& position, float radius, const ImColor& color, int segments)
{
	ImDrawList* drawList = ImGui::GetWindowDrawList();

	drawList->AddCircle(position, radius, ImGui::ColorConvertFloat4ToU32(color), segments);
}

void Draw::AddCircleFilled(const ImVec2& position, float radius, const ImColor& color, int segments)
{
	ImDrawList* drawList = ImGui::GetWindowDrawList();

	drawList->AddCircleFilled(position, radius, ImGui::ColorConvertFloat4ToU32(color), segments);
}

void Draw::AddCircle3D(Vector3 position, float points, float radius, const ImColor& color)
{
	float step = M_PI * 2.0f / points;

	for (float a = 0; a < (M_PI * 2.0f); a += step)
	{
		Vector3 start(
			radius * cosf(a) + position.X,
			position.Y,
			radius * sinf(a) + position.Z
		);

		Vector3 end(
			radius * cosf(a + step) + position.X,
			position.Y,
			radius * sinf(a + step) + position.Z
		);

		Vector2 start2d, end2d;
		if (!WorldToScreen(start, start2d) || !WorldToScreen(end, end2d))
			return;

		AddLine(ImVec2(start2d.X, start2d.Y), ImVec2(end2d.X, end2d.Y), color);
	}
}

void Draw::AddYawLine(Matrix4x4 transform, float length, const ImColor& color)
{
	// 玩家坐标
	Vector3 start3d(
		transform.M41, transform.M42, transform.M43
	);

	// 玩家坐标正前方偏移
	Vector3 end3d(
		0 * transform.M11 - length * transform.M13,
		0,
		length * transform.M11 + 0 * transform.M13
	);

	// 玩家正前方坐标
	end3d.X += transform.M41;
	end3d.Y += transform.M42;
	end3d.Z += transform.M43;

	Vector2 start2d, end2d;
	if (!WorldToScreen(start3d, start2d) || !WorldToScreen(end3d, end2d))
		return;

	draw->AddLine(ImVec2(start2d.X, start2d.Y), ImVec2(end2d.X, end2d.Y), color);
}