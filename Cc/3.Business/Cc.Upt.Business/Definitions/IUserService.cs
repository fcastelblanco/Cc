﻿using System;
using System.Collections.Generic;
using Cc.Upt.Data.Definitions;
using Cc.Upt.Domain;
using Cc.Upt.Domain.Repository.Definitions;

namespace Cc.Upt.Business.Definitions
{
    public interface IUserService: IRepository<User>
    {
        User GetById(Guid id);
        bool Save(User user);
        IEnumerable<User> GetAllUsers();

        bool RecoverPassword(string userEmail, string url, string template);
        bool CreatePassword(string password, string url, string templatePath, string userEmail);
        bool SavePassword(Guid userId, string password);
    }
}
