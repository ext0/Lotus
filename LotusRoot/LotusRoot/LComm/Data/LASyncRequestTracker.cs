using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotusRoot.LComm.Data
{
    public class LASyncRequestTracker
    {
        public class ASyncRequest
        {
            public DateTime Time { get; set; }
            public LRequest Request { get; set; }
            public Action<LResponse> ResponseCallback { get; set; }
        }

        public class FulfilledRequest : ASyncRequest
        {
            public TimeSpan ElapsedTime { get; set; }
        }

        private static readonly ILog Logger = LogManager.GetLogger(typeof(LASyncRequestTracker));

        private List<ASyncRequest> _requests;
        private List<FulfilledRequest> _fulfilled;

        public IReadOnlyCollection<ASyncRequest> UnfullfilledRequests
        {
            get
            {
                return _requests.AsReadOnly();
            }
        }

        public IReadOnlyCollection<FulfilledRequest> FullfilledRequests
        {
            get
            {
                return _fulfilled.AsReadOnly();
            }
        }

        public LASyncRequestTracker()
        {
            _requests = new List<ASyncRequest>();
            _fulfilled = new List<FulfilledRequest>();
        }

        public void AddLRequest(LRequest request, Action<LResponse> response)
        {
            //if we see memory errors later, it's because of this ResponseCallback storage likely
            _requests.Add(new ASyncRequest
            {
                Time = DateTime.Now,
                Request = request,
                ResponseCallback = response
            });
        }

        public LRequest GetRequestByID(String id)
        {
            return (_requests.Where((x) =>
            {
                return (x.Request.ID.Equals(id));
            })).FirstOrDefault().Request;
        }

        public void FulfillRequest(LResponse response)
        {
            ASyncRequest fulfilling = (_requests.Where((x) =>
            {
                return (x.Request.ID.Equals(response.ID));
            })).FirstOrDefault();
            if (fulfilling != null)
            {
                _fulfilled.Add(new FulfilledRequest
                {
                    Time = fulfilling.Time,
                    Request = fulfilling.Request,
                    ResponseCallback = fulfilling.ResponseCallback,
                    ElapsedTime = (DateTime.Now - fulfilling.Time)
                });
                _requests.Remove(fulfilling);

                fulfilling.ResponseCallback(response);
            }
            else
            {
                Logger.Warn("Tried to fulfill nonexistant request [id: " + response.ID + "]");
            }
        }

        public void ClearFulfilled()
        {
            _fulfilled.Clear();
        }
    }
}
