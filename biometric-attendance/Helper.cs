using Dapper;
using System;
using System.Data.SQLite;
using System.Data;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO.Ports;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Drawing;

namespace BiometricAttendance
{
    public class Helper
    {
        public static void NumericKeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        public static string[] GetPorts()
        {
            return SerialPort.GetPortNames();
        }

        public static ModelEmployee[] GetEmployeeList(string connectionString)
        {
            try
            {
                using IDbConnection con = new SQLiteConnection(connectionString);
                var output = con.Query<ModelEmployee>("select * from employees", new DynamicParameters());
                con.Close();
                con.Dispose();
                return output.ToArray();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Helper.GetEmployeeList error: {0}", ex.Message);
            }
            
            return Array.Empty<ModelEmployee>();
        }

        public static bool AddEmployee(string connectionString, ModelEmployee employee) 
        {
            try
            {

                using (IDbConnection con = new SQLiteConnection(connectionString))
                {
                    con.Execute("insert into employees (employee_id, first_name, middle_name, last_name, biometric_id, username, password) values (@employee_id, @first_name, @middle_name, @last_name, @biometric_id, @username, @password)", employee);
                    con.Close();
                    con.Dispose();
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Helper.AddEmployee error: {0}", ex.Message);
            }

            return false;
        }

        public static bool EnrollEmployee(string connectionString, int databaseId, int biometricId, string employeeId)
        {
            try
            {
                using IDbConnection con = new SQLiteConnection(connectionString);
                var output = con.Query<ModelEmployee>("update employees set biometric_id = @biometric_id where id = @id", new { biometric_id = biometricId, id = databaseId });
                con.Close();
                con.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Helper.EnrollEmployee error: {0}", ex.Message);
            }

            return false;

        }

        public static ModelAttendance[] GetAttendaceList(string connectionString)
        {
            try
            {
                using IDbConnection con = new SQLiteConnection(connectionString);
                var output = con.Query<ModelAttendance>("select * from attendance", new DynamicParameters());
                con.Close();
                con.Dispose();
                return output.ToArray();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Helper.GetAttendaceList error: {0}", ex.Message);
            }

            return Array.Empty<ModelAttendance>();
        }
        public static ModelAttendance AddAttendance(string connectionString, int biometricId)
        {
            try
            {
                using IDbConnection con = new SQLiteConnection(connectionString);
                var list = con.Query<ModelEmployee>("select * from employees where biometric_id = @biometric_id", new { biometric_id = biometricId});

                ModelAttendance attendance = null;

                if (list.Count() > 0)
                {
                    var ee = list.ToArray()[0];
                    Console.WriteLine("Employee: {0}", ee.name);

                    var dt = DateTime.Now;

                    attendance = new ModelAttendance
                    {
                        employee_id = ee.employee_id,
                        name = ee.name,
                        date = dt.ToString(),                        
                    };

                    con.Execute("insert into attendance (employee_id, name, date) values (@employee_id, @name, @date)", attendance);

                }

                con.Close();
                con.Dispose();

                
                return attendance;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Helper.AddAttendance error: {0}", ex.Message);
            }

            return null;

        }


    }
}
