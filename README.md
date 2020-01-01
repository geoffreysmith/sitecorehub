#Sitecorehub

**_Sitecorehub_** is a lightweight management UI to centralize and make building [sitecore/docker-images](https://github.com/Sitecore/docker-images) simpler, visual and more intuitive than the current process. For those familiar with the existing project, it is to replace some aspects of the current build pipeline and at its core act as a replacement for the Assets image via a Web API. Streaming from a blob/API reduces the need for creating a large assets image locally while preserving the same reduction in layers.

## Current Functionality

As of right now, the code will load all 9.x asset files and display the depedencies per version, along with indicating if they exist in the blob container:

![Home](/assets/home_image.png)

They also filter per version, and images that exist in the blob:

![Blobs](/assets/blob_view.png)

The blobs were uploaded manually (see known limitations section).

## Prequisites

### Windows "fast ring" insider pre-release:

Major|Minor|Build|Revision|
---|---|---|---|---|
10|0|19536|0|

### [WSL2](https://docs.microsoft.com/en-us/windows/wsl/wsl2-install)
While cross-platform, documentation assumes any depedencies (such as Azurite) are installed and accessd via Linux, e.g., 0.0.0.0/localhost resolves correctly and Linux style directory structure. The project itself is intentionally agnostic and cross-platform. Ironically, as a tool for a build process, the build process for the build process tool works well given the prerequisites and can be improved upon later.

### .NET Core 3.1
Currently only supported release as .NET Core is deprecated as of 12/23/2019. See here to install [.NET Core 3.1](https://dotnet.microsoft.com/download/dotnet-core/3.1)

### Visual Studio 2019 with Containers Tooling Suport
Due to the early release nature of the project, it relies on the Visual Studio ["container fast mode"](https://aka.ms/containerfastmode). This invokes Visual Studio targets based off of preferences for Linux distribution set in Windows Deskto for Docker. This is not a hard dependency it runs just fine without having use the Dockerfile. The project itself is intentionally cross-platform and OS agnostic once built.

### [Azurite](https://github.com/Azure/Azurite)
For local blob emulation. I created a persistent volume in `ProgramData/azurite`, running in blob only mode

`mkdirk /mnt/c/programdata/azurite`
`docker run -p 10000:10000 -v /mnt/c/programdata/azurite:/workspace mcr.microsoft.com/azure-storage/azurite azurite-blob --blobHost 0.0.0.0`

> I highly recommend [Azure Storage Explorer](https://azure.microsoft.com/en-us/features/storage-explorer/) and testing the connection using the default connectionstring in the `appsettings.json`.

## Roadmap / Limitations

* To download the files from Sitecore, credentials are needed. These are usually set in the Azure KeyVault, but there's no way to emulate requests to a Azure KeyVault with Azure Functions. Per the latest documentation [azure-functions-host issue #3907](https://github.com/Azure/azure-functions-host/issues/3907), suggests using seaparate code paths (e.g., `Environment.GetEnvironmentVariable("WEBSITE_INSTANCE_ID`, see [here](https://stackoverflow.com/questions/45026215/how-to-check-azure-function-is-running-on-local-environment-roleenvironment-i). This is something I am specifically trying to avoid, multiple codepaths for multiple environments. I will be looking at [this](https://www.npmjs.com/package/azure-keyvault-emulator) as a viable option, though I'm obviously not wanting to continuously add non-MSFT approved emulation for local development.
* Working WebAPI similar to the asset image without needing to build it locally, nor go through the entire build process to generate a single change to a Dockerfile.
* Generate csproj files for all microservices using [hosting startup assemblies in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/platform-specific-configuration?view=aspnetcore-3.1). Right now adding a plugin to the Sitecore engine is the reverse of what it should be. It involves editing deps json instead of adding a reference to a plugin and the json being generated automatically. This requires you to update all SQL connections/config settings by hand. If this works a side effect would be easier to read DockerFiles as you're generating a real csproj. 
* The assets image correctly takes non-scwdp zip files and extracts them. Blob storage does not allow compression/deprecompression **in situ**. An Azure Function must be run to decompress the files during an upload pipeline, but requires adding the files be added as BatchFiles which the emulator does not support currently. If WSL2 is to be a requirement, the simplest solution is simply unzip in a local folder, overcoming Windows file length restrictions (if you unzip in anything other then something close to your root, the path name is too long on Windows). Since EXT4 is being called directly in WSL2 the max path length of 4096 should not be an issue, but I do not how and if this works. There's huge performance issues depending on how the operation is called, see [wsl issue #4197](https://github.com/microsoft/WSL/issues/4197). Basically if I run it in IIS Express context, despite being a Linux project I do not know if it is hitting EXT4 directly or through /mnt/c/
* Adding support for .update (TDS) files in Azure Toolbox. Some thid-party or internal parties, not OSS but proprietary, provide .update files which Sitecore Azure toolbox will not convert to a bacpac. Despite online documentation stating otherwise the module comments specifically states .update files will only convert scwdp files, not item files. There is however, in the source a workaround that if you decompile it and give it an update file creates a temporary sproc. From that a bacpac may be created. This might be a separate project but for the time being will include work I have done in this repoistory.
* Adding rudimentary checking a non-Sitecore package is compatible with the version of Sitecore. Again, I dealt with vendors/internal teams which provide packages or VS Solutions that are said to work against a certain version but do not. Currently the docker-image project will take any additional packages and blindly install them, which does work if the files are provided correctly, such as the order of operation on installation does not and should not matter.  Since files can be delivered in a variety of manner I developed a tool that takes a package, .update, scwdp or solution/csproj and attempts to convert into a nuget package similar to [Reverse Package Search](https://packagesearch.azurewebsites.net/), but based on the final build output of whatever Sitecore version you're targeting. An easy way to check if someone has Sitecore 7.5 installed, did a bunch of binding redirects and manual dll copies and delivered you a bad package.