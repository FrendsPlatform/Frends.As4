using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace Frends.As4.ValidateAndParsePayload.Attributes;

/// <summary>
/// Validates that a property value is not empty.
/// A value is considered empty when it is:
/// <list type="bullet">
///   <item><description><c>null</c></description></item>
///   <item><description>an empty or white-space-only <see cref="string"/></description></item>
///   <item><description>an <see cref="ICollection"/> (array, list, dictionary, …) with zero elements</description></item>
/// </list>
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
internal class NotEmptyAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var displayName = validationContext.DisplayName;

        if (value is null || (value is string s && string.IsNullOrWhiteSpace(s)) || value is ICollection { Count: 0 })
            return new ValidationResult(ErrorMessage ?? $"{displayName} must not be empty.");

        return ValidationResult.Success;
    }
}
