namespace BF1MarneTools.Helper;

public static class RegistryHelper
{
    /// <summary>
    /// 读取注册表
    /// </summary>
    public static string GetRegistryTargetVaule(string regPath, string keyName)
    {
        try
        {
            var localMachine = Registry.LocalMachine;

            using var regKey = localMachine.OpenSubKey(regPath);
            if (regKey is null)
                return string.Empty;

            return regKey.GetValue(keyName).ToString();
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"读取注册表异常 {regPath} {keyName}", ex);
            return string.Empty;
        }
    }

    /// <summary>
    /// 写入注册表
    /// </summary>
    public static void SetRegistryTargetVaule(string regPath, string keyName, string value)
    {
        try
        {
            var localMachine = Registry.LocalMachine;

            // 创建注册表，如果已经存在则不影响
            using var regKey = localMachine.CreateSubKey(regPath, true);
            if (regKey is null)
                return;

            regKey.SetValue(keyName, value);
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"写入注册表异常 {regPath} {keyName} {value}", ex);
        }
    }

    /// <summary>
    /// 获取Origin/EA App注册表情况，如果不存在就写入一个，这样做可以在不安装平台的情况启动游戏
    /// </summary>
    public static void CheckAndAddEaAppRegistryKey()
    {
        try
        {
            using RegistryKey localMachine = Registry.LocalMachine;
            using RegistryKey openSubKey = localMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Origin", true);
            if (openSubKey is not null)
                return;

            using RegistryKey createSubKey = localMachine.CreateSubKey(@"SOFTWARE\Wow6432Node\Origin");
            createSubKey?.SetValue("ClientPath", @"C:\Program Files\Electronic Arts\EA Desktop\EA Desktop\EADesktop.exe", RegistryValueKind.String);
        }
        catch (Exception ex)
        {
            LoggerHelper.Error("写入 EADesktop 安装路径注册表异常", ex);
        }
    }
}