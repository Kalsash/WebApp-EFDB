﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Serilog;
using System.Net.Mail;
using System.Net;

namespace WebApp_Landing.Pages
{
    public class PrivacyModel : PageModel
    {
        private readonly ILogger<PrivacyModel> _logger;

        [BindProperty]
        public SubscribeModel Subscribe { get; set; }

        public bool ShowSuccessPopup { get; private set; }

        public PrivacyModel(ILogger<PrivacyModel> logger)
        {
            _logger = logger;
        }

        public async void OnPostSubscribeAsync()
        {
            if (ModelState.IsValid)
            {
                Log.Information("New subscription from {Email}", Subscribe.Email);
                ShowSuccessPopup = true;
                await SendEmailAsync(Subscribe.Email);
            }
        }

        private async Task SendEmailAsync(string email)
        {
            string myEmail = "kalsashs@gmail.com";
            string myPassword = System.IO.File.ReadAllText("PostPassword.txt").Trim();
            using (var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(myEmail, myPassword),
                EnableSsl = true,
            })
            {
                var mailMessage = new MailMessage(myEmail, email, "Subscription Confirmation", "Thank you for subscribing!");
                await smtpClient.SendMailAsync(mailMessage);
            }
        }
        public IActionResult OnPostClosePopup()
        {
            ShowSuccessPopup = false;
            return Page();
        }

        public void OnGet()
        {
        }
    }

}
