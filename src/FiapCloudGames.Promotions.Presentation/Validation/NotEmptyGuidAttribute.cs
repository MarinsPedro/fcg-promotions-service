using System.ComponentModel.DataAnnotations;
using FiapCloudGames.Promotions.Application.Validation;

namespace FiapCloudGames.Promotions.Presentation.Validation;

[AttributeUsage(
    AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
    AllowMultiple = false)]
public sealed class NotEmptyGuidAttribute : ValidationAttribute
{
    public override bool IsValid(object? value) =>
        value is null || value is Guid guid && guid != Guid.Empty;

    public override string FormatErrorMessage(string name) =>
        $"{InputFieldName.From(name)} deve possuir um identificador válido.";
}
