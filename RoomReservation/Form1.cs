using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace RoomReservation
{
    public partial class Form1 : Form
    {
        private readonly Dictionary<int, int> roomTimes = new Dictionary<int, int>();
        private readonly Dictionary<int, string> roomNames = new Dictionary<int, string>();

        private System.Windows.Forms.Timer timer;

        public Form1()
        {
            InitializeComponent();

            for (int i = 1; i <= 6; i++)
            {
                roomTimes[i] = 0;
                roomNames[i] = "";
            }

            timer = new System.Windows.Forms.Timer();
            timer.Interval = 1000;
            timer.Tick += Timer_Tick;
            timer.Start();

            UpdateRoomLabels();
        }

        private bool TryParseDuration(string text, out int totalSeconds)
        {
            totalSeconds = 0;

            text = text.Trim();

            if (string.IsNullOrWhiteSpace(text))
                return false;

            string[] parts = text.Split(':');

            if (parts.Length == 1)
            {
                if (!int.TryParse(parts[0], out int minutes))
                    return false;

                if (minutes <= 0)
                    return false;

                totalSeconds = minutes * 60;

                return true;
            }

            if (parts.Length == 2)
            {
                if (!int.TryParse(parts[0], out int minutes))
                    return false;

                if (!int.TryParse(parts[1], out int seconds))
                    return false;

                if (minutes < 0 || seconds < 0 || seconds >= 60)
                    return false;

                totalSeconds = (minutes * 60) + seconds;

                return totalSeconds > 0;
            }

            return false;
        }

        private void ReserveRoom(int roomNumber)
        {
            string name = txtName.Text.Trim();
            string durationText = txtDuration.Text.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show(
                    "Zəhmət olmasa ad və soyadınızı daxil edin!",
                    "Xəta",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                txtName.Focus();
                return;
            }

            if (!TryParseDuration(durationText, out int duration))
            {
                MessageBox.Show(
                    "Müddəti düzgün daxil edin!\n\n" +
                    "Məsələn:\n" +
                    "2 = 2 dəqiqə\n" +
                    "2:30 = 2 dəqiqə 30 saniyə\n" +
                    "10:45 = 10 dəqiqə 45 saniyə",
                    "Xəta",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                txtDuration.Focus();
                return;
            }

            if (duration > 1440 * 60)
            {
                MessageBox.Show(
                    "Müddət maksimum 24 saat ola bilər!",
                    "Xəta",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                txtDuration.Focus();
                return;
            }

            if (roomTimes[roomNumber] > 0)
            {
                MessageBox.Show(
                    $"Otaq {roomNumber} artıq rezerv olunub!\n\n" +
                    $"Qalan müddət: {FormatTime(roomTimes[roomNumber])}",
                    "Otaq doludur",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                return;
            }

            roomTimes[roomNumber] = duration;
            roomNames[roomNumber] = name;

            UpdateRoomLabels();

            MessageBox.Show(
                $"Otaq {roomNumber} uğurla rezerv edildi!\n\n" +
                $"Ad və soyad: {name}\n" +
                $"Müddət: {FormatTime(duration)}",
                "Rezervasiya uğurludur",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );

            txtName.Clear();
            txtDuration.Clear();

            txtName.Focus();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            for (int i = 1; i <= 6; i++)
            {
                if (roomTimes[i] > 0)
                {
                    roomTimes[i]--;

                    if (roomTimes[i] == 0)
                    {
                        string name = roomNames[i];

                        roomNames[i] = "";

                        UpdateRoomLabels();

                        MessageBox.Show(
                            $"Otaq {i}-nin rezervasiya müddəti bitdi.\n\n" +
                            $"Rezervasiya edən: {name}",
                            "Müddət bitdi",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                    }
                }
            }

            UpdateRoomLabels();
        }

        private string FormatTime(int totalSeconds)
        {
            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;

            return $"{minutes:D2}:{seconds:D2}";
        }

        private void UpdateRoomLabels()
        {
            lblTime1.Text = FormatTime(roomTimes[1]);
            lblTime2.Text = FormatTime(roomTimes[2]);
            lblTime3.Text = FormatTime(roomTimes[3]);
            lblTime4.Text = FormatTime(roomTimes[4]);
            lblTime5.Text = FormatTime(roomTimes[5]);
            lblTime6.Text = FormatTime(roomTimes[6]);

            UpdateRoomButton(btnRoom1, 1);
            UpdateRoomButton(btnRoom2, 2);
            UpdateRoomButton(btnRoom3, 3);
            UpdateRoomButton(btnRoom4, 4);
            UpdateRoomButton(btnRoom5, 5);
            UpdateRoomButton(btnRoom6, 6);
        }

        private void UpdateRoomButton(Button button, int roomNumber)
        {
            if (roomTimes[roomNumber] > 0)
            {
                button.Text = "Otaq " + roomNumber;
                button.BackColor = Color.LightCoral;
                button.Enabled = false;
            }
            else
            {
                button.Text = "Otaq " + roomNumber;
                button.BackColor = SystemColors.Control;
                button.Enabled = true;
            }
        }

        private void btnRoom1_Click(object sender, EventArgs e)
        {
            ReserveRoom(1);
        }

        private void btnRoom2_Click(object sender, EventArgs e)
        {
            ReserveRoom(2);
        }

        private void btnRoom3_Click(object sender, EventArgs e)
        {
            ReserveRoom(3);
        }

        private void btnRoom4_Click(object sender, EventArgs e)
        {
            ReserveRoom(4);
        }

        private void btnRoom5_Click(object sender, EventArgs e)
        {
            ReserveRoom(5);
        }

        private void btnRoom6_Click(object sender, EventArgs e)
        {
            ReserveRoom(6);
        }
    }
}