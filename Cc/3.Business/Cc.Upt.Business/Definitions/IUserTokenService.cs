using System;
using Cc.Upt.Domain;
using Cc.Upt.Domain.Enumerations;
using Cc.Upt.DomainRepository.Definitions;

namespace Cc.Upt.Business.Definitions
{
    public interface IUserTokenService : IRepository<UserToken>
    {
        bool Save(UserToken userToken);
        bool IsValidToken(Guid token, TokenType tokenType);
        bool DeleteAllUserTokenByTokenType(Guid userId, TokenType tokenType);
        UserToken GetUserTokenByTokenAndTokenType(Guid token, TokenType tokenType);
    }
}