using System.ComponentModel.DataAnnotations;

namespace Soul.Shop.Infrastructure;

public class ValidationException(Type target, IEnumerable<ValidationResult> validationResults) : Exception
{
    private IEnumerable<ValidationResult> ValidationResults { get; } = validationResults;

    private Type TargetType { get; } = target;

    public override string Message
    {
        get
        {
            return string.Concat(TargetType.ToString(), ": ",
                string.Join(";", ValidationResults.Select(x => $"{x.ErrorMessage}")));
        }
    }
}