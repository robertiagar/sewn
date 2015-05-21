﻿using System;
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

        [Route("AddFriends")]
        public async Task<IHttpActionResult> AddFriends(string[] ids)
        {
            var user = await UserManager.FindByIdAsync(UserId);

            foreach (var id in ids)
            {
                var friend = await UserManager.FindByIdAsync(id);

                var friendship = new Friendship
                {
                    Requester = user,
                    RequesterId = user.Id,
                    Accepter = friend,
                    AccepterId = friend.Id
                };

                DbContext.Friendships.Add(friendship);
            }

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

        [Route("FindFriends")]
        public async Task<FindPhoneViewModel> FindFriends([FromBody]FindPhonesModel phoneNumbers)
        {
            var phoneUtil = PhoneNumberUtil.GetInstance();

            foreach (var contact in phoneNumbers.Contacts)
            {
                int i = 0;
                foreach (var number in contact.PhoneNumbers.ToList())
                {
                    var phonenumber = phoneUtil.Parse(number, contact.Country);
                    contact.PhoneNumbers[i] = phoneUtil.Format(phonenumber, PhoneNumberFormat.E164);
                    i++;
                }
            }

            var numbers = from contact in phoneNumbers.Contacts
                          from phone in contact.PhoneNumbers
                          select phone;

            var users = await (from user in DbContext.Users
                               where numbers.Contains(user.PhoneNumber)
                               select user).Distinct().ToListAsync();

            var userViewModels = Mapper.Map<IEnumerable<UserViewModel>>(users);

            var result = new FindPhoneViewModel();

            result.Contacts = Mapper.Map<IList<ContactMatchModel>>(phoneNumbers.Contacts);

            foreach (var contact in result.Contacts)
            {
                contact.PossibleMatches = userViewModels.Where(u => contact.PhoneNumbers.Contains(u.PhoneNumber)).ToList();
            }

            return result;
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
    }
}
