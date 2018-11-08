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
        string DropBoxText { get; set; }
        void Execute();
    }

    public abstract class BitMexActionCommand : IBitMexActionCommand
    {
        public IBitMexMainAdapter BitMexMain { get; set; }
        public string DropBoxText { get; set; }
        public abstract void Execute();

        public BitMexActionCommand(IBitMexMainAdapter bitmexMain, string dropboxString)
        {
        }
    }

    public class DefaultSampleCommand : BitMexActionCommand
    {
        public DefaultSampleCommand(IBitMexMainAdapter bitmexMain, string dropboxString) : base(bitmexMain, dropboxString)
        {
        }

        public override void Execute()
        {
        }
    }

    public class FixedAvailableXbtCommand : BitMexActionCommand
    {
        public decimal FixedAvailableXbt { get; set; }

        public FixedAvailableXbtCommand(IBitMexMainAdapter bitmexMain, string dropboxString) : base(bitmexMain, dropboxString)
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

        public SpecifiedAditionalCommand(IBitMexMainAdapter bitmexMain, string dropboxString) : base(bitmexMain, dropboxString)
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

        public MarketPriceBuyCommand(IBitMexMainAdapter bitmexMain, string dropboxString, int magnification) : base(bitmexMain, dropboxString)
        {
            Magnification = magnification;
        }

        public override void Execute()
        {
            if (BitMexMain.DriverService.IsInvaildEmail(BitMexMain.Session.Email) == false)
            {
                return;
            }

            if (BitMexMain.DriverService.OperationOrderMarketQty(
                0, 
                Magnification, 
                BitMexMain.Session.FixedAvailableXbt,
                BitMexMain.DriverService.OperationGetCurrentSymbol()
                ) == true)
            {
                BitMexMain.DriverService.OperationBuy();
            }
        }
    }

    public class MarketPriceSellCommand : BitMexActionCommand
    {
        public int Magnification { get; set; }

        public MarketPriceSellCommand(IBitMexMainAdapter bitmexMain, string dropboxString, int magnification) : base(bitmexMain, dropboxString)
        {
            Magnification = magnification;
        }

        public override void Execute()
        {
            if (BitMexMain.DriverService.IsInvaildEmail(BitMexMain.Session.Email) == false)
            {
                return;
            }

            if (BitMexMain.DriverService.OperationOrderMarketQty(
                0, 
                Magnification, 
                BitMexMain.Session.FixedAvailableXbt,
                BitMexMain.DriverService.OperationGetCurrentSymbol()
                ) == true)
            {
                BitMexMain.DriverService.OperationSell();
            }
        }
    }

    public class MarketSpecifiedBuyCommand : BitMexActionCommand
    {
        public int Magnification { get; set; }

        public MarketSpecifiedBuyCommand(IBitMexMainAdapter bitmexMain, string dropboxString, int magnification) : base(bitmexMain, dropboxString)
        {
            Magnification = magnification;
        }

        public override void Execute()
        {
            if (BitMexMain.DriverService.IsInvaildEmail(BitMexMain.Session.Email) == false)
            {
                return;
            }

            if (BitMexMain.DriverService.OperationOrderSpecifiedQty(
                0, 
                Magnification, 
                - BitMexMain.Session.SpecifiedAditional, 
                BitMexMain.Session.FixedAvailableXbt,
                BitMexMain.DriverService.OperationGetCurrentSymbol()
                ) == true)
            {
                BitMexMain.DriverService.OperationBuy();
            }
        }
    }

    public class MarketSpecifiedSellCommand : BitMexActionCommand
    {
        public int Magnification { get; set; }

        public MarketSpecifiedSellCommand(IBitMexMainAdapter bitmexMain, string dropboxString, int magnification) : base(bitmexMain, dropboxString)
        {
            Magnification = magnification;
        }

        public override void Execute()
        {
            if (BitMexMain.DriverService.IsInvaildEmail(BitMexMain.Session.Email) == false)
            {
                return;
            }

            if (BitMexMain.DriverService.OperationOrderSpecifiedQty(
                0, 
                Magnification, 
                BitMexMain.Session.SpecifiedAditional,
                BitMexMain.Session.FixedAvailableXbt,
                BitMexMain.DriverService.OperationGetCurrentSymbol()) == true)
            {
                BitMexMain.DriverService.OperationSell();
            }
        }
    }

    public class ClearPositionCommand : BitMexActionCommand
    {
        public ClearPositionCommand(IBitMexMainAdapter bitmexMain, string dropboxString) : base(bitmexMain, dropboxString)
        {
        }

        public override void Execute()
        {
            if (BitMexMain.DriverService.IsInvaildEmail(BitMexMain.Session.Email) == false)
            {
                return;
            }

            BitMexMain.DriverService.OperationClearPosition(
                BitMexMain.DriverService.OperationGetCurrentSymbol());
        }
    }

    public class CancleTopActivateOrderCommand : BitMexActionCommand
    {
        public CancleTopActivateOrderCommand(IBitMexMainAdapter bitmexMain, string dropboxString) : base(bitmexMain, dropboxString)
        {
        }

        public override void Execute()
        {
            if (BitMexMain.DriverService.IsInvaildEmail(BitMexMain.Session.Email) == false)
            {
                return;
            }

            BitMexMain.DriverService.OperationCancleActivatedOrders(
                BitMexMain.DriverService.OperationGetCurrentSymbol(), false);
        }
    }

    public class CancleAllActivateOrderCommand : BitMexActionCommand
    {
        public CancleAllActivateOrderCommand(IBitMexMainAdapter bitmexMain, string dropboxString) : base(bitmexMain, dropboxString)
        {
        }

        public override void Execute()
        {
            if (BitMexMain.DriverService.IsInvaildEmail(BitMexMain.Session.Email) == false)
            {
                return;
            }

            BitMexMain.DriverService.OperationCancleActivatedOrders(
                BitMexMain.DriverService.OperationGetCurrentSymbol(), true);
        }
    }
}
