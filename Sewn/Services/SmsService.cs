using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Twilio;

namespace Sewn.Services
{
    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            var Twilio = new TwilioRestClient(Keys.TwilioSid, Keys.TwilioToken);
            var result = Twilio.SendMessage(Keys.FromPhone, message.Destination, message.Body);

            // Status is one of Queued, Sending, Sent, Failed or null if the number is not valid
            Trace.WriteLine(result.Status);

            // Twilio doesn't currently have an async API, so return success.
            return Task.FromResult(0);
        }
    }

    public static class Keys
    {
        public static string TwilioSid = "string";
        public static string TwilioToken = "token";
        public static string FromPhone = "phone";
    }
}