using System;
using System.Collections;
using System.Collections.Generic;
using StrawberryShake;

namespace Elsa.OrchardCore
{
    [System.CodeDom.Compiler.GeneratedCode("StrawberryShake", "11.0.0")]
    public partial class VersionOptionsInput
    {
        public Optional<bool?> AllVersions { get; set; }

        public Optional<bool?> Draft { get; set; }

        public Optional<bool?> Latest { get; set; }

        public Optional<bool?> LatestOrPublished { get; set; }

        public Optional<bool?> Published { get; set; }

        public Optional<int?> Version { get; set; }
    }
}
