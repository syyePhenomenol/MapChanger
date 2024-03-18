using System.Collections.Generic;
using System.Linq;

namespace MapChanger.Tracking
{
    public abstract class MultiTrackingItem : TrackingItem
    {
        public TrackingItem[] TrackingItems { get; init; }

        public IEnumerable<(string, string)> GetSdTrackingItemPairs()
        {
            return TrackingItems.Where(ti => ti is SdTrackingItem).Select(ti => ((SdTrackingItem)ti).SceneDataPair);
        }
    }
}