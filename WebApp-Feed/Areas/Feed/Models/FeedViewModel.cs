

using WebApp_Feed.Areas.Feed.Models;

namespace WebApp_Feed.Models
{
    public class FeedViewModel
    {
        public List<Post> Posts { get; set; } = new List<Post>();
        public List<Event> Events { get; set; } = new List<Event>();
        public bool IsAuthenticated { get; set; }
        public Auth CurrentUser { get; set; }

        public List<IFeedItem> CombinedFeed
        {
            get
            {
                var feedItems = new List<IFeedItem>();

                feedItems.AddRange(Posts);

                feedItems.AddRange(Events);


                return feedItems.OrderByDescending(x => x.CreatedAt).ToList();
            }
        }
    }
}
