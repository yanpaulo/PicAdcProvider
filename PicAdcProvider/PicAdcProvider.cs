using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Adc.Provider;

namespace PicAdcProvider
{
    public class PicAdcProvider : IAdcProvider
    {
        static PicAdcProvider() { Instance = new PicAdcProvider(); }
        private PicAdcProvider() { }

        public static PicAdcProvider Instance { get; private set; }

        public IReadOnlyList<IAdcControllerProvider> GetControllers()
            => new List<IAdcControllerProvider>() { new PicAdcProviderController() };
    }
}
