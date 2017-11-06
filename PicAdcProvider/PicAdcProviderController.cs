using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Adc.Provider;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.Storage.Streams;

namespace PicAdcProvider
{
    public class PicAdcProviderController : IAdcControllerProvider
    {
        private Task _initializeTask;
        private SerialDevice _port;
        private DataWriter _writer;
        private DataReader _reader;

        public PicAdcProviderController()
        {
            _initializeTask = Init(); ;
            //_initializeTask.RunSynchronously();
        }

        private async Task Init()
        {
            string aqs = SerialDevice.GetDeviceSelector();                   /* Find the selector string for the serial device   */
            var dis = await DeviceInformation.FindAllAsync(aqs);                    /* Find the serial device with our selector string  */
            _port = await SerialDevice.FromIdAsync(dis[0].Id);    /* Create an serial device with our selected device */

            /* Configure serial settings */
            _port.WriteTimeout = TimeSpan.FromMilliseconds(5);
            _port.ReadTimeout = TimeSpan.FromMilliseconds(5);
            _port.BaudRate = 9600;
            _port.Parity = SerialParity.None;
            _port.StopBits = SerialStopBitCount.One;
            _port.DataBits = 8;

            _writer = new DataWriter(_port.OutputStream);
            _reader = new DataReader(_port.InputStream);

        }

        public int ChannelCount => 6;

        public ProviderAdcChannelMode ChannelMode { get; set; } = ProviderAdcChannelMode.SingleEnded;

        public int MaxValue => 1023;

        public int MinValue => 0;

        public int ResolutionInBits => 10;
        
        public bool IsChannelModeSupported(ProviderAdcChannelMode channelMode)
        {
            return channelMode == ProviderAdcChannelMode.SingleEnded;
        }

        public int ReadValue(int channelNumber)
        {
            _writer.WriteString(channelNumber.ToString());
            _writer.StoreAsync().AsTask().Wait();
            var loadTask = _reader.LoadAsync(10).AsTask();
            loadTask.Wait();
            var length = loadTask.Result;
            var resultText = _reader.ReadString(length);
            return int.Parse(resultText);
        }

        uint channelStatus;
        public void AcquireChannel(int channel)
        {
            uint oldChannelStatus = channelStatus;
            uint channelToAquireFlag = (uint)(1 << channel);

            // See if the channel is available
            if ((oldChannelStatus & channelToAquireFlag) == 0)
            {
                // Not currently acquired
                channelStatus |= channelToAquireFlag;
            }
            else
            {
                // Already acquired, throw an exception
                throw new UnauthorizedAccessException();
            }
        }

        public void ReleaseChannel(int channel)
        {
            uint oldChannelStatus = channelStatus;
            uint channelToAquireFlag = (uint)(1 << channel);
            
            channelStatus &= ~channelToAquireFlag;
        }
    }
}
