using System;
using System.Collections.Generic;
using System.Text;

namespace THD.Core.Api.Helpers
{
    public class GenerateToken
    {
        public static string GetGuid()
        {
            Guid guid = Guid.NewGuid();
            return guid.ToString();
        }
        public static string GetToken()
        {
            Guid guid = Guid.NewGuid();
            string token = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(
                "{" + guid.ToString() + DateTime.Now.ToString("yyyyMMddHHmmss") + "}"
                ));
            return token;
        }

        public static string GetPassword(string passw)
        {
            return System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(passw));
        }
    }
}
