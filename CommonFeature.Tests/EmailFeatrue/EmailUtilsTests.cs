using MimeKit;

namespace CommonFeatrue.Tests
{
    public class EmailUtilsTests
    {
        [Fact()]
        public async Task SendEMailAsyncTest()
        {
            var attachPath = new string[] { };
            var subject = "测试主题";
            //邮件信息先写死
            var email = new Email()
            {
                Host = "smtp.qq.com",
                //465/587
                Port = 587,
                Password = "授权码/企业邮箱密码",
                UseSsl = true,
                UserAddress = "发送邮件账号",
                UserName = "发送邮件名称",
            };
            var sendEmail = new EmailUtils(email);
            var fromAddress = new MailboxAddress[]
            {
                    new("发送方姓名", "发送方邮箱地址")
            };
            var toAddress = new MailboxAddress[]
            {
                    new("接收方名称", "接收方邮箱地址")

            };
            sendEmail.AddAttachment(attachPath);
            //抄送
            sendEmail.AddCcAddress("抄送人名", "抄送人邮箱地址");
            await sendEmail.SendEMailAsync(subject, fromAddress, toAddress);
            Assert.Fail("This test needs an implementation");
        }

        [Fact()]
        public void SendEMailTest()
        {
            Assert.Fail("This test needs an implementation");
        }
    }
}