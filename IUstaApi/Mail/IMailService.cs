namespace IUstaApi.Mail
{
    public interface IMailService
    {
        public void SendConfirmationMessage(string email, string url);
        public void SendTaskAcceptanceMessage(string clientEmail, string workerEmail);
    }
}
