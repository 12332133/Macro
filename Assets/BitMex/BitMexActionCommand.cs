using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.BitMex
{
    public enum BitMexCommandType
    {
        Empty,
        Test,
        FixedAvailableXbt,
        SpecifiedAditional,
        MarketPriceBuy10Magnification,
        MarketPriceBuy25Magnification,
        MarketPriceBuy50Magnification,
        MarketPriceBuy100Magnification,
        MarketPriceSell10Magnification,
        MarketPriceSell25Magnification,
        MarketPriceSell50Magnification,
        MarketPriceSell100Magnification,
        SpecifiedPriceBuy,
        SpecifiedPriceSell,
        ClearPosition,
        CancleActivateOder,
        AutoBuy,
        AutoSell,
        Alarm,
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
            BitMexMain.WriteMacroLog("DefaultSampleCommand");
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
            BitMexMain.WriteMacroLog("FixedAvailableXbtCommand " + FixedAvailableXbt.ToString());
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

        public MarketPriceBuyCommand(IBitMexMainAdapter bitmexMain, string dropboxString) : base(bitmexMain, dropboxString)
        {
        }

        public override void Execute()
        {
            if (BitMexMain.DriverService.IsInvaildEmail(BitMexMain.Session.Email) == false)
            {
                return;
            }

            if (BitMexMain.DriverService.OperationOrderMarketQty(0, Magnification, BitMexMain.Session.FixedAvailableXbt,
                BitMexMain.DriverService.OperationGetCurrentSymbol()) == true)
            {
                BitMexMain.DriverService.OperationBuy();
            }
        }
    }

    public class MarketPriceSellCommand : BitMexActionCommand
    {
        public int Magnification { get; set; }

        public MarketPriceSellCommand(IBitMexMainAdapter bitmexMain, string dropboxString) : base(bitmexMain, dropboxString)
        {
        }

        public override void Execute()
        {
            if (BitMexMain.DriverService.IsInvaildEmail(BitMexMain.Session.Email) == false)
            {
                return;
            }

            if (BitMexMain.DriverService.OperationOrderMarketQty(0, Magnification, BitMexMain.Session.FixedAvailableXbt,
                BitMexMain.DriverService.OperationGetCurrentSymbol()) == true)
            {
                BitMexMain.DriverService.OperationSell();
            }
        }
    }
}
