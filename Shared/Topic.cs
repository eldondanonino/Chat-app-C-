using System;
using System.Collections.Generic;
using System.Text;

namespace Shared
{
    [Serializable]
    public class Topic
    {
        public string _name;
        public int _id;
        public List<Message> _messages;

        public Topic (int id, string name, List<Message> messages)
            {
            _id = id;
            _name = name;
            _messages = messages;
            }
    }
}
