﻿using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System.Collections.Generic;
using System.Data.Entity;
using System;

namespace Sewn.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }

        public virtual Location Location { get; set; }
        public virtual IList<ApplicationUser> Friends { get; set; }

        public ApplicationUser()
        {
            Location = new Location();
            Friends = new List<ApplicationUser>();
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<Friendship> Friendships { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Friendship>().HasKey(f => new { RequesterId = f.RequesterId, AccepterId = f.AccepterId });
            modelBuilder.Entity<Friendship>().HasRequired(f => f.Requester)
                .WithMany()
                .HasForeignKey(f => f.RequesterId);
            modelBuilder.Entity<Friendship>().HasRequired(f => f.Accepter)
                .WithMany()
                .HasForeignKey(f => f.AccepterId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ApplicationUser>().HasKey(u => u.Id);

            base.OnModelCreating(modelBuilder);
        }
    }
}