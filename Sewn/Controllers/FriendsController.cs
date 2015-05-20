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
using Sewn.Infrastructure;
using System.Collections;
using AutoMapper;
using PhoneNumbers;

namespace Sewn.Controllers
{
    [Authorize]
    [RoutePrefix("api/Friends")]
    public class FriendsController : BaseApiController
    {
        [Route("AddFriend")]
        public async Task<IHttpActionResult> AddFriend(string id)
        {
            var user = await UserManager.FindByIdAsync(UserId);
            var friend = await UserManager.FindByIdAsync(id);

            var friendship = new Friendship
            {
                Requester = user,
                RequesterId = user.Id,
                Accepter = friend,
                AccepterId = friend.Id
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

        [Route("AcceptFriend")]
        public async Task<IHttpActionResult> AcceptFriend(string id)
        {
            var user = UserManager.FindById(UserId);
            var friend = UserManager.FindById(id);
            var friendship = await DbContext.Friendships.Where(f => f.RequesterId == friend.Id && f.AccepterId == user.Id).SingleOrDefaultAsync();

            friendship.Status = Status.Accepted;
            friendship.Updated = DateTime.Now;

            var saved = await DbContext.SaveChangesAsync();

            if (saved != 0)
            {
                user.Friends.Add(friend);
                friend.Friends.Add(user);

                //TODO: this is bad code
                var results = new List<IdentityResult>();
                results.Add(await UserManager.UpdateAsync(user));
                results.Add(await UserManager.UpdateAsync(friend));

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

        [Route("BlockFriend")]
        public async Task<IHttpActionResult> BlockFriend(string id)
        {
            var user = await UserManager.FindByIdAsync(UserId);
            var friend = await UserManager.FindByIdAsync(id);

            var friendship = await (from friendshp in DbContext.Friendships
                                    where (friendshp.Requester.Id == user.Id || friendshp.Accepter.Id == user.Id) &&
                                    (friendshp.Requester.Id == friend.Id || friendshp.Accepter.Id == friend.Id)
                                    select friendshp).SingleOrDefaultAsync();

            friendship.Status = Status.Blocked;
            friendship.Updated = DateTime.Now;

            var result = await DbContext.SaveChangesAsync();

            if (result != 0)
            {
                return Ok();
            }

            return BadRequest();
        }

        [Route("SpamFriend")]
        public async Task<IHttpActionResult> SpamFriend(string id)
        {
            var user = await UserManager.FindByIdAsync(UserId);
            var friend = await UserManager.FindByIdAsync(id);

            var friendship = await (from friendshp in DbContext.Friendships
                                    where (friendshp.Requester.Id == user.Id || friendshp.Accepter.Id == user.Id) &&
                                    (friendshp.Requester.Id == friend.Id || friendshp.Accepter.Id == friend.Id)
                                    select friendshp).SingleOrDefaultAsync();

            friendship.Status = Status.Spam;
            friendship.Updated = DateTime.Now;

            var result = await DbContext.SaveChangesAsync();

            if (result != 0)
            {
                return Ok();
            }

            return BadRequest();
        }

        [Route("RevertToFriend")]
        public async Task<IHttpActionResult> RevertToFriend(string id)
        {
            var user = await UserManager.FindByIdAsync(UserId);
            var friend = await UserManager.FindByIdAsync(id);

            var friendship = await (from friendshp in DbContext.Friendships
                                    where (friendshp.Requester.Id == user.Id || friendshp.Accepter.Id == user.Id) &&
                                    (friendshp.Requester.Id == friend.Id || friendshp.Accepter.Id == friend.Id)
                                    select friendshp).SingleOrDefaultAsync();

            friendship.Status = Status.Accepted;
            friendship.Updated = DateTime.Now;

            var result = await DbContext.SaveChangesAsync();

            if (result != 0)
            {
                return Ok();
            }

            return BadRequest();
        }

        public async Task<UserViewModel> GetFriends()
        {
            //var friends = await (from friend in DbContext.Users
            //                     from friendship in DbContext.Friendships
            //                     where (friendship.User1.Id == UserId || friendship.User2.Id == UserId)
            //                     select friend).Distinct().ToListAsync();

            //var user = await UserManager.FindByIdAsync(UserId);

            //var selfs = friends.Where(x => x.Id == UserId);

            //foreach (var self in selfs.ToList())
            //    friends.Remove(self);

            //var userResult = Mapper.Map<UserViewModel>(user);

            //userResult.Friends = Mapper.Map<IEnumerable<UserViewModel>>(friends);

            var user = await UserManager.FindByIdAsync(UserId);

            var userResult = Mapper.Map<UserViewModel>(user);

            userResult.Friends = Mapper.Map<IEnumerable<UserViewModel>>(user.Friends);

            return userResult;
        }

        public async Task<IEnumerable<UserViewModel>> FindFriends(IList<FindPhoneViewModel> phoneNumbers)
        {
            var phoneUtil = PhoneNumberUtil.GetInstance();

            foreach (var phone in phoneNumbers)
            {
                int i = 0;
                foreach (var number in phone.PhoneNumbers.ToList())
                {
                    var phonenumber = phoneUtil.Parse(number, phone.Country);
                    phone.PhoneNumbers[i] = phoneUtil.Format(phonenumber, PhoneNumberFormat.E164);
                    i++;
                }
            }

            var users = await (from u in DbContext.Users
                               from p in phoneNumbers
                               where p.PhoneNumbers.Contains(u.PhoneNumber)
                               select u).Distinct().ToListAsync();

            var result = Mapper.Map<IEnumerable<UserViewModel>>(users);

            return result;
        }
    }
}
