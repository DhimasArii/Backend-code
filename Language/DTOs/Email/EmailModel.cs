namespace Language.DTOs.Email
{
    public class EmailModel
    {
        public List<string> To { get; }
        public List<string> Bcc { get; }
        public List<string> Cc { get; }

        public string Subject { get; }
        public string? Body { get; }

        public EmailModel(
            List<string> to, string subject, string? body = null, List<string>? bcc = null, List<string>? cc = null
        )
        {
            To = to;
            Bcc = bcc ?? new List<string>();
            Cc = cc ?? new List<string>();

            Subject = subject;
            Body = body;
        }
    }
}
