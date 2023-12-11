using HomeAccountant.Core.Authentication;
using Microsoft.AspNetCore.Components;

namespace HomeAccountant.Layout
{
    public partial class HeaderAuth : ComponentBase
    {
        private HeaderAction Action { get; set; }

        private void SetLogin() => Action = HeaderAction.Login;
        private void SetRegister() => Action = HeaderAction.Register;

        private enum HeaderAction
        {
            Login,
            Register
        }
    }
}
