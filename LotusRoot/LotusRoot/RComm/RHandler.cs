using log4net;
using log4net.Core;
using LotusRoot.CComm.CData;
using LotusRoot.Datastore;
using LotusRoot.RComm;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotusRoot.RComm
{
    public static class RHandler
    {
        public enum RootStatus
        {
            REGISTERED,
            LIVE,
            DEAD
        }

        private static readonly ILog Logger = LogManager.GetLogger(typeof(RHandler));

        private static ConcurrentDictionary<Root, RootStatus> _roots = new ConcurrentDictionary<Root, RootStatus>();
        private static TimeSpan _rootCacheDead = TimeSpan.FromMinutes(1d);
        private static DateTime _lastRootQuery = DateTime.MinValue;

        public static void UpdateRoots()
        {
            if (_lastRootQuery.Add(_rootCacheDead) < DateTime.Now)
            {
                List<Root> live = new List<Root>();
                List<Root> dead = new List<Root>();
                foreach (Root root in _roots.Keys)
                {
                    try
                    {
                        RRemoteInvokable remoteRoot = (RRemoteInvokable)Activator.GetObject(typeof(RRemoteInvokable), "tcp://" + root.Endpoint.ToString() + ":" + root.RPort + "/RRemote");
                        root.Identifier = remoteRoot.GetIdentifier();
                        root.CPorts = remoteRoot.GetCPorts();
                        foreach (CThumbprint thumbprint in remoteRoot.GetLocalCThumbprints())
                        {
                            RClientStore.AddRemoteCThumbprint(root, thumbprint);
                        }
                        live.Add(root);
                    }
                    catch (Exception e)
                    {
                        Logger.Error("Failed to connect to " + root.Endpoint.ToString() + ":" + root.RPort + "! : " + e.Message);
                        dead.Add(root);
                    }
                }
                _roots.Clear();
                foreach (Root alive in live)
                {
                    _roots[alive] = RootStatus.LIVE;
                }
                foreach (Root disconnected in dead)
                {
                    _roots[disconnected] = RootStatus.DEAD;
                }
                _lastRootQuery = DateTime.Now;
            }
        }

        public static List<Root> LiveRoots
        {
            get
            {
                UpdateRoots();
                return _roots.Where((x) =>
                {
                    return x.Value == RootStatus.LIVE;
                }).Select((y) =>
                {
                    return y.Key;
                }).ToList();
            }
        }

        public static List<Root> RegisteredRoots
        {
            get
            {
                UpdateRoots();
                return _roots.Select((x) =>
                {
                    return x.Key;
                }).ToList();
            }
        }

        public static List<Root> DeadRoots
        {
            get
            {
                UpdateRoots();
                return _roots.Where((x) =>
                {
                    return x.Value == RootStatus.DEAD;
                }).Select((y) =>
                {
                    return y.Key;
                }).ToList();
            }
        }

        public static void RegisterRoot(String IP, short rport)
        {
            if (!_roots.TryAdd(new Root(IP, new short[] { }, rport, String.Empty), RootStatus.REGISTERED))
            {
                throw new Exception("Failed to add Root due to concurrency issue!");
            }
        }
    }
}
