using FluentValidation;
using MiniHelpDesk.ViewModels;

namespace MiniHelpDesk.Validators
{
    public class LoginViewModelValidator : AbstractValidator<LoginViewModel>
    {
        public LoginViewModelValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Kullanıcı adı zorunludur");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Şifre zorunludur");
        }
    }
}