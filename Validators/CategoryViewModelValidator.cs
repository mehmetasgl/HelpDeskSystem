using FluentValidation;
using MiniHelpDesk.ViewModels;

namespace MiniHelpDesk.Validators
{
    public class CategoryViewModelValidator : AbstractValidator<CategoryViewModel>
    {
        public CategoryViewModelValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Kategori adı zorunludur")
                .MaximumLength(50).WithMessage("Kategori adı maksimum 50 karakter olabilir")
                .MinimumLength(3).WithMessage("Kategori adı en az 3 karakter olmalıdır");
        }
    }
}