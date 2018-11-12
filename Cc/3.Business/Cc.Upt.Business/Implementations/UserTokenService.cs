using System;
using System.Data.Entity;
using System.Linq;
using Cc.Upt.Business.Definitions;
using Cc.Upt.Data.Definitions;
using Cc.Upt.Data.Implementations;
using Cc.Upt.Domain;
using Cc.Upt.Domain.Enumerations;

namespace Cc.Upt.Business.Implementations
{
    public class UserTokenService : EntityService<UserToken>, IUserTokenService
    {
        public UserTokenService(IContext context) : base(context)
        {
        }

        public bool Save(UserToken userToken)
        {
            try
            {
                Create(userToken);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public bool IsValidToken(Guid token, TokenType tokenType)
        {
            var currentDate = DateTime.Now.Date;

            var currentToken =
                FindBy(
                        x =>
                            x.Token == token && x.TokenType == tokenType &&
                            DbFunctions.TruncateTime(x.Expiration) >= currentDate)
                    .FirstOrDefault();

            return currentToken != null;
        }

        public bool DeleteAllUserTokenByTokenType(Guid userId, TokenType tokenType)
        {
            var allUserToken = FindBy(x => x.UserId == userId && x.TokenType == tokenType).ToList();

            foreach (var userToken in allUserToken)
                Delete(userToken);

            return true;
        }

        public UserToken GetUserTokenByTokenAndTokenType(Guid token, TokenType tokenType)
        {
            return FindBy(x => x.Token == token && x.TokenType == tokenType).FirstOrDefault();
        }
    }
}