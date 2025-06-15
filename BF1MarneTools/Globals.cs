using BF1MarneTools.Helper;
using BF1MarneTools.Utils;

namespace BF1MarneTools;

public static class Globals
{
    /// <summary>
    /// 应用程序名称
    /// </summary>
    public static readonly string AppName = Application.ResourceAssembly.GetName().Name;

    /// <summary>
    /// 配置文件路径
    /// </summary>
    private static readonly string _iniPath;

    ///////////////////////////////////

    /// <summary>
    /// 战地1安装目录
    /// </summary>
    public static string BF1InstallDir { get; private set; }

    /// <summary>
    /// 战地1主程序位置
    /// </summary>
    public static string BF1AppPath { get; private set; }

    ///////////////////////////////////

    /// <summary>
    /// 战地1选择对话框路径
    /// </summary>
    public static string GameSelectDir
    {
        get => ReadString("Dialog", "GameSelectDir");
        set => WriteString("Dialog", "GameSelectDir", value);
    }

    /// <summary>
    /// Mod选择对话框路径
    /// </summary>
    public static string ModSelectDir
    {
        get => ReadString("Dialog", "ModSelectDir");
        set => WriteString("Dialog", "ModSelectDir", value);
    }

    ///////////////////////////////////

    /// <summary>
    /// EA账号邮箱
    /// </summary>
    public static string Email
    {
        get => ReadString("EA", "Email");
        set => WriteString("EA", "Email", value);
    }

    /// <summary>
    /// EA账号密码
    /// </summary>
    public static string Password
    {
        get => ReadString("EA", "Password");
        set => WriteString("EA", "Password", value);
    }

    ///////////////////////////////////

    /// <summary>
    /// 是否使用服务端工作模式
    /// </summary>
    public static bool IsUseServer
    {
        get => ReadBoolean("Mode", "IsUseServer");
        set => WriteBoolean("Mode", "IsUseServer", value);
    }

    ///////////////////////////////////

    /// <summary>
    /// 是否使用模组运行游戏
    /// </summary>
    public static bool IsUseMod
    {
        get => ReadBoolean("Mode", "IsUseMod");
        set => WriteBoolean("Mode", "IsUseMod", value);
    }

    /// <summary>
    /// 是否启用 显示FPS
    /// </summary>
    public static bool IsDrawFps
    {
        get => ReadBoolean("Tools", "IsDrawFps");
        set => WriteBoolean("Tools", "IsDrawFps", value);
    }

    ///////////////////////////////////

    static Globals()
    {
        _iniPath = Path.Combine(CoreUtil.Dir_Config, "Config.ini");
        LoggerHelper.Info($"当前配置文件路径 {_iniPath}");

        ///////////////////////////

        // 如果此节点为空，则默认禁用
        if (IniHelper.IsKeyEmpty("Mode", "IsUseServer", _iniPath))
            IsUseServer = false;

        // 如果此节点为空，则默认启用
        if (IniHelper.IsKeyEmpty("Mode", "IsUseMod", _iniPath))
            IsUseMod = true;
    }

    /// <summary>
    /// 读取全局配置文件
    /// </summary>
    public static void Read()
    {
        LoggerHelper.Info("开始读取配置文件...");

        // 读取战地1安装路径
        var appPath = ReadString("BF1", "AppPath");
        // 验证战地1文件路径有效性
        if (CoreUtil.IsBf1MainAppFile(appPath))
        {
            SetBF1AppPath(appPath);
            LoggerHelper.Info($"已发现战地1安装路径 {BF1AppPath}");
        }
        else
        {
            SetBF1AppPath(string.Empty);
            LoggerHelper.Warn($"未发现战地1安装路径，请手动选择");
        }

        LoggerHelper.Info("读取配置文件成功");
    }

    /// <summary>
    /// 写入全局配置文件
    /// </summary>
    public static void Write()
    {
        LoggerHelper.Info("开始保存配置文件...");

        // 保存战地1安装路径
        WriteString("BF1", "AppPath", BF1AppPath);

        LoggerHelper.Info("保存配置文件成功");
    }

    /// <summary>
    /// 设置战地1主程序路径
    /// </summary>
    public static void SetBF1AppPath(string appPath)
    {
        if (string.IsNullOrWhiteSpace(appPath))
        {
            BF1AppPath = string.Empty;
            BF1InstallDir = string.Empty;
            return;
        }

        BF1AppPath = appPath;
        BF1InstallDir = Path.GetDirectoryName(appPath);
    }

    private static string ReadString(string section, string key)
    {
        return IniHelper.ReadString(section, key, _iniPath);
    }

    private static bool ReadBoolean(string section, string key)
    {
        return IniHelper.ReadBoolean(section, key, _iniPath);
    }

    private static void WriteString(string section, string key, string value)
    {
        IniHelper.WriteString(section, key, value, _iniPath);
    }

    private static void WriteBoolean(string section, string key, bool value)
    {
        IniHelper.WriteBoolean(section, key, value, _iniPath);
    }
}
