using CommunityToolkit.Mvvm.ComponentModel;

namespace BF1MarneTools.Models;

public partial class ModModel : ObservableObject
{
    [ObservableProperty]
    private bool isCanRunGame;
}