/*
 * This file is part of StreamSDR.
 *
 * StreamSDR is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * StreamSDR is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with StreamSDR. If not, see <https://www.gnu.org/licenses/>.
 */

using Cake.Common.Tools.VSWhere;
using Cake.Common.Tools.VSWhere.Latest;

namespace StreamSDR.Build;

/// <summary>
/// Controls the global lifetime of the Cake build.
/// </summary>
public sealed class BuildLifetime : FrostingLifetime<BuildContext>
{
    /// <summary>
    /// Setup executed before the build. Used to locate MSBuild and CMake on the Windows platform.
    /// </summary>
    /// <param name="context">The build context.</param>
    public override void Setup(BuildContext context)
    {
        context.EnsureDirectoryExists(context.OutputFolder);

        if (context.Platform == "win")
        {
            // Find Visual Studio
            DirectoryPath? installationPath = context.VSWhereLatest(new VSWhereLatestSettings
            {
                IncludePrerelease = true,
                Requires = "Microsoft.Component.MSBuild Microsoft.VisualStudio.ComponentGroup.VC.Tools.142.x86.x64 Microsoft.VisualStudio.Component.VC.CMake.Project",
                Version = "17.0"
            });

            // Find MSBuild and check it is installed
            FilePath? msBuildPath = installationPath?.CombineWithFilePath("./MsBuild/Current/Bin/amd64/MSBuild.exe");
            if (msBuildPath != null && context.FileExists(msBuildPath))
            {
                context.MsBuildPath = msBuildPath;
            }

            // Find CMake and check it is installed
            FilePath? cMakePath = installationPath?.CombineWithFilePath("./Common7/IDE/CommonExtensions/Microsoft/CMake/CMake/bin/cmake.exe");
            if (cMakePath != null && context.FileExists(cMakePath))
            {
                context.CMakePath = cMakePath;
            }
        }
    }

    /// <summary>
    /// Teardown executed after the build. Empty method required to inherit from <see cref="FrostingLifetime{TContext}"/>.
    /// </summary>
    /// <param name="context">The build context.</param>
    /// <param name="info">Teardown information.</param>
    public override void Teardown(BuildContext context, ITeardownContext info) { }
}
