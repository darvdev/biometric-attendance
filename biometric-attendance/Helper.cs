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
using System.Speech.Synthesis;

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

        public static async Task<ModelStudent[]>  GetStudentList()
        {
            try
            {
                using IDbConnection con = new SQLiteConnection(conString);
                var output = await con.QueryAsync<ModelStudent>("SELECT * FROM students", new DynamicParameters());
                con.Close();
                con.Dispose();
                return output.ToArray();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Helper.GetStudentList Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
            return Array.Empty<ModelStudent>();
        }

        public static async Task<bool> AddStudent(ModelStudent student) 
        {
            try
            {
                using IDbConnection con = new SQLiteConnection(conString);
                await con.ExecuteAsync("INSERT INTO students (student_id, first_name, middle_name, last_name, img_base64, biometric_id, username, password) VALUES (@student_id, @first_name, @middle_name, @last_name, @img_base64, @biometric_id, @username, @password)", student);
                con.Close();
                con.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Helper.AddStudent Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return false;
        }
  
        public static async Task<bool> UpdateStudent(ModelStudent student)
        {
            try
            {
                using IDbConnection con = new SQLiteConnection(conString);
                var result = await con.ExecuteAsync("UPDATE students SET student_id = @student_id, first_name = @first_name, middle_name = @middle_name, last_name = @last_name, img_base64 = @img_base64 WHERE id = @id", student);
                con.Close();
                con.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Helper.UpdateStudent Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return false;
        }

        public static async Task<bool> EnrollStudent(int databaseId, int biometricId)
        {
            try
            {
                using IDbConnection con = new SQLiteConnection(conString);
                var output = await con.QueryAsync<ModelStudent>("UPDATE students SET biometric_id = @biometric_id WHERE id = @id", new { biometric_id = biometricId, id = databaseId });
                con.Close();
                con.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Helper.EnrollStudent Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return false;
        }

        public static async Task<bool> DeleteStudent(ModelStudent student) 
        {
            try
            {
                using (IDbConnection con = new SQLiteConnection(conString))
                {
                    await con.ExecuteAsync("DELETE FROM students WHERE id = @id", student);
                    con.Close();
                    con.Dispose();
                }
  
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Helper.DeleteStudent Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                var list = await con.QueryAsync<ModelStudent>("SELECT * FROM students WHERE biometric_id = @biometric_id", new { biometric_id = biometricId});

                ModelAttendance attendance = null;

                if (list.Count() > 0)
                {
                    var student = list.ToArray()[0];
                    var dt = DateTime.Now;

                    attendance = new ModelAttendance
                    {
                        student_id = student.student_id,
                        name = student.name,
                        date = dt.ToString(),                        
                    };

                    await con.ExecuteAsync("INSERT INTO attendance (student_id, name, date) VALUES (@student_id, @name, @date)", attendance);
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

        public static ModelStudent NewStudent(string studentId, string firstName, string lastName, string middleName = null, Image image = null, string biometricId = null, string username = null, string password = null)
        {
            try
            {
                return new ModelStudent
                {
                    student_id = studentId == string.Empty ? null : studentId,
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
                MessageBox.Show(ex.Message, "Helper.NewStudent Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        public static void Speak(string text)
        {
            SpeechSynthesizer synthesizer = new SpeechSynthesizer();
            synthesizer.SelectVoiceByHints(VoiceGender.Female, VoiceAge.Teen);
            synthesizer.Volume = 100;  // 0...100
            synthesizer.Rate = 0;     // -10...10

            synthesizer.Speak(text); // Synchronous

            //synthesizer.SpeakAsync(text); // Asynchronous
            synthesizer.Dispose();
        }

        public static void PlayBeep()
        {
            //Stream stream = new MemoryStream(Properties.Resources.sensor_beep);
            Stream stream = Properties.Resources.sensor_beep;
            System.Media.SoundPlayer sound = new System.Media.SoundPlayer(stream);
            sound.Play();
            sound.Dispose();

            //System.Media.SoundPlayer player = new System.Media.SoundPlayer();
            //player.SoundLocation = path;
            //player.Play();
            //player.Dispose();
        }

        public static void PlayOk()
        {
            Stream stream = Properties.Resources.finger_ok;
            System.Media.SoundPlayer sound = new System.Media.SoundPlayer(stream);
            sound.Play();
            sound.Dispose();
        }

        public static void PlayError()
        {
            Stream stream = Properties.Resources.finger_error;
            System.Media.SoundPlayer sound = new System.Media.SoundPlayer(stream);
            sound.Play();
            sound.Dispose();
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
