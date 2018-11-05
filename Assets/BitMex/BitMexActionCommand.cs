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
        IBitMexMainAdapter Main { get; }
        string DropBoxText { get; set; }
        void Execute();
    }

    public class FixedAvailableXbtCommand : IBitMexActionCommand
    {
        public IBitMexMainAdapter Main { get; set; }
        public string DropBoxText { get; set; }
        public decimal FixedAvailableXbt { get; set; }

        public void Execute()
        {
            this.Main.BitMexSession.FixedAvailableXbt = FixedAvailableXbt;
        }
    }

    public class SpecifiedAditionalCommand : IBitMexActionCommand
    {
        public IBitMexMainAdapter Main { get; set; }
        public string DropBoxText { get; set; }
        public decimal SpecifiedAditional { get; set; }

        public void Execute()
        {
            this.Main.BitMexSession.SpecifiedAditional = SpecifiedAditional;
        }
    }

    public class MarketPriceBuyCommand : IBitMexActionCommand
    {
        public IBitMexMainAdapter Main { get; set; }
        public string DropBoxText { get; set; }
        public int Magnification { get; set; }

        public void Execute()
        {
            if (Main.DriverService.IsInvaildEmail(this.Main.BitMexSession.Email) == false)
                return;

            if (Main.DriverService.OperationOrderMarketQty(0, Magnification, Main.BitMexSession.FixedAvailableXbt, 
                Main.DriverService.OperationGetCurrentSymbol()) == true)
            {
                Main.DriverService.OperationBuy();
            }
        }
    }

    public class MarketPriceSellCommand : IBitMexActionCommand
    {
        public IBitMexMainAdapter Main { get; set; }
        public string DropBoxText { get; set; }
        public int Magnification { get; set; }

        public void Execute()
        {
            if (this.Main.DriverService.IsInvaildEmail(this.Main.BitMexSession.Email) == false)
                return;

            if (this.Main.DriverService.OperationOrderMarketQty(0, Magnification, this.Main.BitMexSession.FixedAvailableXbt,
                this.Main.DriverService.OperationGetCurrentSymbol()) == true)
            {
                this.Main.DriverService.OperationSell();
            }
        }
    }
}
