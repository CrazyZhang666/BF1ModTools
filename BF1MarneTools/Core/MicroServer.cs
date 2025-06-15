using BF1MarneTools.Helper;
using BF1MarneTools.Native;
using BF1MarneTools.Utils;

namespace BF1MarneTools.Core;

public static class MicroServer
{
    private static Timer _timer = null;

    private const byte _mSize = 10;
    /// <summary>
    /// 清理内存阈值
    /// </summary>
    private const long _targetMemorySize = _mSize * 1024 * 1024 * 1024L;

    public static void Run()
    {
        if (_timer is not null)
        {
            LoggerHelper.Warn("战地1微服务已经在运行，请勿重复启动");
            return;
        }

        _timer = new Timer
        {
            Interval = 60 * 1000,
            AutoReset = true
        };
        _timer.Elapsed += OnTimedEvent;
        _timer.Start();

        LoggerHelper.Info("启动战地1微服务成功");
    }

    public static void Stop()
    {
        _timer?.Stop();
        _timer?.Dispose();
        _timer = null;

        LoggerHelper.Info("停止战地1微服务成功");
    }

    private static void OnTimedEvent(object source, ElapsedEventArgs e)
    {
        try
        {
            foreach (var process in Process.GetProcessesByName(CoreUtil.Name_BF1))
            {
                LoggerHelper.Info($"[微服务] 目标进程 {CoreUtil.Name_BF1}.exe 内存占用 {process.WorkingSet64 / (1024 * 1024)} MB");

                if (process.WorkingSet64 < _targetMemorySize)
                {
                    LoggerHelper.Info($"[微服务] 目标进程未达到清理阈值 {_mSize}GB，跳过清理");
                    continue;
                }

                _ = Win32.EmptyWorkingSet(process.Handle);
                LoggerHelper.Info($"[微服务] 目标进程达到清理阈值 {_mSize}GB，已执行清理");
            }
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"[微服务] 清理目标进程 {CoreUtil.Name_BF1}.exe 内存发生异常", ex);
        }
    }
}