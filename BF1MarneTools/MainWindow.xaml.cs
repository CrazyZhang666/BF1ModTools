using BF1MarneTools.Api;
using BF1MarneTools.Core;
using BF1MarneTools.Helper;
using BF1MarneTools.Models;
using BF1MarneTools.Utils;
using BF1MarneTools.Windows;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Hardcodet.Wpf.TaskbarNotification;
using Window = ModernWpf.Controls.Window;

namespace BF1MarneTools;

/// <summary>
/// MainWindow.xaml 的交互逻辑
/// </summary>
public partial class MainWindow
{
    public MainModel MainModel { get; set; } = new();

    /// <summary>
    /// 用于向外暴露主窗口实例
    /// </summary>
    public static Window MainWinInstance { get; private set; }

    /// <summary>
    /// 窗口关闭识别标志
    /// </summary>
    private bool _isCodeClose = false;
    /// <summary>
    /// 第一次通知标志
    /// </summary>
    private bool _isFirstNotice = false;

    public MainWindow()
    {
        InitializeComponent();
    }

    /// <summary>
    /// 窗口加载完成事件
    /// </summary>
    private void Window_Main_Loaded(object sender, RoutedEventArgs e)
    {
        LoggerHelper.Info("启动主程序成功");

        Title = $"战地1马恩工具箱 v{CoreUtil.VersionInfo} - {CoreUtil.GetWorkMode()}";

        // 向外暴露主窗口实例
        MainWinInstance = this;
        // 重置窗口关闭标志
        _isCodeClose = false;

        // 释放 dinput8.dll 文件
        FileHelper.ExtractResFile("Data.dinput8.dll", Path.Combine(Globals.BF1InstallDir, "dinput8.dll"));

        // 初始化工作
        Ready.Run();

        // 注册 RunBf1Game 启动游戏事件
        WeakReferenceMessenger.Default.Register<string, string>(this, "RunBf1Game", async (s, e) =>
        {
            await this.Dispatcher.BeginInvoke(async () =>
            {
                await Game.RunBf1Game();
            });
        });

        // 注册主程序窗口关闭事件
        WeakReferenceMessenger.Default.Register<string, string>(this, "ExitApp", (s, e) =>
        {
            ExitApp();
        });
    }

    /// <summary>
    /// 窗口渲染完成事件
    /// </summary>
    private async void Window_Main_ContentRendered(object sender, EventArgs e)
    {
        // 检查更新
        await CheckUpdate();
    }

    /// <summary>
    /// 窗口关闭事件
    /// </summary>
    private void Window_Main_Closing(object sender, CancelEventArgs e)
    {
        // 当用户从UI点击关闭时才执行
        if (!_isCodeClose)
        {
            // 取消关闭事件，隐藏主窗口
            e.Cancel = true;
            this.Hide();

            // 仅第一次通知
            if (!_isFirstNotice)
            {
                NotifyIcon_Main.ShowBalloonTip("战地1马恩工具箱 已最小化到托盘", "可通过托盘右键菜单完全退出程序", BalloonIcon.Info);
                _isFirstNotice = true;
            }

            return;
        }

        // 清理工作
        Ready.Stop();

        // 释放托盘图标
        NotifyIcon_Main.CloseBalloon();
        NotifyIcon_Main?.Dispose();
        NotifyIcon_Main = null;

        LoggerHelper.Info("关闭主程序成功");
    }

