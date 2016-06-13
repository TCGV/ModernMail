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

## Remarks

- Don't forget to add [SPF](https://en.wikipedia.org/wiki/Sender_Policy_Framework) and [PTR](https://en.wikipedia.org/wiki/Reverse_DNS_lookup) records for your domain otherwise SMTP servers won't trust e-mails you send
- [DKIM](https://en.wikipedia.org/wiki/DomainKeys_Identified_Mail) authentication not supported yet

##Licensing

This code is released under the MIT License:

Copyright (c) TCGV.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
