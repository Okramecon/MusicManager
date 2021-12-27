using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MusicManager.Data.Model;
using System.ComponentModel.DataAnnotations;

namespace MusicManager.ViewModels
{
    public class LoginViewModel : ViewModelBase<User>
    {
        public string Login
        {
            get => Model.Login;
            set => Model.Login = value;
        }

        public string Password
        {
            get => Model.Password;
            set => Model.Password = value;
        }
    }
}
