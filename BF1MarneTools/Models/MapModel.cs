using CommunityToolkit.Mvvm.ComponentModel;

namespace BF1MarneTools.Models;

public partial class MapModel : ObservableObject
{
    [ObservableProperty]
    private int index;

    [ObservableProperty]
    private string dlc;

    [ObservableProperty]
    private string image;

    [ObservableProperty]
    private string name;

    [ObservableProperty]
    private string name2;

    [ObservableProperty]
    private string name3;

    [ObservableProperty]
    private string url;

    [ObservableProperty]
    private string mode;

    [ObservableProperty]
    private string mode2;

    [ObservableProperty]
    private string code;
}