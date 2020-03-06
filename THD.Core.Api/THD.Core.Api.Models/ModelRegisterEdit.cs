using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace THD.Core.Api.Models
{

    public class ModelMenuF1Edit_InterfaceData
    {
        public IList<ModelSelectOption> listfirstname { get; set; }
        public IList<ModelSelectOption> listfaculty { get; set; }
        public ModelRegisterEdit UserData { get; set; }
        public ModelPermissionPage UserPermission { get; set; }
    }
    public class ModelRegisterEdit
    {
        [Required(ErrorMessage = "RegisterId is required.")]
        public string registerid { get; set; }
        public string userid { get; set; }
        public string passw { get; set; }
        public string confirmpassw { get; set; }
        public string email { get; set; }

        public string firstname1 { get; set; }
        public string firstname2 { get; set; }
        //[Required(ErrorMessage = "first name is required.")]
        public string firstname { get; set; }
        [Required(ErrorMessage = "full name is required.")]
        public string fullname { get; set; }
        [Required(ErrorMessage = "faculty is required.")]
        public string faculty { get; set; }
        public string facultyname { get; set; }
        [Required(ErrorMessage = "position is required.")]
        public string position { get; set; }
        public string positionname { get; set; }
        public string workphone { get; set; }
        public string mobile { get; set; }
        public string fax { get; set; }
        [Required(ErrorMessage = "education is required.")]
        public string education { get; set; }
        public string educationname { get; set; }
        [Required(ErrorMessage = "character is required.")]
        public string character { get; set; }
        public string charactername { get; set; }
        public string note1 { get; set; }
        public string note2 { get; set; }
        public string note3 { get; set; }
        public bool isactive { get; set; }
        public DateTime registerdate { get; set; }
        public DateTime registerexpire { get; set; }
    }

    public class ModelUpdateAccountUser
    {
        [Required(ErrorMessage = "RegisterId is required.")]
        public string registerid { get; set; }
        public string userid { get; set; }
        public string passw { get; set; }
        public string confirmpassw { get; set; }
        public string email { get; set; }
        public string firstname1 { get; set; }
        public string firstname2 { get; set; }
        //[Required(ErrorMessage = "first name is required.")]
        public string firstname { get; set; }
        [Required(ErrorMessage = "full name is required.")]
        public string fullname { get; set; }
        [Required(ErrorMessage = "faculty is required.")]
        public string faculty { get; set; }
        public string facultyname { get; set; }
        [Required(ErrorMessage = "position is required.")]
        public string position { get; set; }
        public string positionname { get; set; }
        public string workphone { get; set; }
        public string mobile { get; set; }
        public string fax { get; set; }
        [Required(ErrorMessage = "education is required.")]
        public string education { get; set; }
        public string educationname { get; set; }
        public string character { get; set; }
        public string charactername { get; set; }
        public string note1 { get; set; }
        public string note2 { get; set; }
        public string note3 { get; set; }
    }

    public class ModelResetPassword
    {
        [Required(ErrorMessage = "RegisterId is required.")]
        public string registerid { get; set; }
        [Required(ErrorMessage = "Old Password is required.")]
        public string oldpassw { get; set; }
        [Required(ErrorMessage = "New Password is required.")]
        public string newpassw { get; set; }
        [Required(ErrorMessage = "Confirm Password is required.")]
        public string confirmpassw { get; set; }
    }


}
