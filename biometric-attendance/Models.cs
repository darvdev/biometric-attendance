﻿using System.Drawing;

namespace BiometricAttendance
{
    public class ModelStudent
    {
        public int id { get; set; }
        public int? biometric_id { get; set; }
        public string student_id { get; set; }
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

        public Image image
        {
            get
            {
                return Helper.GetImageFromBase64String(img_base64);
            }
        }
    }

    public class ModelAttendance
    {
        public int id { get; set; }
        public string student_id { get; set; }
        public string name { get; set; }
        public string date { get; set; }
    }

}
