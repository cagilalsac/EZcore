﻿#nullable disable

using EZcore.Models;
using Microsoft.Extensions.Configuration;

namespace BLL.Models
{
    public class AppSettings : AppSettingsBase
    {
        public static string Title { get; set; }
        public static string Description { get; set; }

        public AppSettings(IConfiguration configuration) : base(configuration)
        {
        }
    }
}