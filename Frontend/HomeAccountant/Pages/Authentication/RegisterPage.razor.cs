﻿using HomeAccountant.Core.ViewModels;
using Microsoft.AspNetCore.Components;

namespace HomeAccountant.Pages.Authentication
{
    public partial class RegisterPage : ComponentBase
    {
        [Inject]
        public required LoginViewModel ViewModel { get; set; }
    }
}
