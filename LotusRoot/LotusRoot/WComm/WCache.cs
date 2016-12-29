using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace LotusRoot.WComm
{
    public static class WCache
    {
        private static CacheItemPolicy _shortTermPolicy;
        private static CacheItemPolicy _normalPolicy;
        private static CacheItemPolicy _longTermPolicy;

        static WCache()
        {
            _shortTermPolicy = new CacheItemPolicy();
            _shortTermPolicy.AbsoluteExpiration = DateTime.Now.AddSeconds(10);

            _normalPolicy = new CacheItemPolicy();
            _normalPolicy.AbsoluteExpiration = DateTime.Now.AddSeconds(30);

            _longTermPolicy = new CacheItemPolicy();
            _longTermPolicy.AbsoluteExpiration = DateTime.Now.AddSeconds(300);
        }

        private static MemoryCache _cache = new MemoryCache("WCACHE", null);

        public static bool CacheValue(String identifier, Object value, WCachePolicy policy)
        {
            CacheItem item = new CacheItem(identifier, value);
            CacheItemPolicy itemPolicy;
            if (policy == WCachePolicy.SHORT_TERM)
            {
                itemPolicy = _shortTermPolicy;
            }
            else if (policy == WCachePolicy.NORMAL)
            {
                itemPolicy = _normalPolicy;
            }
            else
            {
                itemPolicy = _longTermPolicy;
            }
            return _cache.Add(item, itemPolicy);
        }

        public static Object GetCachedValue(String identifier)
        {
            return _cache.Get(identifier);
        }

        public static bool InvalidateCachedValue(String identifier)
        {
            return (_cache.Remove(identifier) != null);
        }
    }
}
