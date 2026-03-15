using FluentValidation;
using MiniHelpDesk.ViewModels;

namespace MiniHelpDesk.Validators
{
    public class RegisterViewModelValidator : AbstractValidator<RegisterViewModel>
    {
        public RegisterViewModelValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Kullanıcı adı zorunludur")
                .MaximumLength(100).WithMessage("Kullanıcı adı maksimum 100 karakter olabilir")
                .MinimumLength(3).WithMessage("Kullanıcı adı en az 3 karakter olmalıdır");

            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz")
                .When(x => !string.IsNullOrEmpty(x.Email)); // Email opsiyonel

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Şifre zorunludur")
                .MinimumLength(6).WithMessage("Şifre en az 6 karakter olmalıdır")
                .Matches("[A-Z]").WithMessage("Şifre en az bir büyük harf içermelidir")
                .Matches("[a-z]").WithMessage("Şifre en az bir küçük harf içermelidir")
                .Matches("[0-9]").WithMessage("Şifre en az bir rakam içermelidir");

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("Şifre tekrar zorunludur")
                .Equal(x => x.Password).WithMessage("Şifreler eşleşmiyor");
        }
    }
}