using System;

namespace dk.CctalkLib.Devices
{
    public struct DeviceEvent
    {
        public DeviceEvent(Byte coinCode, Byte errorOrRouteCode)
        {
            CoinCode = coinCode;
            ErrorOrRouteCode = errorOrRouteCode;
        }

        /// <summary>
        /// 硬币类型编号
        /// </summary>
        public Byte CoinCode;
        public Byte ErrorOrRouteCode;

        /// <summary>
        /// CoinCode为0
        /// </summary>
        public Boolean IsError
        {
            get
            {
                return CoinCode == 0;
            }
        }
    }
}