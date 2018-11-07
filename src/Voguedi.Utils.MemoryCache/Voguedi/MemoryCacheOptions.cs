using System;

namespace Voguedi
{
    public class MemoryCacheOptions
    {
        #region Public Properties

        public TimeSpan DefaultSlidingExpiration { get; set; } = TimeSpan.FromDays(3);

        public DateTimeOffset? DefaultAbsoluteExpiration { get; set; }

        #endregion
    }
}
