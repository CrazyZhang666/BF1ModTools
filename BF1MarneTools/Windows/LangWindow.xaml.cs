using BF1MarneTools.Helper;
using CommunityToolkit.Mvvm.Input;
using ModernWpf.Controls;

namespace BF1MarneTools.Windows;

/// <summary>
/// LangWindow.xaml 的交互逻辑
/// </summary>
public partial class LangWindow
{
    private const string _regedit = "SOFTWARE\\EA Games\\Battlefield 1";
    private const string _regedit2 = "SOFTWARE\\WOW6432Node\\EA Games\\Battlefield 1";

    public LangWindow()
    {
        InitializeComponent();
    }

    /// <summary>
    /// 窗口加载完成事件
    /// </summary>
    private void Window_Lang_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            // 第一次查找
            var locale = RegistryHelper.GetRegistryTargetVaule(_regedit, "Locale");
            if (!string.IsNullOrWhiteSpace(locale))
            {
                SetRadioButtonIsChecked(locale);
                return;
            }
            // 第二次查找
            locale = RegistryHelper.GetRegistryTargetVaule(_regedit2, "Locale");
            if (string.IsNullOrWhiteSpace(locale))
            {
                SetRadioButtonIsChecked(locale);
                return;
            }

            LoggerHelper.Warn("未发现战地1注册表语言信息");
        }
        catch (Exception ex)
        {
            LoggerHelper.Error("读取战地1注册表语言信息时发生异常", ex);
        }
    }

    /// <summary>
    /// 窗口关闭时事件
    /// </summary>
    private void Window_Lang_Closing(object sender, CancelEventArgs e)
    {
    }

    [RelayCommand]
    private void EnterLang()
    {
        try
        {
            var locale = GetRadioButtonIsChecked();
            if (!string.IsNullOrWhiteSpace(locale))
            {
                RegistryHelper.SetRegistryTargetVaule(_regedit, "Locale", locale);
                RegistryHelper.SetRegistryTargetVaule(_regedit2, "Locale", locale);

                LoggerHelper.Info($"设置战地1注册表语言信息 {locale} 成功");
            }
            else
            {
                LoggerHelper.Warn("玩家未选择战地1语言信息，操作取消");
            }
        }
        catch (Exception ex)
        {
            LoggerHelper.Error("设置战地1注册表语言信息时发生异常", ex);
        }

        this.Close();
    }

    [RelayCommand]
    private void CancelLang()
    {
        this.Close();
    }

    private string GetRadioButtonIsChecked()
    {
        foreach (UIElement element in UniformGrid_Language.Children)
        {
            if (element is not ImageRadioButton radioButton)
                continue;

            if (radioButton.IsChecked == true)
                return radioButton.Tag.ToString();
        }

        return string.Empty;
    }

    private void SetRadioButtonIsChecked(string locale)
    {
        foreach (UIElement element in UniformGrid_Language.Children)
        {
            if (element is not ImageRadioButton radioButton)
                continue;

            if (radioButton.Tag.ToString().Equals(locale, StringComparison.OrdinalIgnoreCase))
            {
                radioButton.IsChecked = true;
                LoggerHelper.Info($"获取战地1注册表语言信息 {locale} 成功");
                return;
            }
        }
    }
}