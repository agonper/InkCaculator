using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InkCalculator
{
    class ExpressionEvaluator
    {
        public event Action<String> ExpressionEvaluated;

        public void EvaluateExpression(string expression)
        {
            if (IsInt(expression)) {
                NotifyExpressionEvaluated(expression);
                return;
            }
            if (EvaluateMathString(expression, out double result))
            {
                NotifyExpressionEvaluated(expression + " = " + String.Format("{0:0.####}", result));
                return;
            }
            NotifyExpressionEvaluated(expression + " = [?]");
        }

        private bool IsInt(string expression)
        {
            return int.TryParse(expression, out int intParseResult);
        }

        private bool EvaluateMathString(string expression, out double result)
        {
            try
            {
                result = Convert.ToDouble(new DataTable().Compute(expression, null));
                return true;
            }
            catch (Exception)
            {
                result = 0;
                return false;
            }
        }

        private void NotifyExpressionEvaluated(string result)
        {
            ExpressionEvaluated?.Invoke(result);
        }
    }
}
