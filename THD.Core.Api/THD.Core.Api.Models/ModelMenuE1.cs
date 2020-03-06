using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace THD.Core.Api.Models
{
    public class ModelMenuE1_InterfaceData
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public DateTime docDate { get; set; }
        public string docNumber { get; set; }

        public IList<ModelSelectOption> listfaculty { get; set; }
        public ModelPermissionPage UserPermission { get; set; }
    }

    public class ModelMenuE1
    {
        public int docId { get; set; }
        public DateTime docDate { get; set; }
        public string docNumber { get; set; }
        public string sectionName { get; set; }
        public int faculty { get; set; }
        public string departmentName { get; set; }
        public string phone { get; set; }
        public string fax { get; set; }
        public string email { get; set; }
        public string group1genus { get; set; }
        public string group1species { get; set; }
        public int group1riskHuman { get; set; }
        public int group1riskAnimal { get; set; }
        public int group1pathogens { get; set; }
        public string group1virus { get; set; }
        public string group1bacteria { get; set; }
        public string group1paraSit { get; set; }
        public string group1mold { get; set; }
        public string group1protein { get; set; }
        public string group2genus { get; set; }
        public string group2species { get; set; }
        public int group2riskHuman { get; set; }
        public int group2riskAnimal { get; set; }
        public int group2pathogens { get; set; }
        public string group2virus { get; set; }
        public string group2bacteria { get; set; }
        public string group2paraSit { get; set; }
        public string group2mold { get; set; }
        public string group2protein { get; set; }
        public bool editenable { get; set; }

    }


    public class ModelMenuE1_InterfaceReportData
    {
        public string docnumber { get; set; }
        public string sectionname { get; set; }
        public string faculty { get; set; }

        public string group1riskhuman { get; set; }
        public string group1riskanimal { get; set; }
        public string group1pathogens { get; set; }

        public string group2riskhuman { get; set; }
        public string group2riskanimal { get; set; }
        public string group2pathogens { get; set; }
        public IList<ModelSelectOption> listfaculty { get; set; }
        public IList<ModelMenuE1Report> listreportdata { get; set; }
    }

    public class ModelMenuE1Report
    {
        public int docId { get; set; }
        public string docDate { get; set; }
        public string docNumber { get; set; }
        public string sectionName { get; set; }
        public string facultyName { get; set; }
        public string departmentName { get; set; }
        public string phone { get; set; }
        public string fax { get; set; }
        public string email { get; set; }
        public string group1genus { get; set; }
        public string group1species { get; set; }
        public string group1riskHumanName { get; set; }
        public string group1riskAnimalName { get; set; }
        public string group1pathogensName { get; set; }
        public string group1virus { get; set; }
        public string group1bacteria { get; set; }
        public string group1paraSit { get; set; }
        public string group1mold { get; set; }
        public string group1protein { get; set; }
        public string group2genus { get; set; }
        public string group2species { get; set; }
        public string group2riskHumanName { get; set; }
        public string group2riskAnimalName { get; set; }
        public string group2pathogensName { get; set; }
        public string group2virus { get; set; }
        public string group2bacteria { get; set; }
        public string group2paraSit { get; set; }
        public string group2mold { get; set; }
        public string group2protein { get; set; }

    }

}
