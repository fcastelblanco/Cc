using System;
using Cc.Upt.Data.Definitions;
using Cc.Upt.Domain;
using Cc.Upt.Domain.Enumerations;

namespace Cc.Upt.Business.Definitions
{
    public interface IUserTokenService : IEntityService<UserToken>
    {
        bool Save(UserToken userToken);
        bool IsValidToken(Guid token, TokenType tokenType);
        bool DeleteAllUserTokenByTokenType(Guid userId, TokenType tokenType);
        UserToken GetUserTokenByTokenAndTokenType(Guid token, TokenType tokenType);
    }
}