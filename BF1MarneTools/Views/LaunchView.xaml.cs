using BF1MarneTools.Core;
using BF1MarneTools.Helper;
using BF1MarneTools.Utils;
using BF1MarneTools.Windows;
using CommunityToolkit.Mvvm.Input;

namespace BF1MarneTools.Views;

/// <summary>
/// LaunchView.xaml 的交互逻辑
/// </summary>
public partial class LaunchView : UserControl
{
    public LaunchView()
    {
        InitializeComponent();

        ToDoList();
    }

    private void ToDoList()
    {
    }

    #region Frosty Mod Manager
    /// <summary>
    /// 运行寒霜Mod管理器
    /// </summary>
    [RelayCommand]
    private async Task RunFrostyModManager()
    {
        // 如果战地1正在运行，则终止操作
        if (ProcessHelper.IsAppRun(CoreUtil.Name_BF1))
        {
            NotifierHelper.Warning("战地1正在运行，请关闭后再启动本程序");
            return;
        }

        // 如果FrostyModManager已经在运行，则终止操作
        if (ProcessHelper.IsAppRun(CoreUtil.Name_FrostyModManager))
        {
            NotifierHelper.Warning("程序已经运行了，请不要重复运行");
            return;
        }

        // 如果不使用Mod文件，则直接启动战地1
        if (!Globals.IsUseMod)
        {
            await Game.RunBf1Game();
            return;
        }

        // 寒霜Mod文件选择窗口
        var modWindow = new ModWindow
        {
            Owner = MainWindow.MainWinInstance
        };
        MainWindow.MainWinInstance.IsShowMaskLayer = true;
        modWindow.ShowDialog();
        MainWindow.MainWinInstance.IsShowMaskLayer = false;
    }

    /// <summary>
    /// 关闭寒霜Mod管理器
    /// </summary>
    [RelayCommand]
    private void CloseFrostyModManager()
    {
        ProcessHelper.CloseProcess(CoreUtil.Name_FrostyModManager);
    }
    #endregion

    #region BF1 Marne Launcher
    /// <summary>
    /// 运行马恩启动器
    /// </summary>
    [RelayCommand]
    private void RunMarneLauncher()
    {
        // 如果战地1未运行，则终止操作
        if (!ProcessHelper.IsAppRun(CoreUtil.Name_BF1))
        {
            NotifierHelper.Warning("战地1未运行，请先启动战地1游戏后再执行本操作");
            return;
        }

        // 如果MarneLauncher已经在运行，则终止操作
        if (ProcessHelper.IsAppRun(CoreUtil.Name_MarneLauncher))
        {
            NotifierHelper.Warning("程序已经运行了，请不要重复运行");
            return;
        }

        ProcessHelper.OpenProcess(CoreUtil.File_Marne_MarneLauncher);
    }

    /// <summary>
    /// 关闭马恩启动器
    /// </summary>
    [RelayCommand]
    private void CloseMarneLauncher()
    {
        ProcessHelper.CloseProcess(CoreUtil.Name_MarneLauncher);
    }
    #endregion
}