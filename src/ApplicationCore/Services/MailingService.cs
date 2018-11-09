using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;

namespace ApplicationCore.Services
{
    public class MailingService : IMailingService
    {
        private const string MailAddress = "elektroniczny.dziennik.szkolny@gmail.com";
        private const string Password = "eschooljournal";
        private const string MessageFooter = "Podana wiadomość została wysłana automatycznie przez system. Proszę na nią nie odpowiadać. W razie pytań należy zgłosić się do administratora systemu.";

        public async Task SendEmailWithLoginAndPasswordAsync(User newUser, User administrator)
        {
            const string subject = "Twoje konto w systemie Elektroniczny Dziennik Szkolny zostało utworzone";
            StringBuilder body = new StringBuilder();
            body.AppendLine($"Administrator systemu Elektroniczny Dziennik Szkolny utworzył dla Ciebie konto użytkownika. Jeżeli nie masz jeszcze dostępu do systemu, należy zgłosić się do administratora, który udzieli dalszych informacji. W Twojej szkole jest nim {administrator.FullName} (e-mail: {administrator.Email}).");
            body.AppendLine();
            body.AppendLine($"Login: {newUser.Login}");
            body.AppendLine($"Hasło: {newUser.Password}");
            body.AppendLine();
            body.AppendLine("Proszę jak najszybciej zmienić swoje hasło po zalogowaniu się w systemie. Należy pamiętać, że hasło musi być silne i dobrze strzeżone!");
            body.AppendLine();
            body.AppendLine(MessageFooter);

            await SendEmailAsync(subject, newUser.Email, body.ToString());
        }

        public async Task SendEmailWithRecoveryCodeAsync(string email, int code)
        {
            const string subject = "Kod resetujący hasło w systemie Elektroniczny Dziennik Szkolny";
            StringBuilder body = new StringBuilder();
            body.AppendLine("Ktoś właśnie wysłał prośbę o zresetowanie hasła na Twoim koncie w systemie Elektroniczny Dziennik Szkolny. Jeżeli to nie byłeś Ty to istnieje szansza, że ktoś próbuje przejąć Twoje konto. Zaloguj się do systemu i zmień swoje hasło. Pamiętaj, aby było ono silne i dobrze strzeżone!");
            body.AppendLine("Jeżeli jednak to Ty wysłałeś prośbę o zresetowania hasła wprowadź poniższy kod resetujący w programie.");
            body.AppendLine();
            body.AppendLine($"Kod resetujący hasło: {code}");
            body.AppendLine();
            body.AppendLine(MessageFooter);

            await SendEmailAsync(subject, email, body.ToString());
        }

        public async Task SendEmailWithNewPasswordAsync(string email, string password)
        {
            const string subject = "Wygenerowano nowe hasło w systemie Elektroniczny Dziennik Szkolny";
            StringBuilder body = new StringBuilder();
            body.AppendLine($"Nowe hasło: {password}");
            body.AppendLine();
            body.AppendLine("Proszę jak najszybciej zmienić swoje hasło po zalogowaniu się w systemie. Należy pamiętać, że hasło musi być silne i dobrze strzeżone!");
            body.AppendLine();
            body.AppendLine(MessageFooter);

            await SendEmailAsync(subject, email, body.ToString());
        }

        private async Task SendEmailAsync(string subject, string email, string body)
        {
            // TODO: sprawdzać wcześniej czy adres email ma poprawną formę.
            var mailMessage = new MailMessage
            {
                From = new MailAddress(MailAddress),
                Subject = subject,
                Body = body
            };
            mailMessage.To.Add(email);

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new System.Net.NetworkCredential(MailAddress, Password),
                EnableSsl = true
            };

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
