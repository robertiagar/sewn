using System;
using System.Collections.Generic;
using System.Data.Entity;
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
    public class FriendsController : BaseApiController
    {
        public async Task<IHttpActionResult> AddFriend(string id)
        {
            var user = await UserManager.FindByIdAsync(UserId);
            var friend = await UserManager.FindByIdAsync(id);

            var friendship = new Friendship
            {
                User1 = user,
                UserId1 = user.Id,
                User2 = friend,
                UserId2 = friend.Id
            };

            DbContext.Friendships.Add(friendship);

            var result = await DbContext.SaveChangesAsync();

            if (result != 0)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        public async Task<IHttpActionResult> AcceptFriend(string id)
        {
            var user = UserManager.FindById(UserId);
            var friend = UserManager.FindById(id);
            var friendship = await DbContext.Friendships.Where(f => f.UserId1 == friend.Id && f.UserId2 == user.Id).SingleOrDefaultAsync();

            friendship.Status = Status.Accepted;
            friendship.Updated = DateTime.Now;

            var saved = await DbContext.SaveChangesAsync();

            if (saved != 0)
            {
                user.Friends.Add(friend);
                friend.Friends.Add(user);

                //TODO: this is bad code
                var tasks = new List<Task<IdentityResult>>();
                tasks.Add(UserManager.UpdateAsync(user));
                tasks.Add(UserManager.UpdateAsync(friend));

                var results = await Task.WhenAll(tasks);
                foreach (var result in results)
                {
                    if (!result.Succeeded)
                    {
                        return BadRequest(string.Join(System.Environment.NewLine, result.Errors)); //TODO: this returns on first error. Rollback measures must be taken and implemented.
                    }
                }

                //i'll be surprised if we get here on the first try
                return Ok();
            }

            return BadRequest();
        }
    }
}
