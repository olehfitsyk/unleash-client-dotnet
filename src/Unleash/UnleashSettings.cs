using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Unleash.Communication;
using Unleash.Internal;
using Unleash.Scheduling;
using Unleash.Serialization;

namespace Unleash
{
    /// <summary>
    /// Unleash settings
    /// </summary>
    public class UnleashSettings
    {
        internal readonly Encoding Encoding = Encoding.UTF8;

        internal readonly string FeatureToggleFilename = "unleash.toggles.json";
        internal readonly string EtagFilename = "unleash.etag.txt";

        /// <summary>
        /// Gets the version of unleash client running.
        /// </summary>
        public string SdkVersion { get; } = GetSdkVersion();

        /// <summary>
        /// Gets or set the uri for the backend unleash server.
        /// 
        /// Default: http://unleash.herokuapp.com/
        /// </summary>
        public Uri UnleashApi { get; set; } = new Uri("http://unleash.herokuapp.com/");

        /// <summary>
        /// Gets or sets an application name. Used for communication with backend api.
        /// </summary>
        public string AppName { get; set; } = "my-awesome-app";

        /// <summary>
        /// Gets or sets an instance tag. Used for communication with backend api.
        /// </summary>
        public string InstanceTag { get; set; } = "Dev";

        /// <summary>
        /// Gets or sets the interval in which feature toggle changes are re-fetched.
        /// 
        /// Default: 30 seconds
        /// </summary>
        public TimeSpan FetchTogglesInterval { get; set; } = TimeSpan.FromSeconds(30);

        /// <summary>
        /// Gets or sets the interval in which metriccs are sent to the server. When null, no metrics is sent.
        /// 
        /// Default: null
        /// </summary>
        public TimeSpan? SendMetricsInterval { get; set; } = null;

        /// <summary>
        /// Gets or set a directory for storing temporary files (toggles and current etag values). 
        /// 
        /// Default: Path.GetTempPath()
        /// </summary>
        public Func<string> LocalStorageFolder { get; set; } = Path.GetTempPath;

        /// <summary>
        /// Gets or sets a collection of custom http headers which will be included when communicating with the backend server.
        /// </summary>
        public Dictionary<string, string> CustomHttpHeaders { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Gets or sets the unleash context provider. This is needed when using any of the activation strategies 
        /// that needs application specific context like userid etc.
        /// 
        /// Default: A provider with no context.
        /// </summary>
        public IUnleashContextProvider UnleashContextProvider { get; set; } = new DefaultUnleashContextProvider();

        /// <summary>
        /// Gets or sets a json serializer. 
        /// 
        /// Default: A serializer based on Newtonsoft will be used, given that these assemblies are loaded into the appdomain already.
        /// </summary>
        public IJsonSerializer JsonSerializer { get; set; } = new DynamicNewtonsoftJsonSerializer();

        /// <summary>
        /// Get or sets a factory class for creating the HttpClient instance used for communicating with the backend.
        /// </summary>
        public IHttpClientFactory HttpClientFactory { get; set; } = new DefaultHttpClientFactory();

        /// <summary>
        /// Gets or sets the scheduled task manager used for syncing feature toggles and metrics with the backend in the background.
        /// Default: An implementation based on System.Threading.Timers
        /// </summary>
        public IUnleashScheduledTaskManager ScheduledTaskManager { get; set; } = new SystemTimerScheduledTaskManager();


        /// <summary>
        /// INTERNAL: Gets or sets an api client instance. Can be used for testing/mocking etc.
        /// </summary>
        internal IUnleashApiClient UnleashApiClient { get; set; }

        /// <summary>
        /// INTERNAL: Gets or sets the file system abstraction. Can be used for testing/mocking etc.
        /// </summary>
        internal IFileSystem FileSystem { get; set; } 

        private static string GetSdkVersion()
        {

            var assemblyName = Assembly.GetExecutingAssembly().GetName();
            var version = assemblyName.Version.ToString(3);

            return $"v{version}";
        }

        /// <summary>
        /// Returns info about the unleash setup.
        /// </summary>
        public override string ToString()
        {
            var sb = new StringBuilder("## Unleash settings ##");

            sb.AppendLine($"Application name: {AppName}");
            sb.AppendLine($"Instance tag: {InstanceTag}");
            sb.AppendLine($"Server Uri: {UnleashApi}");
            sb.AppendLine($"Sdk version: {SdkVersion}");

            sb.AppendLine($"Fetch toggles interval: {FetchTogglesInterval.TotalSeconds} seconds");
            var metricsInterval = SendMetricsInterval.HasValue
                ? $"{SendMetricsInterval.Value.TotalSeconds} seconds"
                : "newer";
            sb.AppendLine($"Send metrics interval: {metricsInterval}");

            sb.AppendLine($"Local storage folder: {LocalStorageFolder()}");
            sb.AppendLine($"Backup file: {FeatureToggleFilename}");
            sb.AppendLine($"Etag file: {EtagFilename}");

            sb.AppendLine($"HttpClient Factory: {HttpClientFactory.GetType().Name}");
            sb.AppendLine($"Json serializer: {JsonSerializer.GetType().Name}");
            sb.AppendLine($"Context provider: {UnleashContextProvider.GetType().Name}");

            return sb.ToString();
        }
    }
}