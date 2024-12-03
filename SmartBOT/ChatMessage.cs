using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBOT
{
    public struct ChatMessage
    {
        public string Role { get; set; } // Pode ser "system", "user" ou "assistant"
        public string Content { get; set; } // Conteúdo da mensagem
    }

}
