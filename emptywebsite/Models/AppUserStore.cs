﻿using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;

namespace WEB.Models
{
    public interface IAppUserStore : IUserStore<User, Guid>
    {
    }

    public class AppUserStore :
        UserStore<User, AppRole, Guid, AppUserLogin, AppUserRole, AppUserClaim>,
        IAppUserStore
    {
        public AppUserStore() : base(new ApplicationDbContext())
        {

        }

        public AppUserStore(ApplicationDbContext context) : base(context)
        {

        }
    }
}