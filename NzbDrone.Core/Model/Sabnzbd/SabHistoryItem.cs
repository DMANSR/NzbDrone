﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace NzbDrone.Core.Model.Sabnzbd
{
    public class SabHistoryItem
    {
        [JsonProperty(PropertyName = "fail_message")]
        public string FailMessage { get; set; }

        public string Size { get; set; }
        public string Category { get; set; }

        [JsonProperty(PropertyName = "nzb_name")]
        public string NzbName { get; set; }

        [JsonProperty(PropertyName = "download_time")]
        public int DownloadTime { get; set; }

        public string Storage { get; set; }
        public string Status { get; set; }

        [JsonProperty(PropertyName = "nzo_id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Title { get; set; }
    }
}
