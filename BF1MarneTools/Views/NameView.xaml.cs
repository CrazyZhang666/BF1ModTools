using BF1MarneTools.Helper;
using BF1MarneTools.Utils;
using CommunityToolkit.Mvvm.Input;

namespace BF1MarneTools.Views;

/// <summary>
/// NameView.xaml 的交互逻辑
/// </summary>
public partial class NameView : UserControl
{
    /// <summary>
    /// UTF8编码无BOM
    /// </summary>
    private readonly UTF8Encoding UTF8EncodingNoBom = new(false);

    private const string DefaultName = "战地风云1";

    public NameView()
    {
        InitializeComponent();

        ToDoList();
    }

    private void ToDoList()
    {
        // 使用默认玩家名称
        TextBox_CustomName.Text = DefaultName;

        // 如果玩家名称文件不存在，则跳过
        if (!File.Exists(CoreUtil.File_Config_PlayerName))
            return;

        try
        {
            // 读取本地保存玩家名称
            var fileContent = File.ReadAllText(CoreUtil.File_Config_PlayerName, UTF8EncodingNoBom).Trim();
            // 如果玩家名称内存不为空
            if (!string.IsNullOrWhiteSpace(fileContent))
            {
                TextBox_CustomName.Text = fileContent;
                return;
            }

            // 判断名称是否超过限制
            var nameHexBytes = Encoding.UTF8.GetBytes(DefaultName);
            if (nameHexBytes.Length <= 15)
            {
                // 写入玩家名称
                File.WriteAllText(CoreUtil.File_Config_PlayerName, DefaultName, UTF8EncodingNoBom);
            }
        }
        catch (Exception ex)
        {
            LoggerHelper.Error("初始化中文游戏ID发生异常", ex);
            NotifierHelper.Error("初始化中文游戏ID发生异常");
        }
    }

    #region 自定义区域
    [RelayCommand]
    private void ChangePlayerName()
    {
        if (ProcessHelper.IsAppRun(CoreUtil.Name_BF1))
        {
            NotifierHelper.Warning("战地1正在运行，请关闭后再执行修改ID操作");
            return;
        }

        var playerName = TextBox_CustomName.Text.Trim();
        if (string.IsNullOrWhiteSpace(playerName))
        {
            NotifierHelper.Warning("游戏ID不能为空，请重新修改");
            return;
        }

        var nameHexBytes = Encoding.UTF8.GetBytes(playerName);
        if (nameHexBytes.Length > 15)
        {
            NotifierHelper.Warning("游戏ID字节数不能超过15字节，请重新修改");
            return;
        }

        try
        {
            // 写入玩家名称
            File.WriteAllText(CoreUtil.File_Config_PlayerName, playerName, UTF8EncodingNoBom);
            NotifierHelper.Success("修改中文游戏ID成功，请启动战地1查看");
        }
        catch (Exception ex)
        {
            LoggerHelper.Error("修改中文游戏ID发生异常", ex);
            NotifierHelper.Error("修改中文游戏ID发生异常");
        }
    }
    #endregion
}