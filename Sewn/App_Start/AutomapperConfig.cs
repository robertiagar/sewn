using AutoMapper;
using Sewn.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sewn
{
    public class AutomapperConfig
    {
        public static void RegisterAutomapper()
        {
            Mapper.CreateMap<ApplicationUser, UserViewModel>().ForMember(x => x.Friends, opt => opt.Ignore());
            Mapper.CreateMap<ContactModel, ContactMatchModel>().ForMember(x => x.PossibleMatches, opt => opt.Ignore());
        }
    }
}