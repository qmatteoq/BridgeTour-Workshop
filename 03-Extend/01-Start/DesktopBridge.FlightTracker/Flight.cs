﻿using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace DesktopBridge.FlightTracker
{
    public partial class Flight : Form
    {
        private RegistryKey _regKey;
        public Flight()
        {
            InitializeComponent();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            _regKey.SetValue("Code", codeTextbox.Text);
            _regKey.SetValue("Date", dateTextbox.Text);
            _regKey.SetValue("Departure", departureTextbox.Text);
            _regKey.SetValue("Arrival", arrivalTextbox.Text);
            operationStatusLabel.Text = "The flight has been saved";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _regKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\DesktopBridgeWorkshop\Demo", true);
            if (_regKey == null)
            {
                _regKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Microsoft\DesktopBridgeWorkshop\Demo", RegistryKeyPermissionCheck.ReadWriteSubTree);
            }

            codeTextbox.Text = (string)_regKey.GetValue("Code");
            dateTextbox.Text = (string)_regKey.GetValue("Date");
            departureTextbox.Text = (string)_regKey.GetValue("Departure");
            arrivalTextbox.Text = (string)_regKey.GetValue("Arrival");

            DesktopBridge.Helpers bridgeHelpers = new DesktopBridge.Helpers();
            if (bridgeHelpers.IsRunningAsUwp())
            {
                updateStripMenuItem.Visible = false;
            }
        }

        private async void exportButton_Click(object sender, EventArgs e)
        {
            operationStatusLabel.Text = "Exporting file...";
            progressBar.Visible = true;

            // Simulate a high load process
            await Task.Delay(5 * 1000);

            string userPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            string fileName = $"{userPath}\\BoardingPass.txt";
            var builder = new StringBuilder();
            builder.AppendLine("Boarding pass generated by FlightTracker");
            builder.AppendLine("-----------------------------------------");
            builder.AppendLine($"Flight code: {codeTextbox.Text}");
            builder.AppendLine($"Flight date: {dateTextbox.Text}");
            builder.AppendLine($"Departure city: {departureTextbox.Text}");
            builder.AppendLine($"Arrival city: {arrivalTextbox.Text}");
            builder.AppendLine("-----------------------------------------");
            builder.AppendLine("Thank you for using FlightTracker");
            File.WriteAllText(fileName, builder.ToString());

            progressBar.Visible = false;
            operationStatusLabel.Visible = false;

            DesktopBridge.Helpers bridgeHelpers = new DesktopBridge.Helpers();
            if (bridgeHelpers.IsRunningAsUwp())
            {
                ShowNotification();
            }
            else
            {
                operationStatusLabel.Text = "Export completed";
            }
        }

        private void aboutFlightTrackerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutWindow about = new AboutWindow();
            about.ShowDialog();
        }

        private void ShowNotification()
        {
            string xml = $@"<toast>
                <visual>
                    <binding template='ToastGeneric'>
                        <text>Flight Tracker</text>
                        <text>The boarding pass for flight {codeTextbox.Text} from {departureTextbox.Text} to {arrivalTextbox.Text} has been exported on your desktop</text>
                    </binding>
                </visual>
            </toast>";

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            ToastNotification toast = new ToastNotification(doc);
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }

        private void updateStripMenuItem_Click(object sender, EventArgs e)
        {
            string title = "Flight Tracker";
            string message = "There are no updates available.";

            MessageBox.Show(message, title);
        }
    }
}
