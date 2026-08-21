namespace SheepMonitor.Core.Services;

/// <summary>
/// اجرای فرمول‌های کنترل‌شده جیره با مجموعه محدودی از عملگرها.
/// </summary>
public sealed class RationFormulaEvaluator : IRationFormulaEvaluator
{
    public decimal Evaluate(string? formula, decimal weightKg, decimal basePercent, decimal weightCoefficient)
    {
        if (string.IsNullOrWhiteSpace(formula))
            return weightKg * (basePercent / 100m);

        var expression = formula.Trim()
            .Replace("وزن", weightKg.ToString(System.Globalization.CultureInfo.InvariantCulture), StringComparison.OrdinalIgnoreCase)
            .Replace("درصدپایه", basePercent.ToString(System.Globalization.CultureInfo.InvariantCulture), StringComparison.OrdinalIgnoreCase)
            .Replace("ضریب", weightCoefficient.ToString(System.Globalization.CultureInfo.InvariantCulture), StringComparison.OrdinalIgnoreCase);

        return EvaluateArithmetic(expression);
    }

    private static decimal EvaluateArithmetic(string expression)
    {
        var values = new Stack<decimal>();
        var operators = new Stack<char>();
        for (var i = 0; i < expression.Length; i++)
        {
            if (char.IsWhiteSpace(expression[i])) continue;
            if (char.IsDigit(expression[i]) || expression[i] == '.')
            {
                var start = i;
                while (i + 1 < expression.Length && (char.IsDigit(expression[i + 1]) || expression[i + 1] == '.')) i++;
                values.Push(decimal.Parse(expression[start..(i + 1)], System.Globalization.CultureInfo.InvariantCulture));
                continue;
            }
            if (expression[i] == '(') { operators.Push(expression[i]); continue; }
            if (expression[i] == ')')
            {
                while (operators.Count > 0 && operators.Peek() != '(') Apply(values, operators.Pop());
                if (operators.Count == 0) throw new FormatException("پرانتزهای فرمول نامعتبر است.");
                operators.Pop();
                continue;
            }
            if (!"+-*/".Contains(expression[i])) throw new FormatException("فرمول شامل عملگر نامعتبر است.");
            while (operators.Count > 0 && operators.Peek() != '(' && Priority(operators.Peek()) >= Priority(expression[i])) Apply(values, operators.Pop());
            operators.Push(expression[i]);
        }
        while (operators.Count > 0) Apply(values, operators.Pop());
        if (values.Count != 1) throw new FormatException("فرمول جیره نامعتبر است.");
        return values.Pop();
    }

    private static int Priority(char op) => op is '*' or '/' ? 2 : 1;

    private static void Apply(Stack<decimal> values, char op)
    {
        if (values.Count < 2) throw new FormatException("فرمول جیره ناقص است.");
        var right = values.Pop();
        var left = values.Pop();
        values.Push(op switch
        {
            '+' => left + right,
            '-' => left - right,
            '*' => left * right,
            '/' when right != 0 => left / right,
            '/' => throw new DivideByZeroException("تقسیم بر صفر در فرمول جیره مجاز نیست."),
            _ => throw new FormatException("عملگر نامعتبر است.")
        });
    }
}
