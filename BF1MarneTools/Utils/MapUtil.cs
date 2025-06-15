using BF1MarneTools.Models;

namespace BF1MarneTools.Utils;

public static class MapUtil
{
    public static readonly List<MapInfo> GameMapInfoDb = [];
    public static readonly List<ModeInfo> GameModeInfoDb = [];

    static MapUtil()
    {
        // 1 - 亚眠
        GameMapInfoDb.Add(new()
        {
            Name = "亚眠",
            Name2 = "AMIENS",
            Name3 = "MP_Amiens",
            DLC = "本体",
            Url = "Levels/MP/MP_Amiens/MP_Amiens",
            Modes = ["Conquest0", "Rush0", "BreakthroughLarge0", "Breakthrough0", "Possession0", "TugOfWar0", "Domination0", "TeamDeathMatch0"]
        });

        // 2 - 流血宴厅
        GameMapInfoDb.Add(new()
        {
            Name = "流血宴厅",
            Name2 = "BALLROOM BLITZ",
            Name3 = "MP_Chateau",
            DLC = "本体",
            Url = "Levels/MP/MP_Chateau/MP_Chateau",
            Modes = ["Conquest0", "Rush0", "BreakthroughLarge0", "Breakthrough0", "Possession0", "TugOfWar0", "Domination0", "TeamDeathMatch0"]
        });

        // 3 - 西奈沙漠
        GameMapInfoDb.Add(new()
        {
            Name = "西奈沙漠",
            Name2 = "SINAI DESERT",
            Name3 = "MP_Desert",
            DLC = "本体",
            Url = "Levels/MP/MP_Desert/MP_Desert",
            Modes = ["Conquest0", "Rush0", "BreakthroughLarge0", "Breakthrough0", "Possession0", "TugOfWar0", "Domination0", "TeamDeathMatch0"]
        });

        // 4 - 法欧堡
        GameMapInfoDb.Add(new()
        {
            Name = "法欧堡",
            Name2 = "FAO FORTRESS",
            Name3 = "MP_FaoFortress",
            DLC = "本体",
            Url = "Levels/MP/MP_FaoFortress/MP_FaoFortress",
            Modes = ["Conquest0", "Rush0", "BreakthroughLarge0", "Breakthrough0", "Possession0", "TugOfWar0", "Domination0", "TeamDeathMatch0"]
        });

        // 5 - 阿尔贡森林
        GameMapInfoDb.Add(new()
        {
            Name = "阿尔贡森林",
            Name2 = "ARGONNE FOREST",
            Name3 = "MP_Forest",
            DLC = "本体",
            Url = "Levels/MP/MP_Forest/MP_Forest",
            Modes = ["Conquest0", "Rush0", "BreakthroughLarge0", "Breakthrough0", "Possession0", "TugOfWar0", "Domination0", "TeamDeathMatch0"]
        });

        // 6 - 帝国边境（无前线模式）
        GameMapInfoDb.Add(new()
        {
            Name = "帝国边境",
            Name2 = "EMPIRE'S EDGE",
            Name3 = "MP_ItalianCoast",
            DLC = "本体",
            Url = "Levels/MP/MP_ItalianCoast/MP_ItalianCoast",
            Modes = ["Conquest0", "Rush0", "BreakthroughLarge0", "Breakthrough0", "Possession0", "Domination0", "TeamDeathMatch0"]
        });

        // 7 - 格拉巴山
        GameMapInfoDb.Add(new()
        {
            Name = "格拉巴山",
            Name2 = "MONTE GRAPPA",
            Name3 = "MP_MountainFort",
            DLC = "本体",
            Url = "Levels/MP/MP_MountainFort/MP_MountainFort",
            Modes = ["Conquest0", "Rush0", "BreakthroughLarge0", "Breakthrough0", "Possession0", "TugOfWar0", "Domination0", "TeamDeathMatch0"]
        });

        // 8 - 圣康坦的伤痕
        GameMapInfoDb.Add(new()
        {
            Name = "圣康坦的伤痕",
            Name2 = "ST. QUENTIN SCAR",
            Name3 = "MP_Scar",
            DLC = "本体",
            Url = "Levels/MP/MP_Scar/MP_Scar",
            Modes = ["Conquest0", "Rush0", "BreakthroughLarge0", "Breakthrough0", "Possession0", "TugOfWar0", "Domination0", "TeamDeathMatch0"]
        });

        // 9 - 苏伊士
        GameMapInfoDb.Add(new()
        {
            Name = "苏伊士",
            Name2 = "SUEZ",
            Name3 = "MP_Suez",
            DLC = "本体",
            Url = "Levels/MP/MP_Suez/MP_Suez",
            Modes = ["Conquest0", "Rush0", "BreakthroughLarge0", "Breakthrough0", "Possession0", "TugOfWar0", "Domination0", "TeamDeathMatch0"]
        });

        ///////////////////////////////

        // 10 - 庞然暗影
        GameMapInfoDb.Add(new()
        {
            Name = "庞然暗影",
            Name2 = "GIANT'S SHADOW",
            Name3 = "MP_Giant",
            DLC = "DLC0",
            Url = "Xpack0/Levels/MP/MP_Giant/MP_Giant",
            Modes = ["Conquest0", "Rush0", "BreakthroughLarge0", "Breakthrough0", "Possession0", "TugOfWar0", "Domination0", "TeamDeathMatch0"]
        });

        ///////////////////////////////

        // 11 - 苏瓦松
        GameMapInfoDb.Add(new()
        {
            Name = "苏瓦松",
            Name2 = "SOISSONS",
            Name3 = "MP_Fields",
            DLC = "DLC1",
            Url = "Xpack1/Levels/MP_Fields/MP_Fields",
            Modes = ["Conquest0", "Rush0", "BreakthroughLarge0", "Breakthrough0", "Possession0", "TugOfWar0", "Domination0", "TeamDeathMatch0"]
        });

        // 12 - 决裂
        GameMapInfoDb.Add(new()
        {
            Name = "决裂",
            Name2 = "RUPTURE",
            Name3 = "MP_Graveyard",
            DLC = "DLC1",
            Url = "Xpack1/Levels/MP_Graveyard/MP_Graveyard",
            Modes = ["Conquest0", "Rush0", "BreakthroughLarge0", "Breakthrough0", "Possession0", "TugOfWar0", "Domination0", "TeamDeathMatch0"]
        });

        // 13 - 法乌克斯要塞
        GameMapInfoDb.Add(new()
        {
            Name = "法乌克斯要塞",
            Name2 = "FORT DE VAUX",
            Name3 = "MP_Underworld",
            DLC = "DLC1",
            Url = "Xpack1/Levels/MP_Underworld/MP_Underworld",
            Modes = ["Conquest0", "Rush0", "BreakthroughLarge0", "Breakthrough0", "Possession0", "TugOfWar0", "Domination0", "TeamDeathMatch0"]
        });

        // 14 - 凡尔登高地
        GameMapInfoDb.Add(new()
        {
            Name = "凡尔登高地",
            Name2 = "VERDUN HEIGHTS",
            Name3 = "MP_Verdun",
            DLC = "DLC1",
            Url = "Xpack1/Levels/MP_Verdun/MP_Verdun",
            Modes = ["Conquest0", "Rush0", "BreakthroughLarge0", "Breakthrough0", "Possession0", "TugOfWar0", "Domination0", "TeamDeathMatch0"]
        });

        ///////////////////////////////

        // 15 - 攻占托尔
        GameMapInfoDb.Add(new()
        {
            Name = "攻占托尔",
            Name2 = "PRISE DE TAHURE",
            Name3 = "MP_ShovelTown",
            DLC = "DLC1-3",
            Url = "Xpack1-3/Levels/MP_ShovelTown/MP_ShovelTown",
            Modes = ["Conquest0", "Rush0", "BreakthroughLarge0", "Breakthrough0", "Possession0", "TugOfWar0", "Domination0", "TeamDeathMatch0"]
        });

        // 16 - 尼维尔之夜
        GameMapInfoDb.Add(new()
        {
            Name = "尼维尔之夜",
            Name2 = "NIVELLE NIGHTS",
            Name3 = "MP_Trench",
            DLC = "DLC1-3",
            Url = "Xpack1-3/Levels/MP_Trench/MP_Trench",
            Modes = ["Conquest0", "Rush0", "Possession0", "TugOfWar0", "Domination0", "TeamDeathMatch0"]
        });

        ///////////////////////////////

        // 17 - 勃鲁希洛夫关口
        GameMapInfoDb.Add(new()
        {
            Name = "勃鲁希洛夫关口",
            Name2 = "BRUSILOV KEEP",
            Name3 = "MP_Bridge",
            DLC = "DLC2",
            Url = "Xpack2/Levels/MP/MP_Bridge/MP_Bridge",
            Modes = ["Conquest0", "Rush0", "BreakthroughLarge0", "Breakthrough0", "Possession0", "Domination0", "TeamDeathMatch0", "ZoneControl0"]
        });

        // 18 - 阿尔比恩
        GameMapInfoDb.Add(new()
        {
            Name = "阿尔比恩",
            Name2 = "ALBION",
            Name3 = "MP_Islands",
            DLC = "DLC2",
            Url = "Xpack2/Levels/MP/MP_Islands/MP_Islands",
            Modes = ["Conquest0", "Rush0", "Possession0", "Domination0", "TeamDeathMatch0", "ZoneControl0"]
        });

        // 19 - 武普库夫山口
        GameMapInfoDb.Add(new()
        {
            Name = "武普库夫山口",
            Name2 = "LUPKÓW PASS",
            Name3 = "MP_Ravines",
            DLC = "DLC2",
            Url = "Xpack2/Levels/MP/MP_Ravines/MP_Ravines",
            Modes = ["Conquest0", "Rush0", "BreakthroughLarge0", "Breakthrough0", "Possession0", "Domination0", "TeamDeathMatch0", "ZoneControl0"]
        });

        // 20 - 察里津
        GameMapInfoDb.Add(new()
        {
            Name = "察里津",
            Name2 = "TSARITSYN",
            Name3 = "MP_Tsaritsyn",
            DLC = "DLC2",
            Url = "Xpack2/Levels/MP/MP_Tsaritsyn/MP_Tsaritsyn",
            Modes = ["Conquest0", "Rush0", "BreakthroughLarge0", "Breakthrough0", "Possession0", "Domination0", "TeamDeathMatch0", "ZoneControl0"]
        });

        // 21 - 加利西亚
        GameMapInfoDb.Add(new()
        {
            Name = "加利西亚",
            Name2 = "GALICIA",
            Name3 = "MP_Valley",
            DLC = "DLC2",
            Url = "Xpack2/Levels/MP/MP_Valley/MP_Valley",
            Modes = ["Conquest0", "Rush0", "BreakthroughLarge0", "Breakthrough0", "Possession0", "Domination0", "TeamDeathMatch0", "ZoneControl0"]
        });

        // 22 - 窝瓦河
        GameMapInfoDb.Add(new()
        {
            Name = "窝瓦河",
            Name2 = "VOLGA RIVER",
            Name3 = "MP_Volga",
            DLC = "DLC2",
            Url = "Xpack2/Levels/MP/MP_Volga/MP_Volga",
            Modes = ["Conquest0", "Rush0", "BreakthroughLarge0", "Breakthrough0", "Possession0", "Domination0", "TeamDeathMatch0", "ZoneControl0"]
        });

        ///////////////////////////////

        // 23 - 海丽丝岬
        GameMapInfoDb.Add(new()
        {
            Name = "海丽丝岬",
            Name2 = "CAPE HELLES",
            Name3 = "MP_Beachhead",
            DLC = "DLC3",
            Url = "Xpack3/Levels/MP/MP_Beachhead/MP_Beachhead",
            Modes = ["Conquest0", "Rush0", "BreakthroughLarge0", "Breakthrough0", "Possession0", "Domination0", "TeamDeathMatch0"]
        });

        // 24 - 泽布吕赫
        GameMapInfoDb.Add(new()
        {
            Name = "泽布吕赫",
            Name2 = "ZEEBRUGGE",
            Name3 = "MP_Harbor",
            DLC = "DLC3",
            Url = "Xpack3/Levels/MP/MP_Harbor/MP_Harbor",
            Modes = ["Conquest0", "Rush0", "BreakthroughLarge0", "Breakthrough0", "Possession0", "Domination0", "TeamDeathMatch0"]
        });

        // 25 - 黑尔戈兰湾
        GameMapInfoDb.Add(new()
        {
            Name = "黑尔戈兰湾",
            Name2 = "HELIGOLAND BIGHT",
            Name3 = "MP_Naval",
            DLC = "DLC3",
            Url = "Xpack3/Levels/MP/MP_Naval/MP_Naval",
            Modes = ["Conquest0"]
        });

        // 26 - 阿奇巴巴
        GameMapInfoDb.Add(new()
        {
            Name = "阿奇巴巴",
            Name2 = "ACHI BABA",
            Name3 = "MP_Ridge",
            DLC = "DLC3",
            Url = "Xpack3/Levels/MP/MP_Ridge/MP_Ridge",
            Modes = ["Conquest0", "Rush0", "BreakthroughLarge0", "Breakthrough0", "Possession0", "Domination0", "TeamDeathMatch0"]
        });

        ///////////////////////////////

        // 27 - 剃刀边缘
        GameMapInfoDb.Add(new()
        {
            Name = "剃刀边缘",
            Name2 = "RAZOR'S EDGE",
            Name3 = "MP_Alps",
            DLC = "DLC4",
            Url = "Xpack4/Levels/MP/MP_Alps/MP_Alps",
            Modes = ["AirAssault0"]
        });

        // 28 - 伦敦的呼唤：夜袭
        GameMapInfoDb.Add(new()
        {
            Name = "伦敦的呼唤：夜袭",
            Name2 = "LONDON CALLING: RAIDERS",
            Name3 = "MP_Blitz",
            DLC = "DLC4",
            Url = "Xpack4/Levels/MP/MP_Blitz/MP_Blitz",
            Modes = ["AirAssault0"]
        });

        // 29 - 帕斯尚尔
        GameMapInfoDb.Add(new()
        {
            Name = "帕斯尚尔",
            Name2 = "PASSCHENDAELE",
            Name3 = "MP_Hell",
            DLC = "DLC4",
            Url = "Xpack4/Levels/MP/MP_Hell/MP_Hell",
            Modes = ["Conquest0", "Rush0", "Possession0", "Domination0", "TeamDeathMatch0"]
        });

        // 30 - 伦敦的呼唤：灾祸
        GameMapInfoDb.Add(new()
        {
            Name = "伦敦的呼唤：灾祸",
            Name2 = "LONDON CALLING: SCOURGE",
            Name3 = "MP_London",
            DLC = "DLC4",
            Url = "Xpack4/Levels/MP/MP_London/MP_London",
            Modes = ["AirAssault0"]
        });

        // 31 - 索姆河
        GameMapInfoDb.Add(new()
        {
            Name = "索姆河",
            Name2 = "RIVER SOMME",
            Name3 = "MP_Offensive",
            DLC = "DLC4",
            Url = "Xpack4/Levels/MP/MP_Offensive/MP_Offensive",
            Modes = ["Conquest0", "Rush0", "BreakthroughLarge0", "Breakthrough0", "Possession0", "Domination0", "TeamDeathMatch0"]
        });

        // 32 - 卡波雷托
        GameMapInfoDb.Add(new()
        {
            Name = "卡波雷托",
            Name2 = "CAPORETTO",
            Name3 = "MP_River",
            DLC = "DLC4",
            Url = "Xpack4/Levels/MP/MP_River/MP_River",
            Modes = ["Conquest0", "Rush0", "Possession0", "Domination0", "TeamDeathMatch0"]
        });

        /////////////////////////////////////////////////////

        GameModeInfoDb.Add(new() { Code = "Conquest0", Name = "征服", Name2 = "CONQUEST" });
        GameModeInfoDb.Add(new() { Code = "Rush0", Name = "突袭", Name2 = "RUSH" });
        GameModeInfoDb.Add(new() { Code = "BreakthroughLarge0", Name = "行动模式", Name2 = "OPERATIONS" });
        GameModeInfoDb.Add(new() { Code = "Breakthrough0", Name = "闪击行动", Name2 = "SHOCK OPERATIONS" });
        GameModeInfoDb.Add(new() { Code = "Possession0", Name = "战争信鸽", Name2 = "WAR PIGEONS" });
        GameModeInfoDb.Add(new() { Code = "TugOfWar0", Name = "前线", Name2 = "FRONTLINES" });
        GameModeInfoDb.Add(new() { Code = "Domination0", Name = "抢攻", Name2 = "DOMINATION" });
        GameModeInfoDb.Add(new() { Code = "TeamDeathMatch0", Name = "团队死斗", Name2 = "TEAM DEATHMATCH" });
        GameModeInfoDb.Add(new() { Code = "ZoneControl0", Name = "空降补给", Name2 = "SUPPLY DROP" });
        GameModeInfoDb.Add(new() { Code = "AirAssault0", Name = "空中突袭", Name2 = "AIR ASSAULT" });
    }

    /// <summary>
    /// 通过名称获取地图图片
    /// </summary>
    public static string GetMapImageByName(string mapName)
    {
        return $"pack://application:,,,/BF1MarneTools;component/Assets/Images/Maps/{mapName}.jpg";
    }
}