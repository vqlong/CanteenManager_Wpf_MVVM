using Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;
using System.IO;
using System.Text.Json;
using WpfLibrary;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using WpfLibrary.UserControls;
using System.Windows.Data;
using System.Reflection;

namespace ViewModel
{
    public class SkinViewModel : ViewModelBase
    {
        internal ObservableCollection<NamedBrush> Brushes { get; } = new ObservableCollection<NamedBrush>();
        internal ObservableCollection<FontFamily> FontFamilies { get; } = new ObservableCollection<FontFamily>(Fonts.SystemFontFamilies);
        internal ObservableCollection<Skin> Skins { get; } = new ObservableCollection<Skin>();

        public ListCollectionView SkinsView { get; }

        public ListCollectionView TextColorView { get; }
        public ListCollectionView BackgroundView { get; }
        public ListCollectionView ControlBackgroundView { get; }
        public ListCollectionView BorderColorView { get; }
        public ListCollectionView BillColorView { get; }
        public ListCollectionView SelectedTableColorView { get; }
        public ListCollectionView TableBorderColorView { get; }
        public ListCollectionView UsingTableColorView { get; }
        public ListCollectionView UsingTableNameColorView { get; }
        public ListCollectionView EmptyTableColorView { get; }
        public ListCollectionView EmptyTableNameColorView { get; }

        public ListCollectionView FontFamiliesView { get; }

