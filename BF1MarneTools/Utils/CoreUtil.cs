using BF1MarneTools.Helper;

namespace BF1MarneTools.Utils;

public static class CoreUtil
{
    #region 配置目录
    public static string Dir_CommonAppData { get; private set; }
    public static string Dir_Default { get; private set; }

    public static string Dir_Frosty { get; private set; }
    public static string Dir_Marne { get; private set; }
    public static string Dir_Service { get; private set; }

    public static string Dir_Config { get; private set; }
    public static string Dir_Mods { get; private set; }
    public static string Dir_Log { get; private set; }

    public static string Dir_Mods_Bf1 { get; private set; }

    public static string Dir_Log_Crash { get; private set; }
    public static string Dir_Log_NLog { get; private set; }
    #endregion

    #region 数据目录
    public static string File_Frosty { get; private set; }
    public static string File_Marne { get; private set; }
    public static string File_Service { get; private set; }

    public static string File_Config_ManagerConfig { get; private set; }
    public static string File_Config_PlayerName { get; private set; }

    public static string File_Frosty_FrostyModManager { get; private set; }

    public static string File_Marne_MarneDll { get; private set; }
    public static string File_Marne_MarneLauncher { get; private set; }

    public static string File_Service_EADesktop { get; private set; }
    public static string File_Service_Notepad4 { get; private set; }
    #endregion

    //////////////////////////////////

    public const string Name_BF1 = "bf1";

    public const string Name_FrostyModManager = "FrostyModManager";
    public const string Name_MarneLauncher = "MarneLauncher";

    //////////////////////////////////

    public static readonly Version VersionInfo;

    public static readonly string UserName;             // Win10
    public static readonly string MachineName;          // CRAZYZHANG
    public static readonly string OSVersion;            // Microsoft Windows NT 10.0.19045.0
    public static readonly string SystemDirectory;      // C:\Windows\system32

    public static readonly string RuntimeVersion;       // .NET 6.0.29
    public static readonly string OSArchitecture;       // X64
    public static readonly string RuntimeIdentifier;    // win10-x64

    static CoreUtil()
    {
        #region 配置目录
        Dir_CommonAppData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
        Dir_Default = Path.Combine(Dir_CommonAppData, "BF1MarneTools");

        Dir_Frosty = Path.Combine(Dir_Default, "Frosty");
        Dir_Marne = Path.Combine(Dir_Default, "Marne");
        Dir_Service = Path.Combine(Dir_Default, "Service");

        Dir_Config = Path.Combine(Dir_Default, "Config");
        Dir_Mods = Path.Combine(Dir_Default, "Mods");
        Dir_Log = Path.Combine(Dir_Default, "Log");

        Dir_Mods_Bf1 = Path.Combine(Dir_Mods, "bf1");

        Dir_Log_Crash = Path.Combine(Dir_Log, "Crash");
        Dir_Log_NLog = Path.Combine(Dir_Log, "NLog");

        FileHelper.CreateDirectory(Dir_Frosty);
        FileHelper.CreateDirectory(Dir_Marne);
        FileHelper.CreateDirectory(Dir_Service);

        FileHelper.CreateDirectory(Dir_Config);
        FileHelper.CreateDirectory(Dir_Mods);
        FileHelper.CreateDirectory(Dir_Log);

        FileHelper.CreateDirectory(Dir_Log_Crash);
        FileHelper.CreateDirectory(Dir_Log_NLog);
        #endregion

        #region 数据目录
        File_Frosty = Path.Combine(Dir_Frosty, "Frosty.zip");
        File_Marne = Path.Combine(Dir_Marne, "Marne.zip");
        File_Service = Path.Combine(Dir_Service, "Service.zip");

        File_Config_ManagerConfig = Path.Combine(Dir_Config, "manager_config.json");
        File_Config_PlayerName = Path.Combine(Dir_Config, "PlayerName.txt");

        File_Frosty_FrostyModManager = Path.Combine(Dir_Frosty, "FrostyModManager.exe");

        File_Marne_MarneLauncher = Path.Combine(Dir_Marne, "MarneLauncher.exe");
        File_Marne_MarneDll = Path.Combine(Dir_Marne, "Marne.dll");

        File_Service_EADesktop = Path.Combine(Dir_Service, "EADesktop.exe");
        File_Service_Notepad4 = Path.Combine(Dir_Service, "Notepad4.exe");
        #endregion

        VersionInfo = Application.ResourceAssembly.GetName().Version;

        UserName = Environment.UserName;
        MachineName = Environment.MachineName;
        OSVersion = Environment.OSVersion.ToString();
        SystemDirectory = Environment.SystemDirectory;

        RuntimeVersion = RuntimeInformation.FrameworkDescription;
        OSArchitecture = RuntimeInformation.OSArchitecture.ToString();
        RuntimeIdentifier = RuntimeInformation.RuntimeIdentifier;
    }

