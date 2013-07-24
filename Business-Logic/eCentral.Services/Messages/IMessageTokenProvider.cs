using System.Collections.Generic;
using eCentral.Core.Domain.Users;

namespace eCentral.Services.Messages
{
    public partial interface IMessageTokenProvider
    {
        void AddDefaultTokens(IList<Token> tokens);

        void AddUserTokens(IList<Token> tokens, User user);

        void AddMasterTokens(IList<Token> tokens, string messageBody);

        string[] GetListOfAllowedTokens();
    }
}
