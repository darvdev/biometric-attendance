using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiometricAttendance
{
    public class ModelEmployee
    {
        public int id { get; set; }
        public int? biometric_id { get; set; }
        public string employee_id { get; set; }
        public string first_name { get; set; }
        public string? middle_name { get; set; }
        public string last_name { get; set; }
        public string? img_base64 { get; set; }
        public string? username { get; set; }
        public string? password { get; set; }

        public string name
        {
            get
            {
                return $"{first_name} {last_name}";
            }
        }
    }

    public class ModelAttendance
    {
        public int id { get; set; }
        public string employee_id { get; set; }
        public string name { get; set; }
        public string date { get; set; }
    }

}
