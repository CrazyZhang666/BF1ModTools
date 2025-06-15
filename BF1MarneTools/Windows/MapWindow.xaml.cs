using BF1MarneTools.Models;
using BF1MarneTools.Utils;
using CommunityToolkit.Mvvm.Input;

namespace BF1MarneTools.Windows;

/// <summary>
/// MapWindow.xaml 的交互逻辑
/// </summary>
public partial class MapWindow
{
    public ObservableCollection<ModeInfo> GameModeInfos { get; set; } = [];
    public ObservableCollection<Map2Info> GameMap2Infos { get; set; } = [];
    public ObservableCollection<MapModel> GameMapModels { get; set; } = [];

    private static readonly string _mapListPath = Path.Combine(Globals.BF1InstallDir, "Instance\\MapList.txt");

    public MapWindow()
    {
        InitializeComponent();
    }

    /// <summary>
    /// 窗口加载完成事件
    /// </summary>
    private void Window_Map_Loaded(object sender, RoutedEventArgs e)
    {
        foreach (var item in MapUtil.GameModeInfoDb)
        {
            GameModeInfos.Add(item);
        }
        ListBox_GameModeInfos.SelectedIndex = 0;

        GameMapModels.CollectionChanged += (s, e) => { ReSortMapIndex(); };

        if (File.Exists(_mapListPath))
        {
            var allLines = File.ReadAllLines(_mapListPath);

            int index = 0;
            foreach (var line in allLines)
            {
                var array = line.Split(';');
                if (array.Length != 3)
                    continue;

                var mapInfo = MapUtil.GameMapInfoDb.Find(x => x.Url == array[0]);
                if (mapInfo == null)
                    continue;

                var modeInfo = MapUtil.GameModeInfoDb.Find(x => x.Code == array[1]);
                if (modeInfo == null)
                    continue;

                GameMapModels.Add(new MapModel
                {
                    Index = ++index,
                    Dlc = mapInfo.DLC,
                    Image = MapUtil.GetMapImageByName(mapInfo.Name3),
                    Name = mapInfo.Name,
                    Name2 = mapInfo.Name2,
                    Name3 = mapInfo.Name3,
                    Url = mapInfo.Url,
                    Mode = modeInfo.Name,
                    Mode2 = modeInfo.Name2,
                    Code = modeInfo.Code
                });
            }
        }
    }

    /// <summary>
    /// 窗口关闭时事件
    /// </summary>
    private void Window_Map_Closing(object sender, CancelEventArgs e)
    {
        SaveData();
    }

    /// <summary>
    /// 地图索引重新排序
    /// </summary>
    private void ReSortMapIndex()
    {
        int index = 0;
        foreach (var item in GameMapModels)
        {
            item.Index = ++index;
        }
    }

    private void ListBox_GameModeInfos_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ListBox_GameModeInfos.SelectedItem is null)
            return;

        GameMap2Infos.Clear();

        var modeInfo = ListBox_GameModeInfos.SelectedItem as ModeInfo;

        var index = 0;
        var result = MapUtil.GameMapInfoDb.Where(x => x.Modes.Contains(modeInfo.Code));
        foreach (var item in result)
        {
            GameMap2Infos.Add(new()
            {
                Index = ++index,
                DLC = item.DLC,
                Image = MapUtil.GetMapImageByName(item.Name3),
                Name = item.Name,
                Name2 = item.Name2,
                Name3 = item.Name3,
                Url = item.Url
            });
        }
        ListBox_GameMap2Infos.SelectedIndex = 0;
    }

    [RelayCommand]
    private void AddMap()
    {
        if (ListBox_GameModeInfos.SelectedItem is null)
            return;
        if (ListBox_GameMap2Infos.SelectedItem is null)
            return;

        var modeInfo = ListBox_GameModeInfos.SelectedItem as ModeInfo;
        var map2Info = ListBox_GameMap2Infos.SelectedItem as Map2Info;

        GameMapModels.Add(new()
        {
            Dlc = map2Info.DLC,
            Image = MapUtil.GetMapImageByName(map2Info.Name3),
            Name = map2Info.Name,
            Name2 = map2Info.Name2,
            Name3 = map2Info.Name3,
            Url = map2Info.Url,
            Mode = modeInfo.Name,
            Mode2 = modeInfo.Name2,
            Code = modeInfo.Code,
        });
    }

    [RelayCommand]
    private void DelectMap()
    {
        if (ListBox_GameMapModels.SelectedItem is null)
            return;

        var mapModel = ListBox_GameMapModels.SelectedItem as MapModel;
        GameMapModels.Remove(mapModel);
    }

    [RelayCommand]
    private void ClearMapList()
    {
        GameMapModels.Clear();
    }

    [RelayCommand]
    private void SaveData()
    {
        var strBuilder = new StringBuilder();

        // 当选择地图不为空时，填充新的地图数据
        if (GameMapModels.Count != 0)
        {
            var lastItem = GameMapModels.Last();
            foreach (var item in GameMapModels)
            {
                strBuilder.Append($"{item.Url};{item.Code};1");

                if (item != lastItem)
                    strBuilder.Append(Environment.NewLine);
            }
        }

        // 检查路径文件夹是否存在，不存在则自动创建
        var dirPath = Path.GetDirectoryName(_mapListPath);
        if (!Directory.Exists(dirPath))
            Directory.CreateDirectory(dirPath);

        File.WriteAllText(_mapListPath, strBuilder.ToString());
    }
}