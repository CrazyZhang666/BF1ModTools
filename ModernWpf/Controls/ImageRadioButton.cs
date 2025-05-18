namespace ModernWpf.Controls;

public class ImageRadioButton : RadioButton
{
    /// <summary>
    /// 图片路径
    /// </summary>
    public string Source
    {
        get { return (string)GetValue(SourceProperty); }
        set { SetValue(SourceProperty, value); }
    }
    public static readonly DependencyProperty SourceProperty =
        DependencyProperty.Register("Source", typeof(string), typeof(ImageRadioButton), new PropertyMetadata(default));
}
