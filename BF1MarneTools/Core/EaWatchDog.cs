using BF1MarneTools.Helper;
using BF1MarneTools.Utils;

namespace BF1MarneTools.Core;

public static class EaWatchDog
{
    private static Thread _thread = null;
    private static bool _isRunning = false;

    private const int _delayTime = 10 * 1000;

    public static void Run()
    {
        if (_thread is not null)
        {
            LoggerHelper.Warn("EA看门狗服务已经在运行，请勿重复启动");
            return;
        }

        _thread = new Thread(LoopKillEaAppThread)
        {
            Name = "LoopKillEaAppThread",
            IsBackground = true
        };

        _isRunning = true;
        _thread.Start();

        LoggerHelper.Info("启动EA看门狗服务成功");
    }

    public static void Stop()
    {
        _isRunning = false;
        _thread = null;

        LoggerHelper.Info("停止EA看门狗服务成功");
    }

    /// <summary>
    /// 定时循环结束EaApp相关进程
    /// </summary>
    private static void LoopKillEaAppThread()
    {
        while (_isRunning)
        {
            try
            {
                foreach (var process in Process.GetProcesses())
                {
                    // 结束 Origin 进程树
                    if (process.ProcessName.Equals("Origin", StringComparison.OrdinalIgnoreCase))
                    {
                        process.Kill(true);
                        LoggerHelper.Info("结束 Origin 进程树成功");
                        continue;
                    }

                    // 结束 EADesktop 进程树
                    if (process.ProcessName.Equals("EADesktop", StringComparison.OrdinalIgnoreCase))
                    {
                        if (process.MainModule.FileName != CoreUtil.File_Service_EADesktop)
                        {
                            process.Kill(true);
                            LoggerHelper.Info("结束 EADesktop 进程树成功");
                            continue;
                        }
                    }

                    // 结束 EAAntiCheat.GameServiceLauncher 进程树
                    if (process.ProcessName.Equals("EAAntiCheat.GameServiceLauncher", StringComparison.OrdinalIgnoreCase))
                    {
                        process.Kill(true);
                        LoggerHelper.Info("结束 EAAntiCheat.GameServiceLauncher 进程树成功");
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("循环结束EaApp相关进程发生异常", ex);
            }

            Thread.Sleep(_delayTime);
        }
    }
}