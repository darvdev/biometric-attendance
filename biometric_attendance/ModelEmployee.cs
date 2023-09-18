#nullable enable
using System;

namespace biometric_attendance
{
    public class ModelEmployee
    {
        public int id { get; set; }
        public int? biometric_id { get; set; }
        public string employee_id { get; set; }
        public string first_name { get; set; }
        public string? middle_name { get; set; }
        public string last_name { get; set; }

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
}
