using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SistemaAutoStock.ViewModels;
using System.Threading.Tasks;

namespace SistemaAutoStock.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // =======================================================
        // REGISTRO
        // =======================================================
        [HttpGet("registrar")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost("registrar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(LoginViewModel registroVM)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = registroVM.UserName };
                var result = await _userManager.CreateAsync(user, registroVM.Password);

                if (result.Succeeded)
                {
                    string roleEscolhida = registroVM.NivelAcesso == "Coordenador" ? "Coordenador" : "Professor";
                    await _userManager.AddToRoleAsync(user, roleEscolhida);

                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("Registro", error.Description);
                    }
                }
            }
            return View(registroVM);
        }

        // =======================================================
        // LOGIN
        // =======================================================
        [HttpGet("login")]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost("login")]
        [ValidateAntiForgeryToken] 
        public async Task<IActionResult> Login(LoginViewModel loginVM, string? returnUrl = null) 
        { 
            ViewData["ReturnUrl"] = returnUrl; 
            if (!string.IsNullOrEmpty(loginVM.UserName) && !string.IsNullOrEmpty(loginVM.Password)) 
            { var result = await _signInManager.PasswordSignInAsync(loginVM.UserName, loginVM.Password, isPersistent: false, lockoutOnFailure: false); 
                if (result.Succeeded) 
                { 
                    var user = await _userManager.FindByNameAsync(loginVM.UserName); 
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)) 
                    { 
                        return Redirect(returnUrl); 
                    } 
                    if (await _userManager.IsInRoleAsync(user, "Coordenador")) 
                    { 
                        return RedirectToAction("Index", "Dashboard"); 
                    } 
                    else if (await _userManager.IsInRoleAsync(user, "Professor")) 
                    { 
                        return RedirectToAction("Selecionar", "Estoque"); 
                    } 
                    return Redirect("LoginView"); 
                } 
                else if (result.IsLockedOut) 
                { 
                    return RedirectToAction("Selecionar", "Estoque"); 
                } 
                ModelState.AddModelError(string.Empty, "Usuário ou senha inválidos."); 
            } return View(loginVM); 
        }


        [HttpGet("acesso-negado")]
        public IActionResult AccessDenied()
        {
            if (User.IsInRole("Professor"))
            {
                return RedirectToAction("", "Estoque");
            }

            else if (User.IsInRole("Coordenador"))
            {
                return RedirectToAction("dashboard", "Dashboard");
            }
            return Redirect("/login");
        }


        // =======================================================
        // LOGOUT
        // =======================================================
        [HttpPost("logout")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return Redirect("/login");
        }
    }
}