using System;
using System.Collections.Generic;
using System.Text;

namespace Shared
{
    [Serializable]
    public class Message
    {
        string _un;
        string _con;

        public Message(string username, string content)
        {
            _un = username;
            _con = content;
        }
    }
}