    /// <summary>
    /// 检查更新
    /// </summary>
    private async Task CheckUpdate()
    {
        LoggerHelper.Info("正在检测新版本中...");
        NotifierHelper.Notice("正在检测新版本中...");

        // 最多执行4次
        for (int i = 0; i <= 4; i++)
        {
            // 当第4次还是失败，显示提示
            if (i > 3)
            {
                IconHyperlink_Update.Text = $"点击检查新版本";
                IconHyperlink_Update.Visibility = Visibility.Visible;

                LoggerHelper.Error("检测新版本失败，请检查网络连接");
                NotifierHelper.Error("检测新版本失败，请检查网络连接");
                return;
            }

            // 第1次不提示重试
            if (i > 0)
            {
                LoggerHelper.Warn($"检测新版本失败，开始第 {i} 次重试中...");
            }

            var webVersion = await CoreApi.GetWebUpdateVersion();
            if (webVersion is not null)
            {
                if (CoreUtil.VersionInfo >= webVersion)
                {
                    LoggerHelper.Info($"恭喜，当前是最新版本 {CoreUtil.VersionInfo}");
                    NotifierHelper.Info($"恭喜，当前是最新版本 {CoreUtil.VersionInfo}");
                    return;
                }

                IconHyperlink_Update.Text = $"点击下载新版本 v{webVersion}";
                IconHyperlink_Update.Visibility = Visibility.Visible;

                LoggerHelper.Info($"发现最新版本，请前往QQ频道下载最新版本 {webVersion}");
                NotifierHelper.Warning($"发现最新版本，请前往QQ频道下载最新版本 {webVersion}");

                MainWinInstance.IsShowMaskLayer = true;
                if (MessageBox.Show($"发现新版本 v{webVersion}，是否立即前往QQ频道下载最新版本？", "发现新版本",
                    MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
                {
                    ProcessHelper.OpenLink("https://pd.qq.com/s/f5ryqcr91");
                }
                MainWinInstance.IsShowMaskLayer = false;

                return;
            }
        }
    }

    /// <summary>
    /// 设置启动秘钥
    /// </summary>
    [RelayCommand]
    private void InputToolsKey()
    {
        var keyWindow = new KeyWindow
        {
            Owner = this,
        };
        MainWinInstance.IsShowMaskLayer = true;
        keyWindow.ShowDialog();
        MainWinInstance.IsShowMaskLayer = false;
    }

    /// <summary>
    /// 设置游戏语言
    /// </summary>
    [RelayCommand]
    private void SetGameLanguage()
    {
        // 如果战地1正在运行，则终止操作
        if (ProcessHelper.IsAppRun(CoreUtil.Name_BF1))
        {
            NotifierHelper.Warning("战地1正在运行，请关闭后再执行设置语言操作");
            return;
        }

        var langWindow = new LangWindow
        {
            Owner = this,
        };
        MainWinInstance.IsShowMaskLayer = true;
        langWindow.ShowDialog();
        MainWinInstance.IsShowMaskLayer = false;
    }

    /// <summary>
    /// 打开Marne目录
    /// </summary>
    [RelayCommand]
    private void OpenMarneFolder()
    {
        var appDataDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var marneDir = Path.Combine(appDataDir, "Marne");

        ProcessHelper.OpenDirectory(marneDir);
    }

    /// <summary>
    /// 打开Crash目录
    /// </summary>
    [RelayCommand]
    private void OpenCrashFolder()
    {
        var localAppDataDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var crashDir = Path.Combine(localAppDataDir, "CrashDumps");

        ProcessHelper.OpenDirectory(crashDir);
    }

    /// <summary>
    /// 打开战地1目录
    /// </summary>
    [RelayCommand]
    private void OpenBf1Folder()
    {
        ProcessHelper.OpenDirectory(Globals.BF1InstallDir);
    }

    /// <summary>
    /// 打开配置目录
    /// </summary>
    [RelayCommand]
    private void OpenConfigFolder()
    {
        ProcessHelper.OpenDirectory(CoreUtil.Dir_Default);
    }

    /// <summary>
    /// 显示主窗口
    /// </summary>
    [RelayCommand]
    private void ShowWindow()
    {
        this.Show();

        if (this.WindowState == WindowState.Minimized)
            this.WindowState = WindowState.Normal;

        this.Activate();
        this.Focus();
    }

    /// <summary>
    /// 退出程序
    /// </summary>
    [RelayCommand]
    private void ExitApp()
    {
        // 设置关闭标志
        _isCodeClose = true;
        this.Close();
    }
}