using CommunityToolkit.Mvvm.ComponentModel;

namespace BF1ModTools.Models;

public partial class ModModel : ObservableObject
{
    [ObservableProperty]
    private bool isCanRunGame;
}