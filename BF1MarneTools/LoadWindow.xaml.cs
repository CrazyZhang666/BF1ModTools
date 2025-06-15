using BF1MarneTools.Helper;
using BF1MarneTools.Models;
using BF1MarneTools.Utils;
using CommunityToolkit.Mvvm.Input;

namespace BF1MarneTools;

/// <summary>
/// LoadWindow.xaml 的交互逻辑
/// </summary>
public partial class LoadWindow
{
    public LoadModel LoadModel { get; set; } = new();

    public LoadWindow()
    {
        InitializeComponent();
    }

    /// <summary>
    /// 窗口加载完成事件
    /// </summary>
    private async void Window_Load_Loaded(object sender, RoutedEventArgs e)
    {
        Title = $"战地1马恩工具箱 v{CoreUtil.VersionInfo}";

        DisplayLoadState("等待玩家操作中...");

        // 读取全局配置文件
        Globals.Read();

        // 战地1路径
        LoadModel.Bf1Path = Globals.BF1AppPath;
        // 默认初始化失败
        LoadModel.IsInitSuccess = false;

        /////////////////////////////////////////////////

        await Task.Run(() =>
        {
            try
            {
                // 关闭服务残留进程
                DisplayLoadState("开始关闭服务残留进程...");
                CoreUtil.CloseServiceProcess();
                DisplayLoadState("关闭服务残留进程成功");

                //////////////////////////////////////////

                // 清理数据文件
                DisplayLoadState("正在清理缓存文件...");
                FileHelper.ClearDirectory(CoreUtil.Dir_Frosty);
                FileHelper.ClearDirectory(CoreUtil.Dir_Marne);
                FileHelper.ClearDirectory(CoreUtil.Dir_Service);
                DisplayLoadState("清理缓存文件成功");

                DisplayLoadState("正在解压资源文件1/3...");
                FileHelper.ExtractResFile("Data.Frosty.zip", CoreUtil.File_Frosty);
                ZipFile.ExtractToDirectory(CoreUtil.File_Frosty, CoreUtil.Dir_Frosty, true);
                File.Delete(CoreUtil.File_Frosty);

                DisplayLoadState("正在解压资源文件2/3...");
                FileHelper.ExtractResFile("Data.Marne.zip", CoreUtil.File_Marne);
                ZipFile.ExtractToDirectory(CoreUtil.File_Marne, CoreUtil.Dir_Marne, true);
                File.Delete(CoreUtil.File_Marne);

                DisplayLoadState("正在解压资源文件3/3...");
                FileHelper.ExtractResFile("Data.Service.zip", CoreUtil.File_Service);
                ZipFile.ExtractToDirectory(CoreUtil.File_Service, CoreUtil.Dir_Service, true);
                File.Delete(CoreUtil.File_Service);

                //////////////////////////////////////////

                LoadModel.IsInitSuccess = true;
                DisplayLoadState("初始化游戏信息成功");
            }
            catch (Exception ex)
            {
                DisplayLoadState($"初始化过程中发送异常  {ex.Message}");
            }
        });
    }

    /// <summary>
    /// 窗口渲染完成事件
    /// </summary>
    private void Window_Load_ContentRendered(object sender, EventArgs e)
    {
    }

    /// <summary>
    /// 窗口关闭时事件
    /// </summary>
    private void Window_Load_Closing(object sender, CancelEventArgs e)
    {
    }

    /// <summary>
    /// 显示加载状态到UI界面
    /// </summary>
    private void DisplayLoadState(string log)
    {
        this.Dispatcher.Invoke(() =>
        {
            TextBox_Logger.AppendText($"[{DateTime.Now:HH:mm:ss}]  {log}{Environment.NewLine}");
            TextBox_Logger.ScrollToEnd();
        });
    }

    /// <summary>
    /// 保存数据
    /// </summary>
    private void SaveData()
    {
        Globals.Write();
    }

    /// <summary>
    /// 打开配置文件
    /// </summary>
    [RelayCommand]
    private void OpenConfigFolder()
    {
        ProcessHelper.OpenDirectory(CoreUtil.Dir_Default);
    }

    /// <summary>
    /// 启动主程序
    /// </summary>
    [RelayCommand]
    private void LaunchMainApp()
    {
        // 保存数据
        SaveData();

        ////////////////////////////////

        var mainWindow = new MainWindow();

        // 转移主程序控制权
        Application.Current.MainWindow = mainWindow;
        // 关闭当前窗口
        this.Close();

        // 显示主程序窗口
        mainWindow.Show();
    }

    /// <summary>
    /// 选择战地1文件路径
    /// </summary>
    [RelayCommand]
    private void SelcetBf1Path()
    {
        try
        {
            // 战地1路径无效，重新选择
            var dialog = new OpenFileDialog
            {
                Title = "请选择战地1游戏主程序 bf1.exe 文件路径",
                FileName = "bf1.exe",
                DefaultExt = ".exe",
                Filter = "可执行文件 (.exe)|*.exe",
                Multiselect = false,
                RestoreDirectory = true,
                AddExtension = true,
                CheckFileExists = true,
                CheckPathExists = true
            };

            // 当文件夹路径存在时才会赋值
            if (Directory.Exists(Globals.GameSelectDir))
                dialog.InitialDirectory = Globals.GameSelectDir;

            // 如果未选择，则退出程序
            if (dialog.ShowDialog() == false)
                return;

            var dirPath = Path.GetDirectoryName(dialog.FileName);

            // 记住本次选择的文件路径
            Globals.GameSelectDir = dirPath;

            // 开始校验文件有效性
            if (!CoreUtil.IsBf1MainAppFile(dialog.FileName))
            {
                DisplayLoadState("不支持的战地1游戏主程序版本，请选择正确的版本");
                return;
            }

            // 检查战地1所在目录磁盘格式
            var diskFlag = Path.GetPathRoot(dirPath);
            var driveInfo = new DriveInfo(diskFlag);
            if (!driveInfo.DriveFormat.Equals("NTFS", StringComparison.OrdinalIgnoreCase))
            {
                DisplayLoadState("检测到战地1所在磁盘格式不是NTFS，请转换磁盘格式");
                return;
            }

            Globals.SetBF1AppPath(dialog.FileName);
            LoadModel.Bf1Path = dialog.FileName;

            DisplayLoadState("获取战地1游戏主程序路径成功");
            return;
        }
        catch (Exception ex)
        {
            LoggerHelper.Error("选择战地1安装路径发生异常", ex);
            DisplayLoadState($"选择战地1安装路径发生异常 {ex.Message}");
            return;
        }
    }
}