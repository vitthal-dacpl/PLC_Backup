using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Backup_Config
{
    public partial class Plc_Backup_Configuration : Form
    {
        public Plc_Backup_Configuration()
        {
            InitializeComponent();
        }

        //private void Submit_Button_Click(object sender, EventArgs e)
        //{
        //    string Sourcepath = Source_Path.Text.Trim();
        //    string DestinationPath = Destination_Path.Text.Trim();

        //    if (!IsValidPath(Sourcepath))
        //    {
        //        MessageBox.Show("Invalid Source Path!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        return;
        //    }

        //    if (!IsValidPath(DestinationPath))
        //    {
        //        MessageBox.Show("Invalid Destination Path!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        return;
        //    }

        //    string SourceTrimPath = CleanPath(Sourcepath);
        //    string DestinationTrimPath = CleanPath(DestinationPath);

        //    string filePath = ConfigurationManager.AppSettings["JsonFilePath"]; // Path from App.config

        //    var jsonData = new
        //    {
        //        SourcePath = SourceTrimPath,
        //        DestinationPath = DestinationTrimPath
        //    };

        //    try
        //    {
        //        string directoryPath = Path.GetDirectoryName(filePath);
        //        if (Directory.Exists(directoryPath))
        //        {
        //            foreach (string file in Directory.GetFiles(directoryPath, "*.json"))
        //            {
        //                File.Delete(file);
        //            }
        //        }


        //        string jsonString = JsonSerializer.Serialize(jsonData, new JsonSerializerOptions { WriteIndented = true });
        //        File.WriteAllText(filePath, jsonString);

        //        MessageBox.Show("File created successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}

        private void Submit_Button_Click(object sender, EventArgs e)
        {
            string Sourcepath = Source_Path.Text.Trim();
            string DestinationPath = Destination_Path.Text.Trim();

            if (!IsValidPath(Sourcepath))
            {
                MessageBox.Show("Invalid Source Path!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!IsValidPath(DestinationPath))
            {
                MessageBox.Show("Invalid Destination Path!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string SourceTrimPath = CleanPath(Sourcepath);
            string DestinationTrimPath = CleanPath(DestinationPath);

            if (string.IsNullOrWhiteSpace(SourceTrimPath) || string.IsNullOrWhiteSpace(DestinationTrimPath))
            {
                MessageBox.Show("Both Source and Destination paths must be valid.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string filePath = ConfigurationManager.AppSettings["JsonFilePath"];
            if (string.IsNullOrWhiteSpace(filePath))
            {
                MessageBox.Show("JSON file path is not configured properly.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var jsonData = new
            {
                SourcePath = SourceTrimPath,
                DestinationPath = DestinationTrimPath
            };

            try
            {
                string directoryPath = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directoryPath) && Directory.Exists(directoryPath))
                {
                    try
                    {
                        foreach (string file in Directory.GetFiles(directoryPath, "*.json"))
                        {
                            File.Delete(file);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error deleting old JSON files: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                string jsonString = JsonSerializer.Serialize(jsonData, new JsonSerializerOptions { WriteIndented = true });

                try
                {
                    File.WriteAllText(filePath, jsonString);
                    MessageBox.Show("File created successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (UnauthorizedAccessException)
                {
                    MessageBox.Show("You do not have permission to write to this location.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (IOException ioEx)
                {
                    MessageBox.Show("File writing error: " + ioEx.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unexpected error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unexpected error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static bool IsValidPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return false;

            // Regex pattern for Windows file/folder path
            string pattern = @"^(?:[a-zA-Z]:\\|\\[\w\s-]+\\[\w\s-]+)(?:[\w\s-\\]+)*$";

            // Check if path matches Windows format
            if (!Regex.IsMatch(path, pattern))
                return false;

            // Check for invalid path characters
            char[] invalidChars = Path.GetInvalidPathChars();
            if (path.Any(c => invalidChars.Contains(c)))
                return false;

            return true;
        }
        public static string CleanPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return string.Empty;

            // Remove extra slashes at the end
            return Regex.Replace(path, @"[\\/]+$", "");
        }
        private void Plc_Backup_Configuration_Load(object sender, EventArgs e)
        {
            ManagementObjectSearcher MOS = new ManagementObjectSearcher("Select * From Win32_BaseBoard");
            string motherbord = "";
            foreach (ManagementObject getserial in MOS.Get())
            {
                motherbord = getserial["SerialNumber"].ToString();
            }
            // WriteToFile("SerialNumber = " + motherbord);

            
            string StaticNumber = "9189-8548-1821-2304-9503-6280-34";
            if (StaticNumber == motherbord)
            {
                

            }
            else
            {
                MessageBox.Show("Unauthorized device Invalide License key", "Access Denied");
                this.Close();
            }
        }
    }
}
