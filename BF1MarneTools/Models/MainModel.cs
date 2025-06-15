using CommunityToolkit.Mvvm.ComponentModel;

namespace BF1MarneTools.Models;

public partial class MainModel : ObservableObject
{
    [ObservableProperty]
    private bool isNeedUpdate;
}