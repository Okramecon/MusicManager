using System.ComponentModel.DataAnnotations;
using MusicManager.Data.Model;

namespace MusicManager.ViewModels
{
    public class RegisterViewModel : ViewModelBase<User>
    {
        [Required]
        public string Login
        {
            get => Model.Login;
            set => Model.Login = value;
        }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get=>Model.Password; set=>Model.Password = value; }
        [Required]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [DataType(DataType.Password)]
        [Display(Name = "Подтвердить пароль")]
        public string PasswordConfirm { get; set; }
    }
}