using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProfileManagementSystem.Models
{

    public class profileUser
    {
        public string ID { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string designation { get; set; }
        public string empno { get; set; }
        public string role { get; set; }
        public int status { get; set; }
        public int display { get; set; }
        public string photo { get; set; }
        public string lastName { get; set; }

    }
}