        public ICommand SaveCurrentSkin { get; }
        public ICommand LoadCurrentSkin { get; }
        public ICommand SelectSkin { get; }
        public ICommand SaveNewSkin { get; }
        public Skin SelectedSkin { get => GetProperty<Skin>(); set => SetProperty(value); }
        public SkinViewModel()
        {
            LoadFonts();
            LoadBrushes();
            LoadSkins();

            SkinsView = new ListCollectionView(Skins);
            TextColorView = new ListCollectionView(Brushes);
            BackgroundView = new ListCollectionView(Brushes);
            ControlBackgroundView = new ListCollectionView(Brushes);
            BorderColorView = new ListCollectionView(Brushes);
            BillColorView = new ListCollectionView(Brushes);
            SelectedTableColorView = new ListCollectionView(Brushes);
            TableBorderColorView = new ListCollectionView(Brushes);
            UsingTableColorView = new ListCollectionView(Brushes);
            UsingTableNameColorView = new ListCollectionView(Brushes);
            EmptyTableColorView = new ListCollectionView(Brushes);
            EmptyTableNameColorView = new ListCollectionView(Brushes);
            FontFamiliesView = new ListCollectionView(FontFamilies);

            SaveCurrentSkin = new RelayCommand<string>(skinName => true, SaveCurrentSkin_Execute);
            LoadCurrentSkin = new RelayCommand<object>(obj => true, LoadCurrentSkin_Execute);
            SelectSkin = new RelayCommand<object>(obj => true, SelectSkin_Execute);
            SaveNewSkin = new RelayCommand<string>(skinName => true, SaveNewSkin_Execute);
        }
        private void LoadFonts()
        {
            ResourceDictionary dictionary = CommonResources.Default.MergedDictionaries[2];
            foreach (var key in dictionary.Keys)
            {
                FontFamilies.Add((FontFamily)dictionary[key]);
            }
        }
        private void LoadBrushes()
        {
            ResourceDictionary dictionary = CommonResources.Default.MergedDictionaries[6];
            foreach (var key in dictionary.Keys)
            {
                Brushes.Add(new NamedBrush(key.ToString(), (Brush)dictionary[key]));
            }

            var infoes = typeof(Colors).GetProperties();
            foreach (var info in infoes)
            {
                Brushes.Add(new NamedBrush(info.Name, new SolidColorBrush((Color)ColorConverter.ConvertFromString(info.Name))));
            }
        }
        private void LoadSkins()
        {
            using var stream = Application.GetResourceStream(new Uri("pack://application:,,,/ViewModel;component/Images/Classic.json", UriKind.Absolute)).Stream;
            var skin = JsonSerializer.Deserialize<Skin>(stream);
            if(skin != null) Skins.Add(skin);

            using var stream2 = Application.GetResourceStream(new Uri("pack://application:,,,/ViewModel;component/Images/Dark.json", UriKind.Absolute)).Stream;
            var skin2 = JsonSerializer.Deserialize<Skin>(stream2);
            if (skin2 != null) Skins.Add(skin2);

            using var stream3 = Application.GetResourceStream(new Uri("pack://application:,,,/ViewModel;component/Images/Default.json", UriKind.Absolute)).Stream;
            var skin3 = JsonSerializer.Deserialize<Skin>(stream3);
            if (skin3 != null) Skins.Add(skin3);

            using var stream4 = Application.GetResourceStream(new Uri("pack://application:,,,/ViewModel;component/Images/Gradient.json", UriKind.Absolute)).Stream;
            var skin4 = JsonSerializer.Deserialize<Skin>(stream4);
            if (skin4 != null) Skins.Add(skin4);

            LoadCustomSkin();
        }
        private void LoadCustomSkin()
        {
            var customSkinPath = @$"{Environment.CurrentDirectory}\Skins";
            if (Directory.Exists(customSkinPath))
            {
                var files = Directory.EnumerateFiles(customSkinPath, "*.json");
                foreach (var file in files)
                {
                    var json = File.ReadAllText(file);
                    var customSkin = JsonSerializer.Deserialize<Skin>(json);
                    if(customSkin != null && Skins.All(sk => sk.Name != customSkin.Name)) Skins.Add(customSkin);
                }
            }
        }
        private void SaveNewSkin_Execute(string skinName)
        {
            if (Skins.Any(sk => sk.Name.Equals(skinName)))
            {
                DialogBox.Show("1 Skin với tên này đã tồn tại, hãy chọn 1 tên khác.");
                return;
            }
            Skin newSkin = new
            (
                skinName,
                new Dictionary<string, string?>
                {
                    { nameof(TextColorView), TextColorView.CurrentItem?.ToString()},
                    { nameof(BackgroundView), BackgroundView.CurrentItem?.ToString()},
                    { nameof(ControlBackgroundView), ControlBackgroundView.CurrentItem?.ToString()},
                    { nameof(BorderColorView), BorderColorView.CurrentItem?.ToString()},
                    { nameof(BillColorView), BillColorView.CurrentItem ?.ToString()},
                    { nameof(SelectedTableColorView), SelectedTableColorView.CurrentItem ?.ToString()},
                    { nameof(TableBorderColorView), TableBorderColorView.CurrentItem ?.ToString()},
                    { nameof(UsingTableColorView), UsingTableColorView.CurrentItem ?.ToString()},
                    { nameof(UsingTableNameColorView), UsingTableNameColorView.CurrentItem ?.ToString()},
                    { nameof(EmptyTableColorView), EmptyTableColorView.CurrentItem ?.ToString()},
                    { nameof(EmptyTableNameColorView), EmptyTableNameColorView.CurrentItem ?.ToString()},
                    { nameof(FontFamiliesView), FontFamiliesView.CurrentItem ?.ToString()},
                }
            );
            var path = $@"{Environment.CurrentDirectory}\Skins";
            Directory.CreateDirectory(path);
            using (var stream = File.Create($@"{path}\{newSkin.Name}.json"))
            { 
                JsonSerializer.Serialize(stream, newSkin);
            } 
            //stream.Position = 0;
            LoadCustomSkin();
            DialogBox.Show($"Skin {newSkin.Name}.json đã được lưu tại {path}.", "Thông báo", DialogBoxButton.OK, DialogBoxIcon.Information);
        }
        private void SelectSkin_Execute(object? obj = null)
        {
            if (SelectedSkin is null) return;
            foreach (var key in SelectedSkin.Settings.Keys)
            {
                var info = GetType().GetProperty(key, BindingFlags.Instance | BindingFlags.Public);
                if (info == null) continue;
                var view = info.GetValue(this) as ListCollectionView;
                if (view != null)
                {
                    foreach (var item in view.SourceCollection)
                    {
                        if (item is not null && item.ToString().Equals(SelectedSkin.Settings[key]))
                        {
                            view.MoveCurrentTo(item);
                            break;
                        }                       
                    }
                } 
            }
        }
        private void SaveCurrentSkin_Execute(string skinName)
        {
            //Lưu tên skin hiện tại
            Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\Skins");
            File.WriteAllText($@"{Environment.CurrentDirectory}\Skins\CurrentSkin.txt", skinName);
        }
        private void LoadCurrentSkin_Execute(object? obj = null)
        {
            var currentSkin = @$"{Environment.CurrentDirectory}\Skins\CurrentSkin.txt";
            if (File.Exists(currentSkin))
            {
                var skinName = File.ReadAllText(currentSkin);
                foreach (var skin in Skins)
                {
                    if (skin.Name.Equals(skinName))
                    {
                        SkinsView.MoveCurrentTo(skin);
                        SelectSkin_Execute();
                        return;
                    }
                }
            }
            SkinsView.MoveCurrentToPosition(2);
            SelectSkin_Execute();
        }
    }
}
