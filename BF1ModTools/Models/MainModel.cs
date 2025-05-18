using CommunityToolkit.Mvvm.ComponentModel;

namespace BF1ModTools.Models;

public partial class MainModel : ObservableObject
{
    [ObservableProperty]
    private bool isNeedUpdate;
}