using MimeKit;
using MailKit.Net.Smtp;
using MimeKit.Text;

namespace CommonFeatrue
{
    /// <summary>
    /// 邮件通用类
    /// </summary>
    public sealed class EmailUtils
    {
        /// <summary>
        /// 要发送的附件列表
        /// </summary>
        private List<Attachment> Attachments { get; set; }

        /// <summary>
        /// 邮件内容
        /// </summary>
        public string Content { get; set; } = default;

        /// <summary>
        /// 内容主题模式，默认TextFormat.Text
        /// </summary>
        public TextFormat TextFormat { get; set; } = TextFormat.Text;
        /// <summary>
        /// 是否自动释放附件所用Stream
        /// 默认true
        /// </summary>
        public bool IsAttachmentDispose { get; set; } = true;
        /// <summary>
        /// 抄送人信息
        /// </summary>
        private List<MailboxAddress> CcAddress { get; set; }
        /// <summary>
        /// 暗抄送人信息
        /// </summary>
        private List<MailboxAddress> BccAddress { get; set; }
        private readonly Email _email;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="email"></param>
        public EmailUtils(Email email)
        {
            _email = email;
        }

        /// <summary>
        /// 发送电子邮件
        /// </summary>
        /// <param name="subject">邮件主题</param>
        /// <param name="fromAddress">发送方信息</param>
        /// <param name="toAddress">接收方信息</param>
        /// <exception cref="ArgumentException"></exception>
        /// <returns></returns>
        public void SendEMail(string subject, IEnumerable<MailboxAddress> fromAddress,
            IEnumerable<MailboxAddress> toAddress)
        {
            var message = BuildMimeMessage(subject, fromAddress, toAddress);
            SendEmail(message, _email.UseSsl);
            DisposeAttachment();
        }

        /// <summary>
        /// 发送电子邮件
        /// </summary>
        /// <param name="subject">邮件主题</param>
        /// <param name="fromAddress">发送方信息</param>
        /// <param name="toAddress">接收方信息</param>
        /// <returns></returns>
        public async Task SendEMailAsync(string subject, IEnumerable<MailboxAddress> fromAddress,
            IEnumerable<MailboxAddress> toAddress)
        {
            var message = BuildMimeMessage(subject, fromAddress, toAddress);
            await SendEmailAsync(message, _email.UseSsl);
            await DisposeAttachmentAsync();
        }

        /// <summary>
        /// 构建邮件信息
        /// </summary>
        /// <param name="subject">邮件主题</param>
        /// <param name="fromAddress">发送方信息</param>
        /// <param name="toAddress">接收方信息</param>
        /// <returns></returns>
        private MimeMessage BuildMimeMessage(string subject, IEnumerable<MailboxAddress> fromAddress,
            IEnumerable<MailboxAddress> toAddress)
        {
            var message = new MimeMessage();
            if (fromAddress == null || !fromAddress.Any())
            {
                throw new ArgumentException("发", nameof(fromAddress));
            }
            if (toAddress == null || !toAddress.Any())
            {
                throw new ArgumentException(nameof(toAddress));
            }
            message.From.AddRange(fromAddress);
            message.To.AddRange(toAddress);
            if (CcAddress != null && CcAddress.Any())
            {
                message.Cc.AddRange(CcAddress);
            }
            if (BccAddress != null && BccAddress.Any())
            {
                message.Bcc.AddRange(BccAddress);
            }
            message.Subject = subject;
            var multipart = new Multipart("mixed");
            TextPart body = null;
            if (!string.IsNullOrWhiteSpace(Content))
            {
                body = new TextPart(TextFormat)
                {
                    Text = Content
                };
                multipart.Add(body);
            }
            MimeEntity entity = body;
            //邮件协议中的三种情况,对应下面的三种类型，mixed可以包含附件
            if (Attachments != null)
            {
                foreach (var att in Attachments)
                {
                    if (att.Stream == null) continue;
                    var attachment = string.IsNullOrWhiteSpace(att.ContentType)
                        ? new MimePart()
                        : new MimePart(att.ContentType);
                    attachment.Content = new MimeContent(att.Stream);
                    attachment.ContentDisposition = new ContentDisposition(ContentDisposition.Attachment);
                    attachment.ContentTransferEncoding = att.ContentTransferEncoding;
                    attachment.FileName = att.FileName;
                    multipart.Add(attachment);
                }
                entity = multipart;
            }

            message.Body = entity;
            return message;
        }

