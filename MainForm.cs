using LowLevelHooking;
using LSSDReportHelper;
using LSSDReportHelper.Engines;
using LSSDReportHelper.Models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class MainForm : Form
    {
        private readonly ScreenShooter _screenShooter = new ScreenShooter();
        private readonly PatrolEngine _patrolEngine = new PatrolEngine();
        private readonly DiscordEngine _discord = new DiscordEngine();

        private string _selectedVehicle;

        public MainForm()
        {
            InitializeComponent();
            var config = File.ReadAllText("config.json");
            var myJObject = JsonConvert.DeserializeObject<ConfigModel>(config);

            textBox5.Text = myJObject.Arrest;
            textBox4.Text = myJObject.ArrestSupport;
            textBox3.Text = myJObject.Fine;
            textBox2.Text = myJObject.Patrol;
            textBox1.Text = myJObject.PatrolInterval.ToString();
            button1.Enabled = false;

            Program.GlobalKeyboardHook.KeyDownOrUp += GlobalKeyboardHook_KeyDownOrUp;
            Disposed += MainView_Disposed;

            SetVehicleList();
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            var currentValue = folderBrowserDialog1.SelectedPath;
            folderBrowserDialog1.ShowDialog();
            textBox5.Text = folderBrowserDialog1.SelectedPath;
            if (currentValue != folderBrowserDialog1.SelectedPath)
                button1.Enabled = true;
        }

        private void button6_Click_1(object sender, EventArgs e)
        {
            var currentValue = folderBrowserDialog1.SelectedPath;
            folderBrowserDialog1.ShowDialog();
            textBox4.Text = folderBrowserDialog1.SelectedPath;
            if (currentValue != folderBrowserDialog1.SelectedPath)
                button1.Enabled = true;
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            var currentValue = folderBrowserDialog1.SelectedPath;
            folderBrowserDialog1.ShowDialog();
            textBox3.Text = folderBrowserDialog1.SelectedPath;
            if (currentValue != folderBrowserDialog1.SelectedPath)
                button1.Enabled = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var saveObject = new ConfigModel
            {
                Arrest = textBox5.Text,
                ArrestSupport = textBox4.Text,
                Fine = textBox3.Text,
                Patrol = textBox2.Text,
                PatrolInterval = (int)GetPatrolInterval()
            };
            var config = JsonConvert.SerializeObject(saveObject, Formatting.Indented);
            File.WriteAllText("config.json", config);
            button1.Enabled = false;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            button1.Enabled = true;
        }

        private async void DiscordLoginBtn_Click(object sender, EventArgs e)
        {
            await _discord.SignIn(DiscordLoginText.Text, DiscordPasswordText.Text);
        }

        private uint GetPatrolInterval()
        {
            var currentInterval = textBox1.Text;
            if (!uint.TryParse(textBox1.Text, out var value) || value < 1)
            {
                MessageBox.Show("Неверно задано количество минут!");
                textBox1.Text = currentInterval;
                return uint.Parse(currentInterval);
            }

            return value;
        }

        private void GlobalKeyboardHook_KeyDownOrUp(object sender, GlobalKeyboardHookEventArgs e)
        {
            if (!e.IsUp || !e.Alt) return;
            switch (e.KeyCode)
            {
                case VirtualKey.D1:
                    _screenShooter.SaveImage(textBox5.Text);
                    break;
                case VirtualKey.D2:
                    _screenShooter.SaveImage(textBox4.Text);
                    break;
                case VirtualKey.D3:
                    _screenShooter.SaveImage(textBox3.Text);
                    break;
                case VirtualKey.D4:
                    button2.PerformClick();
                    break;
                case VirtualKey.D5:
                    Show();
                    break;
                case VirtualKey.D6:
                    Hide();
                    break;
                case VirtualKey.D7:
                    VehicleAction();
                    break;
                case VirtualKey.D8:
                    VehicleAction(false);
                    break;
            }
        }

        private void MainView_Disposed(object sender, EventArgs e)
        {
            Program.GlobalKeyboardHook.KeyDownOrUp -= GlobalKeyboardHook_KeyDownOrUp;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var value = GetPatrolInterval();
            if (button2.Text == "Начать")
            {
                _patrolEngine.ChangeInterval(value);
                _patrolEngine.ChangeFolder(textBox2.Text);

                button2.Text = "Завершить";
                textBox1.Enabled = false;
                button3.Enabled = false;
            }
            else
            {
                button2.Text = "Начать";
                textBox1.Enabled = true;
                button3.Enabled = true;
            }
            _patrolEngine.StartPatrol();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var currentValue = folderBrowserDialog1.SelectedPath;
            folderBrowserDialog1.ShowDialog();
            textBox2.Text = folderBrowserDialog1.SelectedPath;
            if (currentValue != folderBrowserDialog1.SelectedPath)
                button1.Enabled = true;
        }

        private void GetVehicleBtn_Click(object sender, EventArgs e)
        {
            VehicleAction();
        }

        private void PutVehicleBtn_Click(object sender, EventArgs e)
        {
            VehicleAction(false);
        }

        private void SetVehicleList()
        {
            for (var i = 1; i <= 20; i++)
            {
                var number = i < 10 ? $"0{i}" : $"{i}";
                VehicleList.Items.Add($"LSSD{number}");
            }

            VehicleList.SelectedIndex = VehicleList.FindStringExact("LSSD01");
            _selectedVehicle = VehicleList.Items[VehicleList.SelectedIndex].ToString();
        }

        private async void VehicleAction(bool isGet = true)
        {
            var path = Directory.GetCurrentDirectory();
            var file = _screenShooter.SaveImage(path);
            var action = isGet ? "Взял" : "Сдал";
            await _discord.SendMessageWithFile(762405385842982932, $"1. Andrew Sinson\n2. {_selectedVehicle}({action})\n3.", file);
            File.Delete(file);
        }

        private void VehicleList_SelectedIndexChanged(object sender, EventArgs e)
        {
            _selectedVehicle = VehicleList.Items[VehicleList.SelectedIndex].ToString();
        }
    }
}
