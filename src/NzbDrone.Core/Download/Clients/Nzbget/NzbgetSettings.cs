﻿using System;
using FluentValidation;
using FluentValidation.Results;
using NzbDrone.Core.Annotations;
using NzbDrone.Core.ThingiProvider;

namespace NzbDrone.Core.Download.Clients.Nzbget
{
    public class NzbgetSettingsValidator : AbstractValidator<NzbgetSettings>
    {
        public NzbgetSettingsValidator()
        {
            RuleFor(c => c.Host).NotEmpty();
            RuleFor(c => c.Port).GreaterThan(0);
            RuleFor(c => c.Username).NotEmpty().When(c => !String.IsNullOrWhiteSpace(c.Password));
            RuleFor(c => c.Password).NotEmpty().When(c => !String.IsNullOrWhiteSpace(c.Username));
        }
    }

    public class NzbgetSettings : IProviderConfig
    {
        private static readonly NzbgetSettingsValidator Validator = new NzbgetSettingsValidator();

        public NzbgetSettings()
        {
            Host = "localhost";
            Port = 6789;
            TvCategory = "tv";
            RecentTvPriority = (int)NzbgetPriority.Normal;
            OlderTvPriority = (int)NzbgetPriority.Normal;
        }

        [FieldDefinition(0, Label = "Host", Type = FieldType.Textbox)]
        public String Host { get; set; }

        [FieldDefinition(1, Label = "Port", Type = FieldType.Textbox)]
        public Int32 Port { get; set; }

        [FieldDefinition(2, Label = "Username", Type = FieldType.Textbox)]
        public String Username { get; set; }

        [FieldDefinition(3, Label = "Password", Type = FieldType.Password)]
        public String Password { get; set; }

        [FieldDefinition(4, Label = "Category", Type = FieldType.Textbox)]
        public String TvCategory { get; set; }

        [FieldDefinition(5, Label = "Recent Priority", Type = FieldType.Select, SelectOptions = typeof(NzbgetPriority), HelpText = "Priority to use when grabbing episodes that aired within the last 14 days")]
        public Int32 RecentTvPriority { get; set; }

        [FieldDefinition(6, Label = "Older Priority", Type = FieldType.Select, SelectOptions = typeof(NzbgetPriority), HelpText = "Priority to use when grabbing episodes that aired over 14 days ago")]
        public Int32 OlderTvPriority { get; set; }

        [FieldDefinition(7, Label = "Use SSL", Type = FieldType.Checkbox)]
        public Boolean UseSsl { get; set; }

        public ValidationResult Validate()
        {
            return Validator.Validate(this);
        }
    }
}
