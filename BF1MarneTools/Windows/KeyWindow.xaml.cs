using CommunityToolkit.Mvvm.Input;

namespace BF1MarneTools.Windows;

/// <summary>
/// KeyWindow.xaml 的交互逻辑
/// </summary>
public partial class KeyWindow
{
    public KeyWindow()
    {
        InitializeComponent();
    }

    /// <summary>
    /// 窗口加载完成事件
    /// </summary>
    private void Window_Key_Loaded(object sender, RoutedEventArgs e)
    {
        TextBoxHint_Email.Text = Globals.Email;
        TextBoxHint_Password.Text = Globals.Password;
    }

    /// <summary>
    /// 窗口关闭时事件
    /// </summary>
    private void Window_Key_Closing(object sender, CancelEventArgs e)
    {
    }

    [RelayCommand]
    private void EnterKey()
    {
        Globals.Email = TextBoxHint_Email.Text.Trim();
        Globals.Password = TextBoxHint_Password.Text.Trim();

        this.Close();
    }

    [RelayCommand]
    private void CancelKey()
    {
        this.Close();
    }
}