using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatDomain.Models
{
    public record UserConnection(string userName, Guid chatId);
}
