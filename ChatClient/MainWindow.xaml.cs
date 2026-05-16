using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ChatClient
{
    public partial class MainWindow : Window
    {
        private TcpClient? client;
        private NetworkStream? stream;

        public MainWindow() => InitializeComponent();

        private async void BtnConnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                client = new TcpClient();
                await client.ConnectAsync("127.0.0.1", 8888);
                stream = client.GetStream();

                txtHistory.AppendText("✅ Connected to server\n");
                btnConnect.IsEnabled = false;

                _ = ReadFromServerAsync();
            }
            catch (Exception ex)
            {
                txtHistory.AppendText($"❌ Error: {ex.Message}\n");
            }
        }

        private async void BtnSend_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Add input validation and sanitization
            if (stream == null || string.IsNullOrWhiteSpace(txtMessage.Text)) return;

            try
            {
                string msg = txtMessage.Text;
                byte[] data = Encoding.UTF8.GetBytes(msg);
                await stream.WriteAsync(data, 0, data.Length);

                txtHistory.AppendText($"📤 You: {msg}\n");
                txtMessage.Clear();
            }
            catch (Exception ex)
            {
                txtHistory.AppendText($"❌ Send error: {ex.Message}\n");
            }
        }

        private async Task ReadFromServerAsync()
        {
            var buffer = new byte[1024];
            try
            {
                while (client?.Connected == true)
                {
                    int bytesRead = await stream!.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;

                    string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Dispatcher.Invoke(() => txtHistory.AppendText($"📥 Server: {response}\n"));
                }
            }
            catch { }
        }

        private void TxtMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) BtnSend_Click(sender, e);
        }
    }
}