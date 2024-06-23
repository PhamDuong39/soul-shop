using IdGen;
using System.Threading;

namespace Shop.Infrastructure;

public class NoGen
{
    private const int GEN_ORDER_NO_ID = 0;

    private static NoGen _instance;
    private static IdGenerator _genOrderNo = new(GEN_ORDER_NO_ID);

    private NoGen()
    {
    }

    public static NoGen Instance
    {
        get
        {
            if (_instance != null)
                return _instance;
            Interlocked.CompareExchange(ref _instance, new NoGen(), null);
            return _instance;
        }
    }

    public long GenOrderNo()
    {
        return _genOrderNo.CreateId();
    }
}