using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Adc;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace PicAdcProvider
{
    
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private DispatcherTimer _timer;
        private AdcChannel _channel;
        private Task _startupTask;

        public MainPage()
        {
            this.InitializeComponent();
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(50);
            _timer.Tick += Timer_Tick;
            _startupTask = InitAdc();
        }

        private async Task InitAdc()
        {
            var list = await AdcController.GetControllersAsync(PicAdcProvider.Instance);
            var controller = list[0];
            _channel = controller.OpenChannel(0);
            _timer.Start();
        }

        private void Timer_Tick(object sender, object e)
        {
            valueTextBox.Text = _channel.ReadValue().ToString();
        }
    }
}
