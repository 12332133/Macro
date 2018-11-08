using Assets.BitMex;
using System.Collections;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ContentsAlarm : ContentsBase
{
    public enum AlramType
    {
        Over,
        Under,
    }

    public class MarketPriceAlram
    {
        public AlramType Type { get; private set; }
        public decimal Price { get; private set; }
        public int AlramCount { get; private set; }

        public MarketPriceAlram(AlramType type, decimal price, int alramCount)
        {
            Type = type;
            Price = price;
            AlramCount = alramCount;
        }
    }

    private Thread thread;
    private ConcurrentQueue<MarketPriceAlram> alrams;

    private decimal marketPrice;

    private void Reset()
    {
    }

    private void Awake()
    {
        this.alrams = new ConcurrentQueue<MarketPriceAlram>();

        this.thread = new Thread(DoWork);
        this.thread.IsBackground = true;
        this.thread.Start();
    }

    public override void Initialize(IBitMexMainAdapter bitmexMain)
    {
        base.Initialize(bitmexMain);
        StartCoroutine(UpdateMarketPrice());
    }

    IEnumerator UpdateMarketPrice() // main으로 이동 ?
    {
        while (true)
        {
            if (this.bitmexMain.DriverService.IsLoginBitMex() == true)
            {
                this.marketPrice = this.bitmexMain.DriverService.OperationGetMarketPrice();
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    private bool IsCompletePriceConditions(MarketPriceAlram alram)
    {
        switch (alram.Type)
        {
            case AlramType.Over:
                return this.marketPrice < alram.Price;
            case AlramType.Under:
                return this.marketPrice > alram.Price;
        }
        return false;
    }

    private void DoWork()
    {
        while (true)
        {
            MarketPriceAlram alram;

            if (this.alrams.TryDequeue(out alram) == true)
            {
                if (IsCompletePriceConditions(alram) == true)
                {
                    Task.Run(() =>
                    {
                        for (int i = 0; i < alram.AlramCount; i++)
                        {
                            Debug.Log(string.Format("run alram {0}", alram.Price));
                        }
                    });
                }
                else
                {
                    this.alrams.Enqueue(alram);
                }
            }

            Thread.Sleep(50);
        }
    }

    public void AddSchedule(AlramType type, decimal price, int alramCount)
    {
        this.alrams.Enqueue(new MarketPriceAlram(type, price, alramCount));
    }
}
