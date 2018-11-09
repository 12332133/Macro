using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.BitMex
{
    public enum BitMexCommandType
    {
        None,
        Test,

        //FixedAvailableXbt,
        //SpecifiedAditional,

        MarketPriceBuy10Magnification,
        MarketPriceBuy25Magnification,
        MarketPriceBuy50Magnification,
        MarketPriceBuy100Magnification,

        MarketPriceSell10Magnification,
        MarketPriceSell25Magnification,
        MarketPriceSell50Magnification,
        MarketPriceSell100Magnification,

        MarketSpecified10PriceBuy,
        MarketSpecified25PriceBuy,
        MarketSpecified50PriceBuy,
        MarketSpecified100PriceBuy,

        MarketSpecified10PriceSell,
        MarketSpecified25PriceSell,
        MarketSpecified50PriceSell,
        MarketSpecified100PriceSell,

        ClearPosition,
        CancleTopActivateOrder,
        CancleAllActivateOrder
    }

    public interface IBitMexActionCommand
    {
        IBitMexMainAdapter BitMexMain { get; }
        string ContentString { get; }
        bool IsExpose { get; }
        void Execute();
    }

    public abstract class BitMexActionCommand : IBitMexActionCommand
    {
        public IBitMexMainAdapter BitMexMain { get; private set; }
        public string ContentString { get; private set; }
        public bool IsExpose { get; private set; }
        public abstract void Execute();

        public BitMexActionCommand(IBitMexMainAdapter bitmexMain, string contentString, bool isExpose)
        {
            BitMexMain = bitmexMain;
            ContentString = contentString;
            IsExpose = isExpose;
        }
    }

    public class DefaultSampleCommand : BitMexActionCommand
    {
        public DefaultSampleCommand(IBitMexMainAdapter bitmexMain, string contentString, bool isExpose) 
            : base(bitmexMain, contentString, isExpose)
        {
        }

        public override void Execute()
        {
        }
    }

    public class FixedAvailableXbtCommand : BitMexActionCommand
    {
        public decimal FixedAvailableXbt { get; set; }

        public FixedAvailableXbtCommand(IBitMexMainAdapter bitmexMain, string contentString, bool isExpose) 
            : base(bitmexMain, contentString, isExpose)
        {
        }

        public override void Execute()
        {
            BitMexMain.Session.FixedAvailableXbt = FixedAvailableXbt;
        }
    }

    public class SpecifiedAditionalCommand : BitMexActionCommand
    {
        public decimal SpecifiedAditional { get; set; }

        public SpecifiedAditionalCommand(IBitMexMainAdapter bitmexMain, string contentString, bool isExpose) 
            : base(bitmexMain, contentString, isExpose)
        {
        }

        public override void Execute()
        {
            BitMexMain.Session.SpecifiedAditional = SpecifiedAditional;
        }
    }

    public class MarketPriceBuyCommand : BitMexActionCommand
    {
        public int Magnification { get; set; }

        public MarketPriceBuyCommand(IBitMexMainAdapter bitmexMain, string contentString, bool isExpose, int magnification) 
            : base(bitmexMain, contentString, isExpose)
        {
            Magnification = magnification;
        }

        public override void Execute()
        {
            if (BitMexMain.DriverService.IsInvaildEmail(BitMexMain.Session.Email) == false)
            {
                return;
            }

            if (BitMexMain.DriverService.HandleOrderMarketQty(
                0, 
                Magnification, 
                BitMexMain.Session.FixedAvailableXbt,
                BitMexMain.DriverService.HandleGetCurrentSymbol()
                ) == true)
            {
                BitMexMain.DriverService.HandleBuy();
            }
        }
    }

    public class MarketPriceSellCommand : BitMexActionCommand
    {
        public int Magnification { get; set; }

        public MarketPriceSellCommand(IBitMexMainAdapter bitmexMain, string contentString, bool isExpose, int magnification) 
            : base(bitmexMain, contentString, isExpose)
        {
            Magnification = magnification;
        }

        public override void Execute()
        {
            if (BitMexMain.DriverService.IsInvaildEmail(BitMexMain.Session.Email) == false)
            {
                return;
            }

            if (BitMexMain.DriverService.HandleOrderMarketQty(
                0, 
                Magnification, 
                BitMexMain.Session.FixedAvailableXbt,
                BitMexMain.DriverService.HandleGetCurrentSymbol()
                ) == true)
            {
                BitMexMain.DriverService.HandleSell();
            }
        }
    }

    public class MarketSpecifiedBuyCommand : BitMexActionCommand
    {
        public int Magnification { get; set; }

        public MarketSpecifiedBuyCommand(IBitMexMainAdapter bitmexMain, string contentString, bool isExpose, int magnification) 
            : base(bitmexMain, contentString, isExpose)
        {
            Magnification = magnification;
        }

        public override void Execute()
        {
            if (BitMexMain.DriverService.IsInvaildEmail(BitMexMain.Session.Email) == false)
            {
                return;
            }

            if (BitMexMain.DriverService.HandleOrderSpecifiedQty(
                0, 
                Magnification, 
                - BitMexMain.Session.SpecifiedAditional, 
                BitMexMain.Session.FixedAvailableXbt,
                BitMexMain.DriverService.HandleGetCurrentSymbol()
                ) == true)
            {
                BitMexMain.DriverService.HandleBuy();
            }
        }
    }

    public class MarketSpecifiedSellCommand : BitMexActionCommand
    {
        public int Magnification { get; set; }

        public MarketSpecifiedSellCommand(IBitMexMainAdapter bitmexMain, string contentString, bool isExpose, int magnification) 
            : base(bitmexMain, contentString, isExpose)
        {
            Magnification = magnification;
        }

        public override void Execute()
        {
            if (BitMexMain.DriverService.IsInvaildEmail(BitMexMain.Session.Email) == false)
            {
                return;
            }

            if (BitMexMain.DriverService.HandleOrderSpecifiedQty(
                0, 
                Magnification, 
                BitMexMain.Session.SpecifiedAditional,
                BitMexMain.Session.FixedAvailableXbt,
                BitMexMain.DriverService.HandleGetCurrentSymbol()) == true)
            {
                BitMexMain.DriverService.HandleSell();
            }
        }
    }

    public class ClearPositionCommand : BitMexActionCommand
    {
        public ClearPositionCommand(IBitMexMainAdapter bitmexMain, string contentString, bool isExpose) 
            : base(bitmexMain, contentString, isExpose)
        {
        }

        public override void Execute()
        {
            if (BitMexMain.DriverService.IsInvaildEmail(BitMexMain.Session.Email) == false)
            {
                return;
            }

            BitMexMain.DriverService.HandleClearPosition(
                BitMexMain.DriverService.HandleGetCurrentSymbol());
        }
    }

    public class CancleTopActivateOrderCommand : BitMexActionCommand
    {
        public CancleTopActivateOrderCommand(IBitMexMainAdapter bitmexMain, string contentString, bool isExpose) 
            : base(bitmexMain, contentString, isExpose)
        {
        }

        public override void Execute()
        {
            if (BitMexMain.DriverService.IsInvaildEmail(BitMexMain.Session.Email) == false)
            {
                return;
            }

            BitMexMain.DriverService.HandleCancleActivatedOrders(
                BitMexMain.DriverService.HandleGetCurrentSymbol(), false);
        }
    }

    public class CancleAllActivateOrderCommand : BitMexActionCommand
    {
        public CancleAllActivateOrderCommand(IBitMexMainAdapter bitmexMain, string contentString, bool isExpose) 
            : base(bitmexMain, contentString, isExpose)
        {
        }

        public override void Execute()
        {
            if (BitMexMain.DriverService.IsInvaildEmail(BitMexMain.Session.Email) == false)
            {
                return;
            }

            BitMexMain.DriverService.HandleCancleActivatedOrders(
                BitMexMain.DriverService.HandleGetCurrentSymbol(), true);
        }
    }
}
