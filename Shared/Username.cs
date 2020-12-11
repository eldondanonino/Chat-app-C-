using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Shared
{
    [Serializable]
    public class Username
    {
        public int _id;
        public string _un;
        public string _pwd;
        public bool _log;
        
        public Username(int id, string un, string pwd, bool log)
        {
            _id = id;
            _un = un;
            _pwd = pwd;
            _log = log;
        }
        /*
        public int ID
        {
            get { return _id; }
        }
        public string UN
        {
            get { return _un; }
            set { _un = value;}
        }
        */


    }
}
