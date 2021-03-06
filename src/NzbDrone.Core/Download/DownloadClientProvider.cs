﻿using System;
using System.Linq;
using System.Collections.Generic;
using NzbDrone.Core.Indexers;

namespace NzbDrone.Core.Download
{
    public interface IProvideDownloadClient
    {
        IDownloadClient GetDownloadClient(DownloadProtocol downloadProtocol);
        IEnumerable<IDownloadClient> GetDownloadClients();
    }

    public class DownloadClientProvider : IProvideDownloadClient
    {
        private readonly IDownloadClientFactory _downloadClientFactory;

        public DownloadClientProvider(IDownloadClientFactory downloadClientFactory)
        {
            _downloadClientFactory = downloadClientFactory;
        }

        public IDownloadClient GetDownloadClient(DownloadProtocol downloadProtocol)
        {
            return _downloadClientFactory.GetAvailableProviders().FirstOrDefault(v => v.Protocol == downloadProtocol);
        }

        public IEnumerable<IDownloadClient> GetDownloadClients()
        {
            return _downloadClientFactory.GetAvailableProviders();
        }
    }
}