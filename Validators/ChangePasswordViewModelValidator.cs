using FluentValidation;
using MiniHelpDesk.ViewModels;

namespace MiniHelpDesk.Validators
{
    public class ChangePasswordViewModelValidator : AbstractValidator<ChangePasswordViewModel>
    {
        public ChangePasswordViewModelValidator()
        {
            RuleFor(x => x.CurrentPassword)
                .NotEmpty().WithMessage("Mevcut şifre zorunludur");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("Yeni şifre zorunludur")
                .MinimumLength(6).WithMessage("Yeni şifre en az 6 karakter olmalıdır")
                .Matches("[A-Z]").WithMessage("Yeni şifre en az bir büyük harf içermelidir")
                .Matches("[a-z]").WithMessage("Yeni şifre en az bir küçük harf içermelidir")
                .Matches("[0-9]").WithMessage("Yeni şifre en az bir rakam içermelidir")
                .NotEqual(x => x.CurrentPassword).WithMessage("Yeni şifre, mevcut şifreden farklı olmalıdır");

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("Şifre tekrar zorunludur")
                .Equal(x => x.NewPassword).WithMessage("Şifreler eşleşmiyor");
        }
    }
}