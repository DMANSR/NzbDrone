﻿using System.Collections.Generic;
using FluentValidation;
using Nancy;
using NzbDrone.Api.Mapping;
using NzbDrone.Core.DecisionEngine;
using NzbDrone.Core.DecisionEngine.Specifications;
using NzbDrone.Core.Download;
using NzbDrone.Core.IndexerSearch;
using NzbDrone.Core.Indexers;
using NzbDrone.Core.Parser;
using NzbDrone.Core.Parser.Model;
using Omu.ValueInjecter;
using System.Linq;
using Nancy.ModelBinding;
using NzbDrone.Api.Extensions;

namespace NzbDrone.Api.Indexers
{
    public class ReleaseModule : NzbDroneRestModule<ReleaseResource>
    {
        private readonly IFetchAndParseRss _rssFetcherAndParser;
        private readonly ISearchForNzb _nzbSearchService;
        private readonly IMakeDownloadDecision _downloadDecisionMaker;
        private readonly IDownloadService _downloadService;
        private readonly IParsingService _parsingService;

        public ReleaseModule(IFetchAndParseRss rssFetcherAndParser,
                             ISearchForNzb nzbSearchService,
                             IMakeDownloadDecision downloadDecisionMaker,
                             IDownloadService downloadService,
                             IParsingService parsingService)
        {
            _rssFetcherAndParser = rssFetcherAndParser;
            _nzbSearchService = nzbSearchService;
            _downloadDecisionMaker = downloadDecisionMaker;
            _downloadService = downloadService;
            _parsingService = parsingService;
            GetResourceAll = GetReleases;
            Post["/"] = x=> DownloadRelease(this.Bind<ReleaseResource>());

            PostValidator.RuleFor(s => s.DownloadAllowed).Equal(true);
        }

        private Response DownloadRelease(ReleaseResource release)
        {
            var remoteEpisode = _parsingService.Map(release.InjectTo<ParsedEpisodeInfo>(), 0);
            remoteEpisode.Release = release.InjectTo<ReleaseInfo>();
            _downloadService.DownloadReport(remoteEpisode);

            return release.AsResponse();
        }

        private List<ReleaseResource> GetReleases()
        {
            if (Request.Query.episodeId != null)
            {
                return GetEpisodeReleases(Request.Query.episodeId);
            }

            return GetRss();
        }

        private List<ReleaseResource> GetEpisodeReleases(int episodeId)
        {
            var decisions = _nzbSearchService.EpisodeSearch(episodeId);

            return MapDecisions(decisions);
        }

        private List<ReleaseResource> GetRss()
        {
            var reports = _rssFetcherAndParser.Fetch();
            var decisions = _downloadDecisionMaker.GetRssDecision(reports);

            return MapDecisions(decisions);
        }

        private static List<ReleaseResource> MapDecisions(IEnumerable<DownloadDecision> decisions)
        {
            var result = new List<ReleaseResource>();

            foreach (var downloadDecision in decisions)
            {
                var release = new ReleaseResource();

                release.InjectFrom(downloadDecision.RemoteEpisode.Release);
                release.InjectFrom(downloadDecision.RemoteEpisode.ParsedEpisodeInfo);
                release.InjectFrom(downloadDecision);
                release.Rejections = downloadDecision.Rejections.ToList();
                release.DownloadAllowed = downloadDecision.RemoteEpisode.DownloadAllowed;

                result.Add(release);
            }

            return result;
        }
    }
}