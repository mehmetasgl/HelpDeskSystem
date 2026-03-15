using FluentValidation;
using MiniHelpDesk.ViewModels;

namespace MiniHelpDesk.Validators
{
    public class EditProfileViewModelValidator : AbstractValidator<EditProfileViewModel>
    {
        public EditProfileViewModelValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Kullanıcı adı zorunludur")
                .MinimumLength(3).WithMessage("Kullanıcı adı en az 3 karakter olmalıdır")
                .MaximumLength(100).WithMessage("Kullanıcı adı maksimum 100 karakter olabilir");

            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz")
                .When(x => !string.IsNullOrEmpty(x.Email));
        }
    }
}