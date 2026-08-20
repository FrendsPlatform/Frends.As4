using System;
using System.ComponentModel.DataAnnotations;

namespace Frends.As4.ValidateAndParsePayload.Attributes;

/// <summary>
/// Validates that a property is required if any of the specified dependent properties has the target value.
/// If the decorated property is null, empty, or white space only, validation fails.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
internal class RequiredIfAnyAttribute(object targetValue, params string[] dependentProperties) : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        foreach (var propertyName in dependentProperties)
        {
            var field = validationContext.ObjectType.GetProperty(propertyName);

            if (field == null)
                return new ValidationResult($"Unknown property: {propertyName}");

            var dependentValue = field.GetValue(validationContext.ObjectInstance);

            if (!Equals(dependentValue, targetValue))
                continue;

            if (value == null || (value is string s && string.IsNullOrWhiteSpace(s)))
                return new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName} is required.");

            return ValidationResult.Success;
        }

        return ValidationResult.Success;
    }
}


