using FluentValidation;
using MiniHelpDesk.ViewModels;

namespace MiniHelpDesk.Validators
{
    public class EditTicketViewModelValidator : AbstractValidator<EditTicketViewModel>
    {
        public EditTicketViewModelValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Başlık zorunludur")
                .MaximumLength(100).WithMessage("Başlık maksimum 100 karakter olabilir")
                .MinimumLength(5).WithMessage("Başlık en az 5 karakter olmalıdır");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Açıklama zorunludur")
                .MaximumLength(1000).WithMessage("Açıklama maksimum 1000 karakter olabilir")
                .MinimumLength(10).WithMessage("Açıklama en az 10 karakter olmalıdır");

            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("Kategori seçimi zorunludur")
                .GreaterThan(0).WithMessage("Geçerli bir kategori seçiniz");

            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Durum zorunludur")
                .Must(x => x == "Open" || x == "Closed")
                .WithMessage("Durum 'Open' veya 'Closed' olmalıdır");

            RuleFor(x => x.Priority)
                .NotEmpty().WithMessage("Öncelik zorunludur")
                .Must(x => x == "Low" || x == "Medium" || x == "High")
                .WithMessage("Öncelik 'Low', 'Medium' veya 'High' olmalıdır");
        }
    }
}