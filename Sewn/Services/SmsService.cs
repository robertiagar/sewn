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
        public static string TwilioSid = "AC2be51c0912f880b738215d1f9d9dc38b";
        public static string TwilioToken = "0ef30e5e6f73c43673aada4e3b532591";
        public static string FromPhone = "+1 415-599-2671";
    }
}