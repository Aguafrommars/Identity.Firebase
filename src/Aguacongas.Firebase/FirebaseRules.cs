// Project: aguacongas/Identity.Firebase
// Copyright (c) 2020 @Olivier Lefebvre
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Aguacongas.Firebase
{
    /// <summary>
    /// Firebase rules
    /// </summary>
    public class FirebaseRules
    {
        /// <summary>
        /// Gets or sets rules property
        /// </summary>
        [JsonProperty(PropertyName = "rules")]
        public Dictionary<string, object> Rules { get; set; }
    }
}
