using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Cc.Upt.Common.LogHelper;

namespace Cc.Upt.Notificator
{
    public partial class MainForm : Form
    {
        private const int ExpirationBallonTip = 100;
        private const int EventsForShowBallonTip = 15;
        private int _notificatorCounter;
        private FileSystemWatcher _watcher;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            textBoxEventsQuantity.Text = EventsForShowBallonTip.ToString();
            textBoxNotificationExpiration.Text = ExpirationBallonTip.ToString();
            Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);

            PrepareFileWatcher();
            PrepareNotifier();

            WindowState = FormWindowState.Minimized;
            ShowInTaskbar = false;
        }

        private void PrepareNotifier()
        {
            notifyIcon.Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);
            notifyIcon.Text = @"Notificador de actualizador de isolución";
            notifyIcon.Visible = true;
            notifyIcon.BalloonTipTitle = @"Notificador del actualizador de isolución";
            notifyIcon.BalloonTipText = @"Click aquí para ver más detalles";
            notifyIcon.ShowBalloonTip((int) TimeSpan.FromSeconds(Convert.ToInt32(textBoxNotificationExpiration.Text))
                .TotalMilliseconds);
        }

        private void PrepareFileWatcher()
        {
            try
            {
                var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
                var expectedDirectory = string.Empty;

                while (true)
                {
                    var directoriesFromDirectory = Directory.GetDirectories(currentDirectory);
                    var wasFound = false;

                    foreach (var directory in directoriesFromDirectory)
                        if (new DirectoryInfo(directory).Name == "Ipm service")
                        {
                            expectedDirectory = directory;
                            wasFound = true;
                            break;
                        }

                    if (wasFound)
                        break;

                    var directoryInfo = new DirectoryInfo(currentDirectory).Parent;
                    if (directoryInfo != null)
                        currentDirectory = directoryInfo.FullName;
                }

                if (!Directory.Exists(expectedDirectory + @"\Logs"))
                    Directory.CreateDirectory(expectedDirectory + @"\Logs");

                _watcher = new FileSystemWatcher
                {
                    Path = expectedDirectory + @"\Logs",
                    NotifyFilter = NotifyFilters.LastWrite
                };
                _watcher.Changed += OnChanged;
                _watcher.EnableRaisingEvents = true;
                _watcher.Filter = "*.log";
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex);
            }
        }

        protected void OnChanged(object source, FileSystemEventArgs e)
        {
            try
            {
                var objFileInfo = new FileInfo(e.FullPath);
                if (!objFileInfo.Exists) return;

                if (textBoxNotification.InvokeRequired)
                    textBoxNotification.Invoke(new MethodInvoker(LoadTextBox));

                _notificatorCounter++;

                if (_notificatorCounter < Convert.ToInt32(textBoxEventsQuantity.Text)) return;

                notifyIcon.ShowBalloonTip((int) TimeSpan
                    .FromSeconds(Convert.ToInt32(textBoxNotificationExpiration.Text)).TotalMilliseconds);
                _notificatorCounter = 0;
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex);
            }
        }

        private void notifyIcon_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Normal;
            ShowInTaskbar = true;
            LoadTextBox();
        }

        private void LoadTextBox()
        {
            try
            {
                var logFiles = Directory.GetFiles(_watcher.Path);
                var fileList = logFiles.Select(x => new FileInfo(x)).ToList();
                var lastFile = fileList.OrderByDescending(x => x.CreationTime).FirstOrDefault();

                if (lastFile == null)
                {
                    Log.Instance.Info("File not found");
                    return;
                }

                textBoxNotification.Text = File.ReadAllText(lastFile.FullName);
                textBoxNotification.SelectionStart = textBoxNotification.TextLength;
                textBoxNotification.ScrollToCaret();
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex);
            }
        }

        private void buttonMinimize_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
            ShowInTaskbar = false;
        }

        private void textBoxEventsQuantity_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
                e.KeyChar != '.')
                e.Handled = true;
        }

        private void textBoxNotificationExpiration_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
                e.KeyChar != '.')
                e.Handled = true;
        }

        private void notifyIcon_BalloonTipClicked(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Normal;
            ShowInTaskbar = true;
            LoadTextBox();
        }
    }
}