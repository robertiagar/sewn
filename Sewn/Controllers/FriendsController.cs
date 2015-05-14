using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using Sewn.Models;

namespace Sewn.Controllers
{
    [Authorize]
    public class FriendsController : ApiController
    {
        private ApplicationUserManager _userManager;

        public FriendsController()
        {
        }

        public FriendsController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        public async Task<IHttpActionResult> AddFriend(string id)
        {
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            var friendToAdd = await UserManager.FindByIdAsync(id);

            var friend = new Friend
            {
                Id = friendToAdd.Id
            };

            user.Friends.Add(friend);

            var result = await (UserManager.UpdateAsync(user));
            if (result.Succeeded)
            {
                return Ok();
            }
            else
            {
                var errors = string.Join(Environment.NewLine, result.Errors);
                return BadRequest(errors);
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }
    }
}
