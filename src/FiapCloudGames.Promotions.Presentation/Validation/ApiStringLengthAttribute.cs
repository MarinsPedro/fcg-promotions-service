using System.ComponentModel.DataAnnotations;
using FiapCloudGames.Promotions.Application.Validation;

namespace FiapCloudGames.Promotions.Presentation.Validation;

[AttributeUsage(
    AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
    AllowMultiple = false)]
public sealed class ApiStringLengthAttribute(int maximumLength)
    : StringLengthAttribute(maximumLength)
{
    public override string FormatErrorMessage(string name)
    {
        var field = InputFieldName.From(name);
        return MinimumLength > 0
            ? $"{field} deve possuir entre {MinimumLength} e {MaximumLength} caracteres."
            : $"{field} deve possuir no máximo {MaximumLength} caracteres.";
    }
}
