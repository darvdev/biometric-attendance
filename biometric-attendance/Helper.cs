using Dapper;
using System;
using System.Data.SQLite;
using System.Data;
using System.Windows.Forms;
using System.Linq;
using System.Threading.Tasks;
using System.IO.Ports;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Configuration;

namespace BiometricAttendance
{
    public class Helper
    {
        public static void NumericKeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }
        private static readonly string conString = ConfigurationManager.ConnectionStrings["default"].ConnectionString;
        public static string[] GetPorts()
        {
            return SerialPort.GetPortNames();
        }

        public static async Task<ModelEmployee[]>  GetEmployeeList()
        {
            try
            {
                using IDbConnection con = new SQLiteConnection(conString);
                var output = await con.QueryAsync<ModelEmployee>("SELECT * FROM employees", new DynamicParameters());
                con.Close();
                con.Dispose();
                return output.ToArray();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Helper.GetEmployeeList Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
            return Array.Empty<ModelEmployee>();
        }

        public static async Task<bool> AddEmployee(ModelEmployee employee) 
        {
            try
            {
                using IDbConnection con = new SQLiteConnection(conString);
                await con.ExecuteAsync("INSERT INTO employees (employee_id, first_name, middle_name, last_name, img_base64, biometric_id, username, password) VALUES (@employee_id, @first_name, @middle_name, @last_name, @img_base64, @biometric_id, @username, @password)", employee);
                con.Close();
                con.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Helper.AddEmployee Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return false;
        }
        /// <summary>
        /// /////////////////////////
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        public static async Task<bool> UpdateEmployee(ModelEmployee employee)
        {
            try
            {
                using IDbConnection con = new SQLiteConnection(conString);
                var result = await con.ExecuteAsync("UPDATE employees SET employee_id = @employee_id, first_name = @first_name, middle_name = @middle_name, last_name = @last_name, img_base64 = @img_base64 WHERE id = @id", employee);
                con.Close();
                con.Dispose();
                Console.Write("Update result: {0}", result);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Helper.UpdateEmployee Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return false;
        }

        public static async Task<bool> EnrollEmployee(int databaseId, int biometricId)
        {
            try
            {
                using IDbConnection con = new SQLiteConnection(conString);
                var output = await con.QueryAsync<ModelEmployee>("UPDATE employees SET biometric_id = @biometric_id WHERE id = @id", new { biometric_id = biometricId, id = databaseId });
                con.Close();
                con.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Helper.EnrollEmployee Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return false;
        }

        public static async Task<bool> DeleteEmployee(ModelEmployee employee) 
        {
            try
            {
                using (IDbConnection con = new SQLiteConnection(conString))
                {
                    await con.ExecuteAsync("DELETE FROM employees WHERE id = @id", employee);
                    con.Close();
                    con.Dispose();
                }
  
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Helper.DeleteEmployee Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return false;
        }
        public static async Task<ModelAttendance[]> GetAttendaceList()
        {
            try
            {
                using IDbConnection con = new SQLiteConnection(conString);
                var output = await con.QueryAsync<ModelAttendance>("SELECT * FROM attendance", new DynamicParameters());
                con.Close();
                con.Dispose();
                return output.ToArray();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Helper.GetAttendaceList Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return Array.Empty<ModelAttendance>();
        }
        
        public static async Task<ModelAttendance> AddAttendance(int biometricId)
        {
            try
            {
                using IDbConnection con = new SQLiteConnection(conString);
                var list = await con.QueryAsync<ModelEmployee>("SELECT * FROM employees WHERE biometric_id = @biometric_id", new { biometric_id = biometricId});

                ModelAttendance attendance = null;

                if (list.Count() > 0)
                {
                    var ee = list.ToArray()[0];
                    var dt = DateTime.Now;

                    attendance = new ModelAttendance
                    {
                        employee_id = ee.employee_id,
                        name = ee.name,
                        date = dt.ToString(),                        
                    };

                    await con.ExecuteAsync("INSERT INTO attendance (employee_id, name, date) VALUES (@employee_id, @name, @date)", attendance);
                }

                con.Close();
                con.Dispose();
                
                return attendance;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Helper.AddAttendance Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return null;

        }

        public static Image BrowseImage()
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp)|*.jpg; *.jpeg; *.gif; *.bmp; *.png";
            if (open.ShowDialog() == DialogResult.OK)
            {
                return ResizeImage(new Bitmap(open.FileName));
            }

            return null;
        }

        public static Image ResizeImage(Image image)
        {
            return (Image)(new Bitmap(image, new Size(200, 200)));
        }

        public static ModelEmployee NewEmployee(string employeeId, string firstName, string lastName, string middleName = null, Image image = null, string biometricId = null, string username = null, string password = null)
        {
            try
            {
                return new ModelEmployee
                {
                    employee_id = employeeId == string.Empty ? null : employeeId,
                    first_name = firstName == string.Empty ? null : firstName,
                    last_name = lastName == string.Empty ? null : lastName,

                    middle_name = middleName == string.Empty ? null : middleName,
                    img_base64 = ImageToBase64String(image),
                    biometric_id = biometricId == null ? null : TryParseNullable(biometricId),
                    username = username == string.Empty ? null : username,
                    password = password == string.Empty ? null : password,
                };

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return null;
        }

        public static Image GetImageFromBase64String(string base64)
        {
            if (base64 == null) return null;

            try
            {
                //data:image/gif;base64,
                //this image is a single pixel (black)
                byte[] bytes = Convert.FromBase64String(base64);

                Image image;
                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    image = Image.FromStream(ms);
                }

                return image;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Helper.GetImageFromBase64String error: {0}", ex.Message);
            }

            return null;
        }

        private static int? TryParseNullable(string val)
        {
            int outValue;
            return int.TryParse(val, out outValue) ? (int?)outValue : null;
        }

        private static string ImageToBase64String(Image image)
        {
            if (image != null)
            {
                Bitmap tmpImg = new Bitmap(image);

                MemoryStream ms = new MemoryStream();
                tmpImg.Save(ms, ImageFormat.Jpeg);
                tmpImg.Dispose();
                image.Dispose();
                byte[] imageByte = ms.ToArray();
                var base64 = Convert.ToBase64String(imageByte);
                return base64;
            }
            return null;
        }

    }
}
