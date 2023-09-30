using PdfSharp;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Windows.Forms;
using TheArtOfDev.HtmlRenderer.PdfSharp;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace BiometricAttendance
{
    public partial class FormReports : Form
    {
        public FormReports()
        {
            InitializeComponent();
        }

        private FormMain formMain = (FormMain)Application.OpenForms["FormMain"];

        private void FormReports_Load(object sender, EventArgs e)
        {
            comboBoxSection.SelectedIndex = 0;
            comboBoxClass.SelectedIndex = 0;
            comboBoxType.SelectedIndex = 0;
            dateTimePickerStart.Value = DateTime.Parse(DateTime.Now.ToShortDateString() + " 12:00:00 am");
            dateTimePickerEnd.Value = DateTime.Parse(DateTime.Now.ToShortDateString() + " 11:59:59 pm");

        }

        private void ButtonGenerate_Click(object sender, EventArgs e)
        {            
            buttonGenerate.Enabled = false;

            if (dateTimePickerStart.Value > dateTimePickerEnd.Value)
            {
                MessageBox.Show("From date is greater than To date.", "Date Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }




            var rawDays = (dateTimePickerEnd.Value - dateTimePickerStart.Value).TotalDays;
            var days = Math.Round(rawDays, MidpointRounding.AwayFromZero);


            PdfDocument finalPdf = new PdfDocument();
            
            DateTime date = dateTimePickerStart.Value;

            for (int x = 0; x < days; x++)
            {
                if (date > dateTimePickerEnd.Value)
                {
                    break;
                }

                Console.WriteLine("Date: {0}", date);

                string sheetTitle = "Daily Attendance Sheet";

                string header = "" +
                    $"<h2><center>{sheetTitle}</center></h2><br/>" +
                    $"<div>Section: <b>{comboBoxSection.Text}</b></div>" +
                    $"<div>Class: <b>{comboBoxClass.Text}</b></div>" +
                    $"<div>Date: <b>{date:yyyy-MM-dd}</b></div><br/>";


                ModelAttendance[] attendanceToday = Array.FindAll(formMain.attendaceList, (a) => {
                    var dt = DateTime.Parse(a.date);
                    return date.Year == dt.Year && date.Month == dt.Month && date.Day == dt.Day;
                });


                IDictionary<string, List<ModelAttendance>> attendance = new Dictionary<string, List<ModelAttendance>>();

                foreach (var att in attendanceToday)
                {
                    if (!attendance.ContainsKey(att.student_id))
                    {
                        attendance.Add(att.student_id, new List<ModelAttendance>());
                    }
                    attendance[att.student_id].Add(att);
                }

                int max = attendance.Count == 0 ? 4 : attendance.Values.Max(e => e.Count);

                Console.WriteLine("Max: {0}", max);

                if (max < 4)
                {
                    max = 4;
                }
                
                string th = "";

                for (int i = 0; i < max; i++)
                {
                    th += @"<th class=""date head"">" + (i % 2 == 0 ? "TIME IN" : "TIME OUT") + "</th>";
                }

                var tableColumn = $@"<tr><th class=""id head"">ID</th><th class=""name head"">NAME</th>{th}</tr>";


                string tableRow = "";
                foreach (KeyValuePair<string, List<ModelAttendance>> entry in attendance)
                {
                    var td = $@"<td class=""id"">{entry.Value.First().student_id}</td><td class=""name"">{entry.Value.First().name}</td>";

                    int count = 0;
                    foreach (var val in entry.Value)
                    {
                        count++;
                        td += $@"<td class=""date"">{DateTime.Parse(val.date).ToShortTimeString()}</td>";
                    }

                    if (count < max)
                    {
                        for (int i = count; i < max; i++)
                        {
                            td += @"<td class=""date""></td>";
                        }
                    }

                    tableRow += $"<tr>{td}</tr>";
                }


                string end = "<br><div><center><b>* * * * * Nothing Follows * * * * *<b><center></div>";

                string table = $"<table>{tableColumn}{tableRow}</table>";
                PdfDocument pdf = PdfGenerator.GeneratePdf(style + header + table + end, PageSize.A4, 50);

                var openPdf = GetPdfDoc(pdf);
                pdf.Close();
                pdf.Dispose();
                finalPdf.Pages.Add(openPdf.Pages[0]);

                date = date.AddDays(1);
            }

            var today = DateTime.Now;

            var path = $"Report_{today:yyyy-MM-dd_HH-mm-ss}.pdf";

            finalPdf.Save(path);
            finalPdf.Close();
            finalPdf.Dispose();
            Process.Start(path);
            buttonGenerate.Enabled = true;

        }

        string style = @"
                <style>
                    h2 { padding: 0px; margin: 0px; }
                    table { border-collapse: collapse; width: 100%; padding: 0px; margin: 0px; }
                    tr { padding: 0px; margin: 0px; }
                    th, td { border: 1px solid #dddddd; text-align: left; font-size: 12px; padding: 4px; margin: 0px; }
                    th.head {background-color: #dddddd; border: 1px solid white}
                    .date {width: 55px; text-align: center;}
                    .id {width: 60px; text-align: center;}
                    .name {padding-left: 5px;}
                </style>
                ";

        private static PdfDocument GetPdfDoc(PdfDocument pdf)
        {
            var stream = new MemoryStream();
            pdf.Save(stream, false);
            stream.Position = 0;
            var result = PdfReader.Open(stream, PdfDocumentOpenMode.Import);
            stream.Close();
            stream.Dispose();
            return result;
        }
    }
}
