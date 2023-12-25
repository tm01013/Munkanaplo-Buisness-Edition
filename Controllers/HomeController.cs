using System.Diagnostics;
using dotenv.net;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Munkanaplo2.Data;
using Munkanaplo2.Global;
using Munkanaplo2.Models;

namespace Munkanaplo2.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        if (!IsConfigCorrect()) return View("ConfigError");

        if (DotEnv.Read()["USE_MANAGERS"].ToLower() == "false")
        {
            List<IdentityUser> users = _context.Users.ToList();
            for (int i = 0; i < users.Count; i++)
            {
                if (DotEnv.Read()["ADMIN_USERNAME"].ToString() != users[i].UserName)
                {
                    if (ManagerHelper.IsManager(users[i].UserName.ToString()))
                    {
                        IdentityUser oldUser = _context.Users.FirstOrDefault(u => u.Id == users[i].Id);
                        oldUser = new IdentityUser
                        {
                            Id = oldUser.Id,
                            UserName = oldUser.UserName.Split(" [Menedzser]")[0],
                            NormalizedUserName = oldUser.NormalizedUserName.Split(" [MENEDZSER]")[0],
                            Email = oldUser.UserName.Split(" [Menedzser]")[0],
                            NormalizedEmail = oldUser.NormalizedUserName.Split(" [MENEDZSER]")[0],
                            EmailConfirmed = oldUser.EmailConfirmed,
                            PasswordHash = oldUser.PasswordHash,
                            SecurityStamp = oldUser.ConcurrencyStamp,
                            ConcurrencyStamp = oldUser.ConcurrencyStamp,
                            PhoneNumber = oldUser.PhoneNumber,
                            PhoneNumberConfirmed = oldUser.PhoneNumberConfirmed,
                            TwoFactorEnabled = oldUser.TwoFactorEnabled,
                            LockoutEnd = oldUser.LockoutEnd,
                            LockoutEnabled = oldUser.LockoutEnabled,
                            AccessFailedCount = oldUser.AccessFailedCount
                        };

                        _context.Update(oldUser);
                        _context.SaveChanges();
                    }

                    if (!_context.Workers.Where(w => w.User == users[i].UserName).Any())
                    {
                        WorkerModel worker = new WorkerModel
                        {
                            User = users[i].UserName,
                            MoneyPerHour = 0
                        };

                        _context.Add(worker);
                    }
                }

            }
        }

        return View();
    }

    public IActionResult Hiba()
    {
        return View("Hiba");
    }
    public IActionResult AccesDenied()
    {
        return View("AccesDenied");
    }

    public IActionResult Privacy()
    {
        if (!IsConfigCorrect()) return View("ConfigError");
        return View();
    }

    public IActionResult ConfigError()
    {
        return View();
    }

    bool IsConfigCorrect()
    {
        if (!DotEnv.Read().ContainsKey("ADMIN_USERNAME")) return false;
        if (!DotEnv.Read().ContainsKey("USE_MANAGERS")) return false;
        if (DotEnv.Read()["USE_MANAGERS"].ToLower().Trim() == "") return false;

        if (DotEnv.Read()["USE_MANAGERS"].ToLower() == "false" && DotEnv.Read()["ADMIN_USERNAME"].ToLower().Trim() == "") return false;
        if (DotEnv.Read()["USE_MANAGERS"].ToLower() != "true" && DotEnv.Read()["USE_MANAGERS"].ToLower() != "false") return false;

        return true;
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

