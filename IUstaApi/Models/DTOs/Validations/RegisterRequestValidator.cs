using FluentValidation;
using IUstaApi.Models.DTOs.Auth;
using System.Text.RegularExpressions;

namespace IUstaApi.Models.DTOs.Validations
{
    public static class SharedValidator
    {
        public static bool BeValidPassword(string password)
        {
            return new Regex(@"\d").IsMatch(password)
                && new Regex(@"[a-z]").IsMatch(password)
                && new Regex(@"[A-Z]").IsMatch(password)
                && new Regex(@"[\.,';]").IsMatch(password);
        }
    }

    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(e => e.Email).EmailAddress().NotEmpty();
            RuleFor(e => e.Password).Must(SharedValidator.BeValidPassword).NotEmpty();
            RuleFor(e => e.Role).NotEmpty();
        }
    }
}
