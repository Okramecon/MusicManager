using System.ComponentModel.DataAnnotations;
using MusicManager.Data.Model;

namespace MusicManager.ViewModels
{
    public class UserViewModel : ViewModelBase<User>
    {
        public UserViewModel(User model) : base(model)
        {
        }

        public UserViewModel()
        {
        }

        [Display(Name = "Имя (логин)")]
        public string Name
        {
            get => Model.Login;
            set => Model.Login = value;
        }

        public int Id
        {
            get => Model.Id;
            set => Model.Id = value;
        }

        public int RoleId
        {
            get => Model.RoleId;
            set => Model.RoleId = value;
        }

        [Display(Name = "Заблокирован")]
        public bool IsBlocked
        {
            get => Model.IsBlocked;
            set => Model.IsBlocked = value;
        }

        [Display(Name = "Роль")]
        public string RoleName => Model.Role.Name;
    }
}
