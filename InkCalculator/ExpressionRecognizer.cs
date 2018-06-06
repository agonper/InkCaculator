using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Threading;

namespace InkCalculator
{
    class ExpressionRecognizer
    {
        private static readonly Dictionary<String, String> validNonNumericElements = new Dictionary<String, String>()
        {
            { "+", "+" },
            { "-", "-" },
            { "_", "-" },
            { "", "-" },
            { "/", "/" },
            { "*", "*" },
            { "x", "*" },
            { "X", "*" },
            { ".", "." }
        };

        public string RecognizedExpression
        {
            get => _recognizedExpression;
            set
            {
                _recognizedExpression = value;
                ExpressionRecognized?.Invoke(_recognizedExpression);
            }
        }
        public Mode RecognitionMode { get; set; }

        public event Action<string> ExpressionRecognized;

        private InkCanvas expressionArea;
        private InkAnalyzer analyzer;
        private DispatcherTimer timer;

        private string _recognizedExpression;
        private List<String> pausedRecognitionResults = new List<String>();

        public ExpressionRecognizer(InkCanvas expressionArea, int recognitionDelay)
        {
            this.expressionArea = expressionArea;

            analyzer = new InkAnalyzer()
            {
                AnalysisModes = AnalysisModes.AutomaticReconciliationEnabled
            };

            timer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromMilliseconds(recognitionDelay)
            };

            timer.Tick += Timer_Tick;

            expressionArea.StrokeCollected += ExpressionArea_StrokeCollected;
            expressionArea.StrokeErasing += ExpressionArea_StrokeErasing;
        }

        public void Clear()
        {
            ClearExpressionInputArea();
            pausedRecognitionResults.Clear();
            RecognizedExpression = "0";
        }

        public void RemoveLastInput()
        {
            if (expressionArea.Strokes.Count > 0)
            {
                ClearExpressionInputArea();
                return;
            }
            RemoveLastPausedResultsEntry();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            analyzer.Analyze();
            ProcessRecognizedInput(analyzer.GetRecognizedString());
        }

        private void ExpressionArea_StrokeCollected(object sender, InkCanvasStrokeCollectedEventArgs e)
        {
            timer.Stop();
            analyzer.AddStroke(e.Stroke);
            timer.Start();
        }

        private void ExpressionArea_StrokeErasing(object sender, InkCanvasStrokeErasingEventArgs e)
        {
            analyzer.RemoveStroke(e.Stroke);
        }

        private void ProcessRecognizedInput(string input)
        {
            switch (RecognitionMode)
            {
                case Mode.PAUSED:
                    RecognizeInPausedMode(input);
                    break;
                case Mode.FLUID:
                    RecognizeInFluidMode(input);
                    break;
                default:
                    throw new Exception("Please specify recognition mode first");
            }
        }

        private void RecognizeInPausedMode(string input)
        {
            timer.Stop();
            bool isInt = IsInt(input);
            bool isValidChar = IsValidNonNumericalChar(input);
            if (!isInt && !isValidChar)
            {
                Console.WriteLine(String.Format("Reconocido: \"{0}\"", input));
                return;
            }
            AddToPausedResults(isInt ? input : validNonNumericElements[input]);
            ClearExpressionInputArea();
        }

        private void RecognizeInFluidMode(string input)
        {
            if (input == "") return;
            string sanitizedInput = input.Replace("Other", "F");
            string[] inputParts = sanitizedInput.ToCharArray().Select(c => c.ToString()).ToArray();


            for (int i = 0; i < inputParts.Length; i++)
            {
                string character = inputParts[i];
                bool isInt = IsInt(character);
                bool isValidChar = IsValidNonNumericalChar(character);

                if (!isInt && !isValidChar)
                {
                    inputParts[i] = "[?]";
                }
                if (isValidChar)
                {
                    inputParts[i] = validNonNumericElements[character];
                }
            }
            RecognizedExpression = String.Join("", inputParts);
        }

        private void AddToPausedResults(string recognizedElement)
        {
            pausedRecognitionResults.Add(recognizedElement);
            SyncPausedResults();
        }

        private void RemoveLastPausedResultsEntry()
        {
            pausedRecognitionResults.RemoveAt(pausedRecognitionResults.Count - 1);
            SyncPausedResults();
        }

        public void ClearExpressionInputArea()
        {
            analyzer.RemoveStrokes(expressionArea.Strokes);
            expressionArea.Strokes.Clear();
        }

        private void SyncPausedResults()
        {
            RecognizedExpression = String.Join("", pausedRecognitionResults.ToArray());
        }

        private bool IsInt(string character)
        {
            return int.TryParse(character, out int parseIntResult);
        }

        private bool IsValidNonNumericalChar(string character)
        {
            return validNonNumericElements.Keys.Contains(character);
        }

        public enum Mode
        {
            PAUSED, FLUID
        }
    }
}
