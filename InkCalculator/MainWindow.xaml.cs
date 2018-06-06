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
        private readonly ExpressionRecognizer.Mode defaultInputMode = ExpressionRecognizer.Mode.PAUSED;
        private readonly int recognitionDelay = 500;

        private ExpressionRecognizer.Mode InputMode
        {
            get => _inputMode;
            set {
                _inputMode = value;
                if (_inputMode == ExpressionRecognizer.Mode.PAUSED)
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
                expressionRecognizer.Clear();
                expressionRecognizer.RecognitionMode = _inputMode;
            }
        }

        private ExpressionRecognizer.Mode _inputMode;

        private ExpressionRecognizer expressionRecognizer;
        private ExpressionEvaluator expressionEvaluator;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            expressionRecognizer = new ExpressionRecognizer(ExpressionsArea, recognitionDelay);
            expressionEvaluator = new ExpressionEvaluator();

            expressionEvaluator.ExpressionEvaluated += ExpressionEvaluator_ExpressionEvaluated;
            expressionRecognizer.ExpressionRecognized += expressionEvaluator.EvaluateExpression;
            InputMode = defaultInputMode;
        }

        private void ExpressionEvaluator_ExpressionEvaluated(string evaluatedExpression)
        {
            ResultsArea.Text = evaluatedExpression;
        }

        private void Clean_Click(object sender, RoutedEventArgs e)
        {
            expressionRecognizer.Clear();
        }

        private void DeleteLast_Click(object sender, RoutedEventArgs e)
        {
            expressionRecognizer.RemoveLastInput();
        }

        private void PausedMode_Click(object sender, RoutedEventArgs e)
        {
            InputMode = ExpressionRecognizer.Mode.PAUSED;
        }

        private void FluidMode_Click(object sender, RoutedEventArgs e)
        {
            InputMode = ExpressionRecognizer.Mode.FLUID;
        }

        private void ClearResultsArea()
        {
            ResultsArea.Text = "0";
        }

        private void ClearExpressionsArea()
        {
            ExpressionsArea.Strokes.Clear();
        }
    }
}
