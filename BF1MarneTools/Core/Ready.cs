using BF1MarneTools.Helper;
using BF1MarneTools.Utils;

namespace BF1MarneTools.Core;

public static class Ready
{
    /// <summary>
    /// 运行核心功能
    /// </summary>
    public static void Run()
    {
        try
        {
            // 打开服务进程
            LoggerHelper.Info("正在启动服务进程...");
            ProcessHelper.OpenProcess(CoreUtil.File_Service_EADesktop, true);

            LoggerHelper.Info("正在启动 LSX 监听服务...");
            LSXTcpServer.Run();

            LoggerHelper.Info("正在启动 Local HTTP 监听服务...");
            LocalHttpServer.Run();

            LoggerHelper.Info("正在启动EA看门狗服务...");
            EaWatchDog.Run();

            // 仅非服务器模式下运行
            //if (!Globals.IsUseServer)
            //{
            //    LoggerHelper.Info("正在启动战地1微服务...");
            //    MicroServer.Run();
            //}

            // 检查EA App注册表
            RegistryHelper.CheckAndAddEaAppRegistryKey();
        }
        catch (Exception ex)
        {
            LoggerHelper.Error("运行核心功能发生异常", ex);
        }
    }

    /// <summary>
    /// 停止核心
    /// </summary>
    public static void Stop()
    {
        try
        {
            LoggerHelper.Info("正在停止 Local HTTP 监听服务...");
            LocalHttpServer.Stop();

            LoggerHelper.Info("正在停止 LSX 监听服务...");
            LSXTcpServer.Stop();

            LoggerHelper.Info("正在停止EA看门狗服务...");
            EaWatchDog.Stop();

            // 仅非服务器模式下运行
            //if (!Globals.IsUseServer)
            //{
            //    LoggerHelper.Info("正在停止战地1微服务...");
            //    MicroServer.Stop();
            //}

            // 关闭服务进程
            CoreUtil.CloseServiceProcess();
            // 清理游戏目录第三方文件
            CoreUtil.ClearGameDirThirdFile();
        }
        catch (Exception ex)
        {
            LoggerHelper.Error("停止核心功能发生异常", ex);
        }
    }
}