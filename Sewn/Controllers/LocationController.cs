using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Sewn.Models;
using Sewn.Providers;
using Sewn.Results;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;

namespace Sewn.Controllers
{
    [Authorize]
    [RoutePrefix("api/Location")]
    public class LocationController : ApiController
    {
        private ApplicationUserManager _userManager;

        public LocationController()
        {
        }

        public LocationController(ApplicationUserManager userManager)
        {
            this.UserManager = userManager;
        }

        public async void Post([FromBody]LocationModel value)
        {
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            user.Location = value;

            await _userManager.UpdateAsync(user);
        }

        public async Task<IEnumerable<ApplicationUser>> Get()
        {
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            var users = await UserManager.Users.Where(u => user.FriendsIds.Contains(u.Id)).ToListAsync();

            return users;
        }

        [Route("GetUser")]
        public async Task<ApplicationUser> GetUser()
        {
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            return user;
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
    }
}