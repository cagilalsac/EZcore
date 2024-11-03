#nullable disable

using Microsoft.Extensions.Configuration;

namespace EZcore.Models
{
    public abstract class AppSettingsBase
    {
        protected virtual string AppSettingsSection => "AppSettings";

        private readonly IConfiguration _configuration;

        protected AppSettingsBase(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Bind() => _configuration.GetSection(AppSettingsSection).Bind(this);
    }
}
