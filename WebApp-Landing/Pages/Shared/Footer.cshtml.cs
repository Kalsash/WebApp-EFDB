using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Serilog;
using System.Net;
using System.Net.Mail;

namespace WebApp_Landing.Pages.Shared
{
    public class FooterModel : PageModel
    {
        [BindProperty]
        public SubscribeModel Subscribe { get; set; }

        public IActionResult OnPostSubscribe()
        {
            if (ModelState.IsValid)
            {
                Log.Information("New subscription from {Email}", Subscribe.Email);
                SendEmail(Subscribe.Email);
                return RedirectToPage();
            }
            return Page();
        }

        private void SendEmail(string email)
        {
            string MyEmail = "kalsashs@gmail.com";
            string MyPassword = System.IO.File.ReadAllText("PostPassword.txt").Trim();
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(MyEmail, MyPassword),
                EnableSsl = true,
            };

            smtpClient.Send(MyEmail, email, "Subscription Confirmation", "Thank you for subscribing!");
        }

        public void OnGet()
        {
        }
    }
}
