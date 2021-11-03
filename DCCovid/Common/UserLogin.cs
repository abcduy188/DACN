using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DCCovid.Common
{
    [Serializable]
    //lưu trữ dữ liệu
    public class UserLogin
    {
        public long UserID { set; get; } //các hàm properties
        public string Email { set; get; }
        public string Name { set; get; }
        public bool Status { set; get; }
    }
}