using System;
using Identity.Service.Application.Constants;
using Identity.Service.Application.DTOs.Shared;

namespace Identity.Service.Application.Helpers
{
    public class EmailTemplateService
    {
        public EmailTemplateService()
        {
        }
        public async Task<bool> SendEmail(string source, string destination, string subject, string body)
        {
            List<string> destinations = new() { destination };
            //bool result = await aws.SendEmail(source, destinations, subject, body);
            return false;
        }
        public async Task<bool> SendForgetPasswordEmail(string source, string destination, string subject, 
            Guid InviteUserId, string emailHtml)
        {
            emailHtml = emailHtml.Replace("[[InviteUserLink]]", ParameterStoreDto.BaseUrlIdentity_FE + StaticUrls.InviteUserLink +
            InviteUserId);
            return await SendEmail(source, destination, subject, emailHtml);
        }
    }
}
