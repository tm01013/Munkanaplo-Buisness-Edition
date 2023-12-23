using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Munkanaplo2.Global
{
    public static class ManagerHelper
    {
        public static bool IsManager(string userName)
        {
            if (userName.Contains(" [Menedzser]")) return true;
            else return false;
        }
        public static bool IsManager(ClaimsPrincipal user)
        {
            if (user.Identity.Name.ToString().Contains(" [Menedzser]")) return true;
            else return false;
        }
    }
}