        /// <summary>
        /// 附件回收
        /// </summary>
        private void DisposeAttachment()
        {
            if (!IsAttachmentDispose || Attachments == null) return;
            {
                foreach (var att in Attachments)
                {
                    att.Dispose();
                }
            }
        }
        /// <summary>
        /// 附件异步回收
        /// </summary>
        /// <returns></returns>
        private async ValueTask DisposeAttachmentAsync()
        {
            if (!IsAttachmentDispose || Attachments == null) return;
            {
                foreach (var att in Attachments)
                {
                    await att.DisposeAsync();
                }
            }
        }

        /// <summary>
        /// 添加附件
        /// </summary>
        /// <param name="attachmentPath">附件地址</param>
        public void AddAttachment(string[] attachmentPath)
        {
            Attachments ??= new List<Attachment>();
            foreach (var item in attachmentPath)
            {
                //这里不能using回收等发完邮件统一回收
                var stream = File.OpenRead(item);
                var att = new Attachment
                {
                    FileName = Path.GetFileName(item),
                    Stream = stream,
                    ContentTransferEncoding = ContentEncoding.Base64
                };
                Attachments.Add(att);
            }
        }
        /// <summary>
        /// 批量添加抄送人
        /// </summary>
        /// <param name="carBonCopyAddress">抄送人</param>
        public void BatchAddCcAddress(IEnumerable<MailboxAddress> carBonCopyAddress)
        {
            CcAddress ??= new List<MailboxAddress>();
            CcAddress.AddRange(carBonCopyAddress);
        }
        /// <summary>
        /// 添加抄送人
        /// </summary>
        /// <param name="name">抄送人名</param>
        /// <param name="address">抄送人地址</param>
        public void AddCcAddress(string name, string address)
        {
            CcAddress ??= new List<MailboxAddress>();
            CcAddress.Add(new MailboxAddress(name, address));
        }
        /// <summary>
        /// 批量暗抄送人
        /// </summary>
        /// <param name="bccCopyAddress">暗抄送人</param>
        public void BatchAddBccAddress(IEnumerable<MailboxAddress> bccCopyAddress)
        {
            BccAddress ??= new List<MailboxAddress>();
            BccAddress.AddRange(bccCopyAddress);
        }
        /// <summary>
        /// 添加暗抄送人
        /// </summary>
        /// <param name="name">抄送人名</param>
        /// <param name="address">抄送人地址</param>
        public void AddBccAddress(string name, string address)
        {
            BccAddress ??= new List<MailboxAddress>();
            BccAddress.Add(new MailboxAddress(name, address));
        }


        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="email">邮件信息</param>
        /// <param name="useSsl">邮件服务器是否是ssl</param>
        private void SendEmail(MimeMessage email, bool useSsl)
        {
            //发送邮件
            using var smtp = new SmtpClient();
            smtp.Connect(_email.Host, _email.Port, useSsl);
            smtp.Authenticate(_email.UserName, _email.Password);
            smtp.Send(email);
            smtp.Disconnect(quit: true);
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="email">邮件信息</param>
        /// <param name="useSsl">邮件服务器是否是ssl</param>
        private async Task SendEmailAsync(MimeMessage email, bool useSsl)
        {
            //发送邮件
            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_email.Host, _email.Port, useSsl);
            await smtp.AuthenticateAsync(_email.UserName, _email.Password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(quit: true);
        }
    }
}
