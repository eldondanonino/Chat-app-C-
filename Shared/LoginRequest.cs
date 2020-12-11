using System;
using System.Collections.Generic;
using System.Text;

namespace Shared
{
    [Serializable]
    public class LoginRequest
    {

        public string _un;
        public string _pwd;

        public LoginRequest(string un, string pwd)
        {
            _un = un;
            _pwd = pwd;
        }

    }
}
