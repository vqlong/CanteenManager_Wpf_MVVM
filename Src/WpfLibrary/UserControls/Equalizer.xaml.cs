using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing.IndexedProperties;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfLibrary.UserControls
{
    /// <summary>
    /// Interaction logic for Equalizer.xaml
    /// </summary>
    public partial class Equalizer : UserControl
    {
        public Equalizer()
        {
            InitializeComponent();

            _storyboard = (Storyboard)TryFindResource("Storyboard.Equalizer4");
        }

        Storyboard _storyboard;
        public static readonly DependencyProperty StatusProperty =
                DependencyProperty.Register("Status", typeof(MediaPlayerStatus), typeof(Equalizer), new PropertyMetadata(MediaPlayerStatus.Stop, StatusChangedCallback));
        public MediaPlayerStatus Status { get => (MediaPlayerStatus)GetValue(StatusProperty); set => SetValue(StatusProperty, value); }
        private static void StatusChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var equalizer = (Equalizer)d;
            var status = (MediaPlayerStatus)e.NewValue;
            if (status == MediaPlayerStatus.Play) equalizer._storyboard.Begin();
            if (status == MediaPlayerStatus.Pause) equalizer._storyboard.Pause();
            if (status == MediaPlayerStatus.Resume) equalizer._storyboard.Resume();
            if (status == MediaPlayerStatus.Stop) equalizer._storyboard.Stop();
            if (status == MediaPlayerStatus.End) equalizer._storyboard.Stop();
        }
        public static readonly DependencyProperty ModeProperty =
            DependencyProperty.Register("Mode", typeof(int), typeof(Equalizer), new PropertyMetadata(4, ModeChangedCallback));
        public int Mode { get => (int)GetValue(ModeProperty); set => SetValue(ModeProperty, value); }
        private static void ModeChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var equalizer = (Equalizer)d;
            var currentStatus = equalizer.Status;
            equalizer.SetCurrentValue(StatusProperty, MediaPlayerStatus.Stop);

            var mode = (int)e.NewValue;
            if(mode == 0 || mode == 1 || mode == 2)
            {
                equalizer.grid012.Visibility = Visibility.Visible;
                equalizer.grid3.Visibility = Visibility.Collapsed;
                equalizer.ellipse4.Visibility = Visibility.Collapsed;
                if (mode == 0) equalizer._storyboard = (Storyboard)equalizer.TryFindResource("Storyboard.Equalizer0");
                if (mode == 1) equalizer._storyboard = (Storyboard)equalizer.TryFindResource("Storyboard.Equalizer1");
                if (mode == 2) equalizer._storyboard = (Storyboard)equalizer.TryFindResource("Storyboard.Equalizer2");
            }
            else if (mode == 3)
            {
                equalizer.grid012.Visibility = Visibility.Collapsed;
                equalizer.grid3.Visibility = Visibility.Visible;
                equalizer.ellipse4.Visibility = Visibility.Collapsed;
                equalizer._storyboard = (Storyboard)equalizer.TryFindResource("Storyboard.Equalizer3");
            }
            else if (mode == 4)
            {
                equalizer.grid012.Visibility = Visibility.Collapsed;
                equalizer.grid3.Visibility = Visibility.Collapsed;
                equalizer.ellipse4.Visibility = Visibility.Visible;
                equalizer._storyboard = (Storyboard)equalizer.TryFindResource("Storyboard.Equalizer4");
            }
            equalizer.SetCurrentValue(StatusProperty, MediaPlayerStatus.Play);
            equalizer.SetCurrentValue(StatusProperty, currentStatus);
        }
        public static readonly DependencyProperty ShowModeProperty =
            DependencyProperty.Register("ShowMode", typeof(bool), typeof(Equalizer), new PropertyMetadata(false));
        public bool ShowMode { get => (bool)GetValue(ShowModeProperty); set => SetValue(ShowModeProperty, value); }
        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if(Status == MediaPlayerStatus.Play || Status == MediaPlayerStatus.Resume || Status == MediaPlayerStatus.Pause)
            {
                var currentStatus = Status;
                SetCurrentValue(StatusProperty, MediaPlayerStatus.Stop);
                SetCurrentValue(StatusProperty, MediaPlayerStatus.Play);
                SetCurrentValue(StatusProperty, currentStatus);
            }
        }
    }
}
