// Copyright Matthias Koch, Sebastian Karasek 2018.
// Distributed under the MIT License.
// https://github.com/nuke-build/nuke/blob/master/LICENSE

// Generated with Nuke.CodeGeneration, Version: 0.4.0 [CommitSha: c494ebb7].

using JetBrains.Annotations;
using Nuke.Common;
using Nuke.Common.Execution;
using Nuke.Common.Tooling;
using Nuke.Common.Tools;
using Nuke.Common.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;

namespace Nuke.CoberturaConverter
{
    #region DotCoverConversionSettings
    /// <summary><p>Used within <see cref="CoberturaConverterTasks"/>.</p></summary>
    [PublicAPI]
    [ExcludeFromCodeCoverage]
    [Serializable]
    public partial class DotCoverConversionSettings : ToolSettings
    {
        /// <summary><p>Path pointing to a dotCover DetailedXML report</p></summary>
        public virtual string InputFile { get; internal set; }
        /// <summary><p>Output file for the cobertura report</p></summary>
        public virtual string OutputFile { get; internal set; }
        protected override void AssertValid()
        {
            base.AssertValid();
            ControlFlow.Assert(File.Exists(InputFile), "File.Exists(InputFile)");
            ControlFlow.Assert(OutputFile != null, "OutputFile != null");
        }
    }
    #endregion
    #region OpenCoverConversionSettings
    /// <summary><p>Used within <see cref="CoberturaConverterTasks"/>.</p></summary>
    [PublicAPI]
    [ExcludeFromCodeCoverage]
    [Serializable]
    public partial class OpenCoverConversionSettings : ToolSettings
    {
        /// <summary><p>Path pointing to a OpenCover Xml report</p></summary>
        public virtual string InputFile { get; internal set; }
        /// <summary><p>Output file for the cobertura report</p></summary>
        public virtual string OutputFile { get; internal set; }
        protected override void AssertValid()
        {
            base.AssertValid();
            ControlFlow.Assert(File.Exists(InputFile), "File.Exists(InputFile)");
            ControlFlow.Assert(OutputFile != null, "OutputFile != null");
        }
    }
    #endregion
    #region DotCoverConversionSettingsExtensions
    /// <summary><p>Used within <see cref="CoberturaConverterTasks"/>.</p></summary>
    [PublicAPI]
    [ExcludeFromCodeCoverage]
    public static partial class DotCoverConversionSettingsExtensions
    {
        #region InputFile
        /// <summary><p><em>Sets <see cref="DotCoverConversionSettings.InputFile"/>.</em></p><p>Path pointing to a dotCover DetailedXML report</p></summary>
        [Pure]
        public static DotCoverConversionSettings SetInputFile(this DotCoverConversionSettings toolSettings, string inputFile)
        {
            toolSettings = toolSettings.NewInstance();
            toolSettings.InputFile = inputFile;
            return toolSettings;
        }
        /// <summary><p><em>Resets <see cref="DotCoverConversionSettings.InputFile"/>.</em></p><p>Path pointing to a dotCover DetailedXML report</p></summary>
        [Pure]
        public static DotCoverConversionSettings ResetInputFile(this DotCoverConversionSettings toolSettings)
        {
            toolSettings = toolSettings.NewInstance();
            toolSettings.InputFile = null;
            return toolSettings;
        }
        #endregion
        #region OutputFile
        /// <summary><p><em>Sets <see cref="DotCoverConversionSettings.OutputFile"/>.</em></p><p>Output file for the cobertura report</p></summary>
        [Pure]
        public static DotCoverConversionSettings SetOutputFile(this DotCoverConversionSettings toolSettings, string outputFile)
        {
            toolSettings = toolSettings.NewInstance();
            toolSettings.OutputFile = outputFile;
            return toolSettings;
        }
        /// <summary><p><em>Resets <see cref="DotCoverConversionSettings.OutputFile"/>.</em></p><p>Output file for the cobertura report</p></summary>
        [Pure]
        public static DotCoverConversionSettings ResetOutputFile(this DotCoverConversionSettings toolSettings)
        {
            toolSettings = toolSettings.NewInstance();
            toolSettings.OutputFile = null;
            return toolSettings;
        }
        #endregion
    }
    #endregion
    #region OpenCoverConversionSettingsExtensions
    /// <summary><p>Used within <see cref="CoberturaConverterTasks"/>.</p></summary>
    [PublicAPI]
    [ExcludeFromCodeCoverage]
    public static partial class OpenCoverConversionSettingsExtensions
    {
        #region InputFile
        /// <summary><p><em>Sets <see cref="OpenCoverConversionSettings.InputFile"/>.</em></p><p>Path pointing to a OpenCover Xml report</p></summary>
        [Pure]
        public static OpenCoverConversionSettings SetInputFile(this OpenCoverConversionSettings toolSettings, string inputFile)
        {
            toolSettings = toolSettings.NewInstance();
            toolSettings.InputFile = inputFile;
            return toolSettings;
        }
        /// <summary><p><em>Resets <see cref="OpenCoverConversionSettings.InputFile"/>.</em></p><p>Path pointing to a OpenCover Xml report</p></summary>
        [Pure]
        public static OpenCoverConversionSettings ResetInputFile(this OpenCoverConversionSettings toolSettings)
        {
            toolSettings = toolSettings.NewInstance();
            toolSettings.InputFile = null;
            return toolSettings;
        }
        #endregion
        #region OutputFile
        /// <summary><p><em>Sets <see cref="OpenCoverConversionSettings.OutputFile"/>.</em></p><p>Output file for the cobertura report</p></summary>
        [Pure]
        public static OpenCoverConversionSettings SetOutputFile(this OpenCoverConversionSettings toolSettings, string outputFile)
        {
            toolSettings = toolSettings.NewInstance();
            toolSettings.OutputFile = outputFile;
            return toolSettings;
        }
        /// <summary><p><em>Resets <see cref="OpenCoverConversionSettings.OutputFile"/>.</em></p><p>Output file for the cobertura report</p></summary>
        [Pure]
        public static OpenCoverConversionSettings ResetOutputFile(this OpenCoverConversionSettings toolSettings)
        {
            toolSettings = toolSettings.NewInstance();
            toolSettings.OutputFile = null;
            return toolSettings;
        }
        #endregion
    }
    #endregion
}
