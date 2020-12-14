using System;
using System.Collections.Generic;
using System.Text;

namespace Shared
{
    [Serializable]
    public class PrivateMessage
    {
        public string un1, un2;
        public List<Message> pm;

        public PrivateMessage(string first, string second, List<Message> list)
        {
            un1 = first;
            un2 = second;
            pm = list;
        }

    }
}
