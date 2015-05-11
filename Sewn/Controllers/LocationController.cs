using Sewn.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using System.Collections;
using System.Data.Entity;
using System.Threading.Tasks;

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
            this._userManager = userManager;
        }

        public async void Post([FromBody]LocationModel value)
        {
            var user = await _userManager.FindByIdAsync(User.Identity.GetUserId());

            user.Location = value;

            await _userManager.UpdateAsync(user);
        }

        public async Task<IEnumerable<ApplicationUser>> Get()
        {
            var user = await _userManager.FindByIdAsync(User.Identity.GetUserId());

            var users = await _userManager.Users.Where(u => user.FriendsIds.Contains(u.Id)).ToListAsync();

            return users;
        }
    }
}