    /// <summary>
    /// 关闭服务进程
    /// </summary>
    public static void CloseServiceProcess()
    {
        LoggerHelper.Info("准备关闭服务进程");

        ProcessHelper.CloseProcess("EADesktop");
        ProcessHelper.CloseProcess("Origin");
        ProcessHelper.CloseProcess("Notepad4");
        ProcessHelper.CloseProcess("FrostyModManager");
        ProcessHelper.CloseProcess("MarneLauncher");
        ProcessHelper.CloseProcess("EAappEmulater");

        ProcessHelper.CloseProcess("bf1");
        ProcessHelper.CloseProcess("EAAntiCheat.GameServiceLauncher");
    }

    /// <summary>
    /// 清理游戏目录第三方文件
    /// </summary>
    public static void ClearGameDirThirdFile()
    {
        FileHelper.DeleteFileAsync(Path.Combine(Globals.BF1InstallDir, "dinput8.dll"));
        FileHelper.DeleteFileAsync(Path.Combine(Globals.BF1InstallDir, "CryptBase.dll"));
    }

    /// <summary>
    /// 判断是否管理员权限运行
    /// </summary>
    public static bool IsRunAsAdmin()
    {
        var current = WindowsIdentity.GetCurrent();
        var windowsPrincipal = new WindowsPrincipal(current);
        return windowsPrincipal.IsInRole(WindowsBuiltInRole.Administrator);
    }

    /// <summary>
    /// 获取是否管理员运行字符串
    /// </summary>
    public static string GetIsAdminStr()
    {
        return IsRunAsAdmin() ? "管理员" : "非管理员";
    }

    /// <summary>
    /// 获取是否管理员运行字符串
    /// </summary>
    public static string GetWorkMode()
    {
        return Globals.IsUseServer ? "服务端模式" : "客户端模式";
    }

    /// <summary>
    /// 检测是否为战地1主程序文件
    /// </summary>
    public static bool IsBf1MainAppFile(string bf1Path)
    {
        // 判断路径是否为空
        if (string.IsNullOrWhiteSpace(bf1Path))
            return false;

        // 判断文件是否存在
        if (!File.Exists(bf1Path))
            return false;

        // 判断文件名称
        if (Path.GetFileName(bf1Path) != "bf1.exe")
            return false;

        // 判断文件大小
        var fileInfo = new FileInfo(bf1Path);
        if (fileInfo.Length != 344590632)
            return false;

        // 判断文件详细信息
        var fileVerInfo = FileVersionInfo.GetVersionInfo(bf1Path);

        if (fileVerInfo.CompanyName != "EA Digital Illusions CE AB")
            return false;
        if (fileVerInfo.FileDescription != "Battlefield™ 1")
            return false;
        if (fileVerInfo.FileVersion != "1, 0, 0, 0")
            return false;
        if (fileVerInfo.LegalCopyright != "Copyright © 2016 EA Digital Illusions CE AB. All rights reserved.")
            return false;

        return true;
    }
}