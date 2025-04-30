using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace IRepositoryService
{
    public interface IGrokService
    {
        Task<JsonObject> SendMessage(List<string> inputmessage, List<string> inputrole);
        void VerifiyMessageSize(List<string> inputmessage, List<string> inputroles
                  , int limit = 15000);

    }
}
