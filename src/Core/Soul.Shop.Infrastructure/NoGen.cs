using IdGen;

namespace Soul.Shop.Infrastructure;

public class NoGen
{
    private const int GenOrderNoId = 0;

    private static NoGen _instance;
    private static readonly IdGenerator _genOrderNo = new(GenOrderNoId);

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