using System;
using Isn.Upt.Data.Definitions;
using Isn.Upt.Domain;
using Isn.Upt.Domain.Enumerations;

namespace Isn.Upt.Business.Definitions
{
    public interface IUserTokenService : IEntityService<UserToken>
    {
        bool Save(UserToken userToken);
        bool IsValidToken(Guid token, TokenType tokenType);
        bool DeleteAllUserTokenByTokenType(Guid userId, TokenType tokenType);
        UserToken GetUserTokenByTokenAndTokenType(Guid token, TokenType tokenType);
    }
}