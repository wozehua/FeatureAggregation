namespace CommonFeatrue
{
    /// <summary>
    /// Email模型
    /// </summary>
    public sealed class Email
    {
        /// <summary>
        /// 邮件服务器Host 如：
        /// 邮箱	POP3服务器（端口995）	SMTP服务器（端口465或587）
        /// qq.com pop.qq.com       smtp.qq.com
        /// 必填
        /// </summary>
        public string Host { get; set; }
        /// <summary>
        /// 邮件服务器端口:如SMTP服务器（端口465或587）
        ///  必填
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// 邮件服务器是否是ssl
        /// </summary>
        public bool UseSsl { get; set; }
        /// <summary>
        /// 发送邮件的账号名称
        ///  必填
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 发送邮件账号
        ///  必填
        /// </summary>
        public string UserAddress { get; set; }
        /// <summary>
        /// 发送邮件账号所需密码(授权码)
        /// qq邮箱授权码不是账号密码
        /// 企业邮箱就是账户密码
        ///  必填
        /// 腾讯邮箱：更改QQ密码以及独立密码会触发授权码过期，需要重新获取新的授权码登录。
        /// </summary>
        public string Password { get; set; }
    }
}
