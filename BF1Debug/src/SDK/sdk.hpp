#pragma once

#define OFFSET_CLIENTGAMECONTEXT 0x1437F8758
#define OFFSET_OBFUSCATEMGR 0x14351E058
#define OFFSET_GAMERENDERER 0x1439E7D08

// 检查指针地址是否有效
#define IsValidPtr(x) (x != NULL && (DWORD_PTR)x >= 0x10000 && (DWORD_PTR)x < 0x00007FFFFFFEFFFF)

template<typename T1, typename T2>
struct pair
{
	T1 first;
	T2 second;
};

template<typename T>
struct hash_node
{
	pair<uint64_t, T*> mValue;
	hash_node<T>* mpNext;
};

template<typename T>
struct hashtable
{
	uint64_t vtable;
	hash_node<T>** mpBucketArray;
	unsigned int mnBucketCount;
	unsigned int mnElementCount;
	//... some other stuff
};

template<typename T>
struct hashtable_iterator
{
	hash_node<T>* mpNode;
	hash_node<T>** mpBucket;
};

class GameMode
{
public:
	char pad_0000[8]; //0x0000
	char* name; //0x0008
}; 

class Level
{
public:
	char pad_0000[40]; //0x0000
	char* name; //0x0028
	class GameMode* gameMode; //0x0030
};

class ClientPlayerManager {
};

class ClientGameContext {
public:
	char pad_0000[48]; //0x0000
	class Level* level; //0x0030
	char pad_0038[48]; //0x0038
	ClientPlayerManager* clientPlayerManager; //0x0068

	static ClientGameContext* GetInstance() {
		return *(ClientGameContext**)(OFFSET_CLIENTGAMECONTEXT);
	}
};

class HealthComponent {
public:
	char pad_0000[32]; //0x0000
	float m_Health; //0x0020
	float m_MaxHealth; //0x0024
	char pad_0028[24]; //0x0028
	float m_VehicleHealth; //0x0040
	char pad_0044[4092]; //0x0044
}; //Size: 0x1040

class ClientVehicleEntity {
};

class BoneCollisionComponent {
};

class ClientSoldierEntity {
public:
	char pad_0000[464]; //0x0000
	HealthComponent* healthcomponent; //0x01D0
	char pad_01D8[696]; //0x01D8
	BoneCollisionComponent* bonecollisioncomponent; //0x0490
	char pad_0498[363]; //0x0498
	uint8_t N00000670; //0x0603
	float authorativeYaw; //0x0604
	char pad_0608[41]; //0x0608
	uint8_t N00000521; //0x0631
	char pad_0632[6]; //0x0632
	uint8_t poseType; //0x0638
	char pad_0639[176]; //0x0639
	uint8_t N00000538; //0x06E9
	uint8_t N0000022B; //0x06EA
	uint8_t occluded; //0x06EB
	char pad_06EC[628]; //0x06EC
	Matrix4x4 transform; //0x0960
}; //Size: 0x104C

class ClientPlayer {
public:
	virtual ~ClientPlayer();
	virtual DWORD_PTR GetCharacterEntity(); //=> ClientSoldierEntity + 0x268 
	virtual DWORD_PTR GetCharacterUserData(); //=> PlayerCharacterUserData
	virtual class EntryComponent* GetEntryComponent();
	virtual bool InVehicle();
	virtual unsigned int getId();
	char _0x0008[16];
	char* name; //0x0018
	char pad_0020[32]; //0x0020
	char szName[8]; //0x0040
	char pad_0048[7144]; //0x0048
	uint8_t N00000393; //0x1C30
	uint8_t N0000042C; //0x1C31
	char pad_1C32[2]; //0x1C32
	uint8_t teamId; //0x1C34
	char pad_1C35[259]; //0x1C35
	ClientVehicleEntity* clientVehicleEntity; //0x1D38
	char pad_1D40[8]; //0x1D40
	ClientSoldierEntity* clientSoldierEntity; //0x1D48
	char pad_1D50[736]; //0x1D50
};

class RenderView {
public:
	char pad_0x0000[0x320]; //0x0000
	Matrix4x4 m_viewMatrixInverse; //0x0320
	char pad_0x0360[0x100]; //0x0360
	Matrix4x4 viewProj; //0x0460
	char pad_0x04A0[0x28]; //0x04A0
}; //Size: 0x05C0

class GameRenderer {
public:
	char pad_0000[96]; //0x0000
	class RenderView* renderView; //0x0060
	char pad_0068[4112]; //0x0068

	static GameRenderer* GetInstance() {
		return *(GameRenderer**)OFFSET_GAMERENDERER;
	}
}; //Size: 0x0088
