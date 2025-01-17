﻿namespace WhMgr.Configuration
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// Url configuration class
    /// </summary>
    public class UrlConfig
    {
        /// <summary>
        /// Gets or sets the scanner map url
        /// </summary>
        [JsonPropertyName("scannerMap")]
        public string ScannerMap { get; set; }
    }
}