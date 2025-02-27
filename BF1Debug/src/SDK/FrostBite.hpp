#pragma once

typedef DWORD64 QWORD;
typedef BYTE _BYTE;
typedef WORD _WORD;
typedef DWORD _DWORD;
typedef QWORD _QWORD;

// 哈希表查找
inline hashtable_iterator<_QWORD>* __fastcall hashtable_find(hashtable<_QWORD>* table, hashtable_iterator<_QWORD>* iterator, _QWORD key)
{
	unsigned int mnBucketCount = table->mnBucketCount;
	unsigned int startCount = (unsigned int)(key) % mnBucketCount;

	hash_node<_QWORD>* node = table->mpBucketArray[startCount];

	if (IsValidPtr(node) && node->mValue.first)
	{
		while (key != node->mValue.first)
		{
			node = node->mpNext;
			if (!node || !IsValidPtr(node))
				goto LABEL_4;
		}
		iterator->mpNode = node;
		iterator->mpBucket = &table->mpBucketArray[startCount];
	}
	else
	{
	LABEL_4:
		iterator->mpNode = table->mpBucketArray[mnBucketCount];
		iterator->mpBucket = &table->mpBucketArray[mnBucketCount];
	}

	return iterator;
}

// 解密玩家指针
inline ClientPlayer* EncryptedPlayerMgr__GetPlayer(QWORD encryptedPlayerMgr, int id)
{
	_QWORD XorValue1 = *(_QWORD*)(encryptedPlayerMgr + 0x20) ^ *(_QWORD*)(encryptedPlayerMgr + 0x08);
	_QWORD XorValue2 = (XorValue1 ^ *(_QWORD*)(encryptedPlayerMgr + 0x10));
	if (!IsValidPtr(XorValue2))
		return nullptr;

	_QWORD Player = XorValue1 ^ *(_QWORD*)(XorValue2 + 0x08 * id);

	return (ClientPlayer*)Player;
}

// 获取本地玩家指针
inline ClientPlayer* GetLocalPlayer()
{
	ClientGameContext* pClientGameContext = ClientGameContext::GetInstance();
	if (!IsValidPtr(pClientGameContext))
		return nullptr;

	ClientPlayerManager* pPlayerManager = pClientGameContext->clientPlayerManager;
	if (!IsValidPtr(pPlayerManager))
		return nullptr;

	_QWORD pObfuscationMgr = *(_QWORD*)OFFSET_OBFUSCATEMGR;

	_QWORD localPlayerListXorValue = *(_QWORD*)((_QWORD)pPlayerManager + 0xF0);
	_QWORD localPlayerListKey = localPlayerListXorValue ^ *(_QWORD*)(pObfuscationMgr + 0x70);

	hashtable<_QWORD>* table = (hashtable<_QWORD>*)(pObfuscationMgr + 0x08);
	hashtable_iterator<_QWORD> iterator = { 0 };

	hashtable_find(table, &iterator, localPlayerListKey);

	if (iterator.mpNode == table->mpBucketArray[table->mnBucketCount])
		return nullptr;

	_QWORD encryptedPlayerMgr = (_QWORD)iterator.mpNode->mValue.second;
	if (!IsValidPtr(encryptedPlayerMgr))
		return nullptr;

	_DWORD maxPlayerCount = *(_DWORD*)(encryptedPlayerMgr + 0x18);
	if (maxPlayerCount != 1)
		return nullptr;

	return EncryptedPlayerMgr__GetPlayer(encryptedPlayerMgr, 0);
}

// 世界坐标转屏幕坐标
inline bool WorldToScreen(Vector3 position, Vector2& screen)
{
	// 获取游戏渲染实例
	GameRenderer* game_render = GameRenderer::GetInstance();

	// 验证游戏渲染实例是否有效
	if (!IsValidPtr(game_render) || !IsValidPtr(game_render->renderView))
		return false;

	// 获取视图投影
	auto viewProj = game_render->renderView->viewProj;

	/*
	 *   x  y     z
	 *	11 12 12 14
	 *	21 22 23 24
	 *	31 32 33 34
	 *	41 42 43 44
	 */

	 // 计算这个向量的 W 分量
	float w = viewProj.M14 * position.X + viewProj.M24 * position.Y + viewProj.M34 * position.Z + viewProj.M44;

	// 检查目标玩家是不是在我们后面
	if (w < 0.01f)
		return false;

	// 计算这个向量的 X 分量
	float x = viewProj.M11 * position.X + viewProj.M21 * position.Y + viewProj.M31 * position.Z + viewProj.M41;
	// 计算这个向量的 Y 分量
	float y = viewProj.M12 * position.X + viewProj.M22 * position.Y + viewProj.M32 * position.Z + viewProj.M42;

	ImGuiIO io = ImGui::GetIO();
	// 获取游戏窗口大小信息
	Vector2 display_size = Vector2(io.DisplaySize.x / 2, io.DisplaySize.y / 2);

	// 计算屏幕坐标
	screen = Vector2(display_size.X + display_size.X * x / w, display_size.Y - display_size.Y * y / w);

	// 返回成功
	return true;
}

// 获取雷达坐标
inline Vector2 RadarRotatePoint(Matrix4x4 localTransform, Matrix4x4 targetTransform, int posX, int posY, int sizeX, int sizeY, float angle, float zoom)
{
	Vector3 localPlayerPos(
		localTransform.M41, localTransform.M42, localTransform.M43
	);

	Vector3 targetPlayerPos(
		targetTransform.M41, targetTransform.M42, targetTransform.M43
	);

	float zs = localPlayerPos.Z - targetPlayerPos.Z;
	float xs = localPlayerPos.X - targetPlayerPos.X;

	double yaw = -(double)angle;

	float single = xs * (float)cos(yaw) - zs * (float)sin(yaw);
	float ypisilum1 = xs * (float)sin(yaw) + zs * (float)cos(yaw);

	single *= zoom * 5.0f;
	ypisilum1 *= zoom * 5.0f;

	single += posX + sizeX / 2.0f;
	ypisilum1 += posY + sizeY / 2.0f;

	if (single < posX)
		single = posX;

	if (ypisilum1 < posY)
		ypisilum1 = posY;

	if (single > posX + sizeX - 3.0f)
		single = posX + sizeX - 3.0f;

	if (ypisilum1 > posY + sizeY - 3.0f)
		ypisilum1 = posY + sizeY - 3.0f;

	return Vector2(single, ypisilum1);
}