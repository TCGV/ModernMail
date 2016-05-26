# ModernMail
Mail Submission Agent (MSA) implementation for .NET

```c#
class Program
{
    static void Main(string[] args)
    {
        using (var sender = new MailSubmissionAgent("mydomain.com"))
        {
            using (var msg = new MailMessage())
            {
                msg.From = new MailAddress("admin@mydomain.com", "System Administrator");
                msg.To.Add(new MailAddress("john@yahoo.com"));
                msg.CC.Add(new MailAddress("bob@outlook.com"));

                msg.Subject = "Hello World!";
                msg.Body = "<div>Hello,</div><br><br>" +
                    "<div>This is a test e-mail with a file attached.</div><br><br>" +
                    "<div>Regards,</div><br><div>System Administrator</div>";
                msg.IsBodyHtml = true;

                var plain = "Hello,\n\nThis is a test e-mail with a file attached.\n\n" +
                    "Regards,\nSystem Administrator";
                var alt = AlternateView.CreateAlternateViewFromString(plain, new ContentType("text/plain"));
                msg.AlternateViews.Add(alt);

                var filePath = @"...\some-folder\pretty-image.png";
                var att = new Attachment(filePath);
                msg.Attachments.Add(att);

                var result = sender.Send(msg);
                Console.WriteLine(JsonConvert.SerializeObject(result));
            }
        }

        Console.ReadKey();
    }
}
```

This sample code depends on a JSON serializer (`JsonConvert.SerializeObject`) to easily print results to the console.
