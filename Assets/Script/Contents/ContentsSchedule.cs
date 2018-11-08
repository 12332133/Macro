using Assets.BitMex;
using System.Collections;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ContentsSchedule : ContentsBase
{
    public enum AlramType
    {
        Over,
        Under,
    }

    public class BitMexMarketPriceAlram
    {
        public AlramType Type { get; private set; }
        public decimal Price { get; private set; }
        public int AlramCount { get; private set; }

        public BitMexMarketPriceAlram(AlramType type, decimal price, int alramCount)
        {
            Type = type;
            Price = price;
            AlramCount = alramCount;
        }
    }

    private Thread thread;
    private ConcurrentQueue<BitMexMarketPriceAlram> alrams;

    private decimal marketPrice;
    private decimal tempPrice;

    private void Reset()
    {
    }

    private void Awake()
    {
        Application.runInBackground = true;

        this.alrams = new ConcurrentQueue<BitMexMarketPriceAlram>();

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

                if (this.marketPrice != this.tempPrice)
                {
                    this.tempPrice = this.marketPrice;
                    Debug.Log(string.Format("change price {0} => {1}", this.marketPrice, this.tempPrice));
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    private bool IsCompletePriceConditions(BitMexMarketPriceAlram alram)
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
        BitMexMarketPriceAlram alram;

        while (true)
        {
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
        this.alrams.Enqueue(new BitMexMarketPriceAlram(type, price, alramCount));
    }

}
