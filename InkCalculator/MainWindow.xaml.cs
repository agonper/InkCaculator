using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace InkCalculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Mode InputMode
        {
            get => _inputMode;
            private set {
                _inputMode = value;
                if (_inputMode == Mode.PAUSED)
                {
                    PausedModeBtn.IsEnabled = false;
                    FluidModeBtn.IsEnabled = true;

                    PausedModeCtrl.Visibility = Visibility.Visible;
                    FluidModeCtrl.Visibility = Visibility.Collapsed;
                } else
                {
                    PausedModeBtn.IsEnabled = true;
                    FluidModeBtn.IsEnabled = false;

                    PausedModeCtrl.Visibility = Visibility.Collapsed;
                    FluidModeCtrl.Visibility = Visibility.Visible;
                }
            }
        }

        private Mode _inputMode;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Clean_Click(object sender, RoutedEventArgs e)
        {
            ClearResultsArea();
            ClearExpressionsArea();
        }

        private void DeleteLast_Click(object sender, RoutedEventArgs e)
        {

        }

        private void PausedMode_Click(object sender, RoutedEventArgs e)
        {
            InputMode = Mode.PAUSED;
        }

        private void FluidMode_Click(object sender, RoutedEventArgs e)
        {
            InputMode = Mode.FLUID;
        }

        private void ClearResultsArea()
        {
            ResultsArea.Text = "0";
        }

        private void ClearExpressionsArea()
        {
            ExpressionsArea.Strokes.Clear();
        }

        public enum Mode
        {
            PAUSED, FLUID
        }
    }
}
