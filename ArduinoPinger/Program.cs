using System;
using System.Text;
using System.Net.NetworkInformation;
using System.IO.Ports;
using System.Management;
using System.ComponentModel;

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
                string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
                byte[] buffer = Encoding.ASCII.GetBytes(data);
                int timeout = 240;

                string server = "server1.threesteplabs.com";

                PingReply reply = pingSender.Send(server, timeout, buffer, options);
                if (reply.Status == IPStatus.Success)
                {
                    Arduino a = new Arduino();
                    if (a.Port == null && !a.UpdateArduinoPort())
                    {
                        // TODO Present Error to User
                        continue;
                    }
                    
                }
            }
        }
    }
    class Arduino
    {
        public Arduino()
        {
            UpdateArduinoPort();
            this.components = new Container();
            this.serialPort = new SerialPort(this.components);
            this.serialPort.PortName = Port;
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
            catch (ManagementException e)
            {
                /* Do Nothing */
            }

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
    }
}
