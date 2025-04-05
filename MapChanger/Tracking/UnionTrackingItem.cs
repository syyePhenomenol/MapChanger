using System.Linq;

namespace MapChanger.Tracking;

public class UnionTrackingItem : MultiTrackingItem
{
    public override bool Has()
    {
        return TrackingItems.Any(i => i.Has());
    }
}
