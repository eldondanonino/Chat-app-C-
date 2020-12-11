using System;
using System.Collections.Generic;
using System.Text;

namespace Shared
{
    public class Topic
    {
        public string _name;
        public int _id;

        public Topic (int id, string name)
            {
            _id = id;
            _name = name;
            }
    }
}
