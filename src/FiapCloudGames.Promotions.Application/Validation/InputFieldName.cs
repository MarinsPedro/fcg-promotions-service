using System.Text.Json;

namespace FiapCloudGames.Promotions.Application.Validation;

public static class InputFieldName
{
    public static string From(string propertyName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(propertyName);
        return JsonNamingPolicy.CamelCase.ConvertName(propertyName);
    }
}
