using System;
using System.Collections.Generic;
using System.Text;

namespace Shared
{
    [Serializable]
    public class Message
    {
        public string _un;
        public string _con;

        public Message(string username, string content)
        {
            _un = username;
            _con = content;
        }
    }
}
