using Assets.BitMex;
using UnityEngine;

public class ContentsBase : MonoBehaviour
{
    protected IBitMexMainAdapter bitmexMain;

    public virtual void Initialize(IBitMexMainAdapter bitmexMain)
    {
        this.bitmexMain = bitmexMain;
    }
}
