// Generated with Nuke.CodeGeneration version 0.19.0 (Windows,.NETStandard,Version=v2.0)

using JetBrains.Annotations;
using Newtonsoft.Json;
using Nuke.Common;
using Nuke.Common.Execution;
using Nuke.Common.Tooling;
using Nuke.Common.Tools;
using Nuke.Common.Tools.DotCover;
using Nuke.Common.Tools.OpenCover;
using Nuke.Common.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;

namespace Nuke.CoberturaConverter
{
    #region DotCoverConversionSettings
    /// <summary>
    ///   Used within <see cref="CoberturaConverterTasks"/>.
    /// </summary>
    [PublicAPI]
    [ExcludeFromCodeCoverage]
    [Serializable]
    public partial class DotCoverConversionSettings : ToolSettings
    {
        /// <summary>
        ///   Path pointing to a dotCover DetailedXML report
        /// </summary>
        public virtual string InputFile { get; internal set; }
        /// <summary>
        ///   Output file for the cobertura report
        /// </summary>
        public virtual string OutputFile { get; internal set; }

        public override Action<OutputType, string> CustomLogger => DotCoverTasks.DotCoverLogger;
    }
    #endregion
    #region OpenCoverConversionSettings
    /// <summary>
    ///   Used within <see cref="CoberturaConverterTasks"/>.
    /// </summary>
    [PublicAPI]
    [ExcludeFromCodeCoverage]
    [Serializable]
    public partial class OpenCoverConversionSettings : ToolSettings
    {
        /// <summary>
        ///   Path pointing to a OpenCover Xml report
        /// </summary>
        public virtual string InputFile { get; internal set; }
        /// <summary>
        ///   Output file for the cobertura report
        /// </summary>
        public virtual string OutputFile { get; internal set; }

        public override Action<OutputType, string> CustomLogger => OpenCoverTasks.OpenCoverLogger;
    }
    #endregion
    #region DotCoverConversionSettingsExtensions
    /// <summary>
    ///   Used within <see cref="CoberturaConverterTasks"/>.
    /// </summary>
    [PublicAPI]
    [ExcludeFromCodeCoverage]
    public static partial class DotCoverConversionSettingsExtensions
    {
        #region InputFile
        /// <summary>
        ///   <p><em>Sets <see cref="DotCoverConversionSettings.InputFile"/></em></p>
        ///   <p>Path pointing to a dotCover DetailedXML report</p>
        /// </summary>
        [Pure]
        public static DotCoverConversionSettings SetInputFile(this DotCoverConversionSettings toolSettings, string inputFile)
        {
            toolSettings = toolSettings.NewInstance();
            toolSettings.InputFile = inputFile;
            return toolSettings;
        }
        /// <summary>
        ///   <p><em>Resets <see cref="DotCoverConversionSettings.InputFile"/></em></p>
        ///   <p>Path pointing to a dotCover DetailedXML report</p>
        /// </summary>
        [Pure]
        public static DotCoverConversionSettings ResetInputFile(this DotCoverConversionSettings toolSettings)
        {
            toolSettings = toolSettings.NewInstance();
            toolSettings.InputFile = null;
            return toolSettings;
        }
        #endregion
        #region OutputFile
        /// <summary>
        ///   <p><em>Sets <see cref="DotCoverConversionSettings.OutputFile"/></em></p>
        ///   <p>Output file for the cobertura report</p>
        /// </summary>
        [Pure]
        public static DotCoverConversionSettings SetOutputFile(this DotCoverConversionSettings toolSettings, string outputFile)
        {
            toolSettings = toolSettings.NewInstance();
            toolSettings.OutputFile = outputFile;
            return toolSettings;
        }
        /// <summary>
        ///   <p><em>Resets <see cref="DotCoverConversionSettings.OutputFile"/></em></p>
        ///   <p>Output file for the cobertura report</p>
        /// </summary>
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
    /// <summary>
    ///   Used within <see cref="CoberturaConverterTasks"/>.
    /// </summary>
    [PublicAPI]
    [ExcludeFromCodeCoverage]
    public static partial class OpenCoverConversionSettingsExtensions
    {
        #region InputFile
        /// <summary>
        ///   <p><em>Sets <see cref="OpenCoverConversionSettings.InputFile"/></em></p>
        ///   <p>Path pointing to a OpenCover Xml report</p>
        /// </summary>
        [Pure]
        public static OpenCoverConversionSettings SetInputFile(this OpenCoverConversionSettings toolSettings, string inputFile)
        {
            toolSettings = toolSettings.NewInstance();
            toolSettings.InputFile = inputFile;
            return toolSettings;
        }
        /// <summary>
        ///   <p><em>Resets <see cref="OpenCoverConversionSettings.InputFile"/></em></p>
        ///   <p>Path pointing to a OpenCover Xml report</p>
        /// </summary>
        [Pure]
        public static OpenCoverConversionSettings ResetInputFile(this OpenCoverConversionSettings toolSettings)
        {
            toolSettings = toolSettings.NewInstance();
            toolSettings.InputFile = null;
            return toolSettings;
        }
        #endregion
        #region OutputFile
        /// <summary>
        ///   <p><em>Sets <see cref="OpenCoverConversionSettings.OutputFile"/></em></p>
        ///   <p>Output file for the cobertura report</p>
        /// </summary>
        [Pure]
        public static OpenCoverConversionSettings SetOutputFile(this OpenCoverConversionSettings toolSettings, string outputFile)
        {
            toolSettings = toolSettings.NewInstance();
            toolSettings.OutputFile = outputFile;
            return toolSettings;
        }
        /// <summary>
        ///   <p><em>Resets <see cref="OpenCoverConversionSettings.OutputFile"/></em></p>
        ///   <p>Output file for the cobertura report</p>
        /// </summary>
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
