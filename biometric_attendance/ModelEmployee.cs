namespace biometric_attendance
{
    public class ModelEmployee
    {
        public int id { get; set; }
        public string finger_id { get; set; }
        public string employee_id { get; set; }
        public string first_name { get; set; }
        public string middle_name { get; set; }
        public string last_name { get; set; }

        public string fullName
        {
            get 
            {
                return $"{first_name} {last_name}";
            }
        }
    }
}
