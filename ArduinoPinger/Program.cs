using System;
using System.Text;
using System.Net.NetworkInformation;
using System.IO.Ports;
using System.Management;
using System.ComponentModel;
using System.Windows.Forms;

namespace ArduinoPinger
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Ping pingSender = new Ping();
                PingOptions options = new PingOptions();

                options.DontFragment = true;

                // 32 Bytes of Data
                string data = "randomdatatobeusedforpinglalalal";
                byte[] buffer = Encoding.ASCII.GetBytes(data);
                int timeout = 240;

                string server = "server1.threesteplabs.com";

                PingReply reply = pingSender.Send(server, timeout, buffer, options);
                if (reply.Status == IPStatus.Success)
                {
                    Arduino a = new Arduino();
                    if (a.Port == null)
                    {
                        error("Unable to Connect to Arduino", "Ping: Success  Arduino: Error 1");
                        break;
                    }
                    if (!a.openConnection())
                    {
                        error("Connection Error", "Ping: Success  Arduino: Error 2");
                    }else
                    {
                        a.writePing(unchecked((int)reply.RoundtripTime));
                    }
                }
                else{
                    Arduino a = new Arduino();
                    if (a.Port == null)
                    {
                        error("Unable to Connect to Arduino", "Ping: Error  Arduino: Error 1");
                        break;
                    }
                    if (!a.openConnection())
                    {
                        error("Connection Error", "Ping: Error  Arduino: Error 2");
                    }
                    else
                    {
                        a.displayError();
                    }
                }
            }
        }

        private static void error(string message, string caption)
        {
            MessageBoxButtons buttons = MessageBoxButtons.OK;
            MessageBox.Show(message, caption, buttons);
        }
    }
    class Arduino
    {
        public Arduino()
        {
            this.components = new Container();
            this.serialPort = new SerialPort(this.components);
        }

        public string Port
        {
            get;
            private set;
        }

        private SerialPort serialPort;
        private IContainer components = null;

        public Boolean UpdateArduinoPort()
        {
            ManagementScope connectionScope = new ManagementScope();
            SelectQuery serialQuery = new SelectQuery("SELECT * FROM Win32_SerialPort");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(connectionScope, serialQuery);

            try
            {
                foreach (ManagementObject item in searcher.Get())
                {
                    string desc = item["Description"].ToString();
                    string deviceId = item["DeviceID"].ToString();

                    if (desc.Contains("Arduino"))
                    {
                        Port = deviceId;
                        return true;
                    }
                }
            }
            catch (ManagementException) { /* Ignored */ }

            Port = null;
            return false;
        }

        public bool openConnection()
        {
            if (!serialPort.IsOpen)
            {
                serialPort.Open();
                return true;
            }
            return false;
        }

        public void closeConnection()
        {
            serialPort.Close();
        }

        public void writePing(int ping)
        {
            string temp = ping.ToString();
            serialPort.Write(temp);
        }

        internal void displayError()
        {
            serialPort.Write("error");
        }

        ~Arduino()
        {
            closeConnection();
            Port = null;
        }
    }
}
