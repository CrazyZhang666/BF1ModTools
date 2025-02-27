#pragma once

class Draw
{
public:
	void AddLine(const ImVec2& from, const ImVec2& to, const ImColor& color, float thickness = 1.f);
	void AddText(Vector2 position, const ImColor& color, const char* text);
	void AddText2(Vector3 position, const ImColor& color, const char* text);
	void AddCircle(const ImVec2& position, float radius, const ImColor& color, int segments);
	void AddCircleFilled(const ImVec2& position, float radius, const ImColor& color, int segments);

	void AddCircle3D(Vector3 position, float points, float radius, const ImColor& color);

	void AddYawLine(Matrix4x4 transform, float length, const ImColor& color);
};

extern Draw* draw;
