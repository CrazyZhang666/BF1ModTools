using CommunityToolkit.Mvvm.ComponentModel;

namespace BF1MarneTools.Models;

public partial class ModInfo : ObservableObject
{
    [ObservableProperty]
    private int index;

    [ObservableProperty]
    private int order;

    [ObservableProperty]
    private string name;

    [ObservableProperty]
    private string changeTime;

    [ObservableProperty]
    private string fullName;

    [ObservableProperty]
    private string fileSize;
}