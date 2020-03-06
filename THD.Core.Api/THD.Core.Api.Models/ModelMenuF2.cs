using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace THD.Core.Api.Models
{
    public class ModelMenuF2_InterfaceData
    {
        public string usergroup { get; set; }
        public IList<ModelMenuF2Report> listdata { get; set; }
        public ModelPermissionPage UserPermission { get; set; }
    }

    public class ModelMenuF2Report
    {
        public string usergroupname { get; set; }
        public string menupagecode { get; set; }
        public string menupagename { get; set; }
        public string pmview { get; set; }
        public string pminsert { get; set; }
        public string pmupdate { get; set; }
        public string pmprint { get; set; }
        public string pmalldata { get; set; }
        public string isactive { get; set; }
    }

    public class ModelMenuF2Edit
    {
        public string usergroup { get; set; }
        public string menupagecode { get; set; }
        public bool pmview { get; set; }
        public bool pminsert { get; set; }
        public bool pmupdate { get; set; }
        public bool pmprint { get; set; }
        public bool pmalldata { get; set; }
    }


}
