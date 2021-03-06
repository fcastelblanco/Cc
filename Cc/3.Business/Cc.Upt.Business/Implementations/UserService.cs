﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cc.Upt.Business.Definitions;
using Cc.Upt.Business.Implementations.Singleton;
using Cc.Upt.Common.EmailHelper;
using Cc.Upt.Common.ExtensionMethods;
using Cc.Upt.Data.Definitions;
using Cc.Upt.Data.Implementations;
using Cc.Upt.Domain;
using Cc.Upt.Domain.Enumerations;

namespace Cc.Upt.Business.Implementations
{
    public class UserService : Repository<User>, IUserService
    {
        public UserService(IContext context) : base(context)
        {
            Dbset = context.Set<User>();
        }

        public User GetById(Guid id)
        {
            return Dbset.FirstOrDefault(x => x.Id == id);
        }

        public bool Save(User user)
        {
            try
            {
                var exists = Dbset.FirstOrDefault(x => x.Id == user.Id);

                if (exists != null)
                {
                    exists.Password = exists.Password;
                    exists.Name = user.Name;
                    exists.LastName = user.LastName;
                    exists.Email = user.Email;
                    exists.CompanyId = user.CompanyId;
                    exists.Profile = user.Profile;
                    exists.CompanyId = user.CompanyId;
                    Update(exists);
                }
                else
                {
                    var passwordEncoded = StringExtension.RandomString(5, true).Encrypt(StringExtension.PassPhrase);
                    user.Password = passwordEncoded;
                    Create(user);
                }
                return true;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        public IEnumerable<User> GetAllUsers()
        {
            return GetAll();
        }

        public bool RecoverPassword(string userEmail, string url, string templatePath)
        {
            try
            {
                var currentUser = FindBy(x => x.Email == userEmail).FirstOrDefault();

                if (currentUser == null)
                    return false;

                var template = File.ReadAllText(templatePath);

                if (string.IsNullOrEmpty(template))
                    return false;

                var dataDictionary = new Dictionary<string, string>
                {
                    {"{{name}}", currentUser.Name + " " + currentUser.LastName},
                    {"{{resetUrl}}", url},
                    {"{{contactUrl}}", "http://web.isolucion.com.co/contactenos/"}
                };

                template = dataDictionary.Aggregate(template, (current, data) => current.Replace(data.Key, data.Value));
                var parameterInstance = ParameterSingleton.Instance;

                EmailSender.SendEmail(currentUser.Email,
                    template,
                    "Recuperación de contraseña",
                    parameterInstance.ParameterList.FirstOrDefault(x => x.ParameterInternalIdentificator == ParameterInternalIdentificator.EmailSender).Value,
                    parameterInstance.ParameterList.FirstOrDefault(x => x.ParameterInternalIdentificator == ParameterInternalIdentificator.SmtpServer).Value,
                    Convert.ToInt32(parameterInstance.ParameterList.FirstOrDefault(x => x.ParameterInternalIdentificator == ParameterInternalIdentificator.SendingPort).Value),
                    Convert.ToBoolean(parameterInstance.ParameterList.FirstOrDefault(x => x.ParameterInternalIdentificator == ParameterInternalIdentificator.EnableSsl) == null 
                    ? 
                    false.ToString() 
                    : 
                    parameterInstance.ParameterList.FirstOrDefault(x => x.ParameterInternalIdentificator == ParameterInternalIdentificator.EnableSsl).Value),
                    parameterInstance.ParameterList.FirstOrDefault(x => x.ParameterInternalIdentificator == ParameterInternalIdentificator.UserSmtpServer).Value,
                    parameterInstance.ParameterList.FirstOrDefault(x => x.ParameterInternalIdentificator == ParameterInternalIdentificator.PasswordSender).Value);

                return true;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        public bool CreatePassword(string password, string url, string templatePath, string userEmail)
        {
            try
            {
                var currentUser = FindBy(x => x.Email == userEmail).FirstOrDefault();

                if (currentUser == null)
                    return false;

                var template = File.ReadAllText(templatePath);

                if (string.IsNullOrEmpty(template))
                    return false;

                var dataDictionary = new Dictionary<string, string>
                {
                    {"{{name}}", currentUser.Name + " " + currentUser.LastName},
                    {"{{clave}}", password.Encrypt(StringExtension.PassPhrase)},
                    {"{{url}}", url},
                    {"{{contactUrl}}", "http://web.isolucion.com.co/contactenos/"}
                };

                template = dataDictionary.Aggregate(template, (current, data) => current.Replace(data.Key, data.Value));
                var parameterInstance = ParameterSingleton.Instance;

                EmailSender.SendEmail(userEmail,
                    template,
                    "Creación de contraseña",
                    parameterInstance.ParameterList.FirstOrDefault(x => x.ParameterInternalIdentificator == ParameterInternalIdentificator.EmailSender).Value,
                    parameterInstance.ParameterList.FirstOrDefault(x => x.ParameterInternalIdentificator == ParameterInternalIdentificator.SmtpServer).Value,
                    Convert.ToInt32(parameterInstance.ParameterList.FirstOrDefault(x => x.ParameterInternalIdentificator == ParameterInternalIdentificator.SendingPort).Value),
                     Convert.ToBoolean(parameterInstance.ParameterList.FirstOrDefault(x => x.ParameterInternalIdentificator == ParameterInternalIdentificator.EnableSsl) == null
                    ?
                    false.ToString()
                    :
                    parameterInstance.ParameterList.FirstOrDefault(x => x.ParameterInternalIdentificator == ParameterInternalIdentificator.EnableSsl).Value),
                    parameterInstance.ParameterList.FirstOrDefault(x => x.ParameterInternalIdentificator == ParameterInternalIdentificator.UserSmtpServer).Value,
                    parameterInstance.ParameterList.FirstOrDefault(x => x.ParameterInternalIdentificator == ParameterInternalIdentificator.PasswordSender).Value);

                return true;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        public bool SavePassword(Guid userId, string password)
        {
            var currentUser = FindBy(x => x.Id == userId).FirstOrDefault();

            if (currentUser == null)
                return false;

            try
            {
                currentUser.Password = password.Encrypt(StringExtension.PassPhrase);
                Update(currentUser);
                return true;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }
    }
}