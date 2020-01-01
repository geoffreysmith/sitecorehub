#Sitecorehub

**_Sitecorehub_** is a lightweight management UI to centralize and make building [sitecore/docker-images](https://github.com/Sitecore/docker-images) simpler, visual and more intuitive than the current process. For those familiar with the existing project, it is to replace some aspects of the current build pipeline and at its core act as a replacement for the Assets image via a Web API. Streaming from a blob/API reduces the need for creating a large assets image locally while preserving the same reduction in layers.

## Prequisites

### Windows "fast ring" insider pre-release:

Major|Minor|Build|Revision
---|---|---|---|---
10|0|19536|0

### [WSL2](https://docs.microsoft.com/en-us/windows/wsl/wsl2-install)
While cross-platform, documentation assumes any depedencies (such as Azurite) are installed and access via Linux, e.g., localhost resolves correctly and Linux style directory structure. The project itself is intentionally agnostic and cross-platform. The build process is a work in progress.

### .NET Core 3.1
Currently only supported release as .NET Core is deprecated as of 12/23/2019. See here to install [.NET Core 3.1](https://dotnet.microsoft.com/download/dotnet-core/3.1)

###Visual Studio 2019 with Containers Tooling Suport
Due to the early release nature of the project, it relies on the Visual Studio ["container fast mode"](https://aka.ms/containerfastmode). This invokes Visual Studio targets based off of preferences for Linux distribution set in Windows Deskto for Docker. This is not a hard dependency it runs just fine without having use the Dockerfile. The project itself is intentionally cross-platform and OS agnostic once built.

### [Azurite](https://github.com/Azure/Azurite)

For local blob emulation. I created a persistent volume in `ProgramData/azurite`, running in blob only mode

>mkdirk /mnt/c/programdata/azurite
>docker run -p 10000:10000 -v /mnt/c/programdata/azurite:/workspace mcr.microsoft.com/azure-storage/azurite azurite-blob --blobHost 0.0.0.0

I highly recommend [Azure Storage Explorer](https://azure.microsoft.com/en-us/features/storage-explorer/) and testing the connection using the default connectionstring:

>DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;

As this is very new, I highly recommend reading [this article](https://edi.wang/post/2019/12/5/try-the-new-azure-net-sdk) as referenced in the comments.

## Current Functionality

As of right now, the code will load all 9.x asset files and display the depedencies per version, along with indicating if they exist in the blob container:

![Home](/assets/home_image.png)

They also filter per version, and images that exist in the blob:

![Blobs](/assets/blob_view.png)

The blobs were uploaded manually (see known limitations section). This is heavily reliant two files being pulled in, I did not want to duplicate data:

`https://raw.githubusercontent.com/Sitecore/docker-images/master/windows/{scVersion}/sitecore-assets/build.json`
`https://raw.githubusercontent.com/Sitecore/docker-images/master/sitecore-packages.json`

Also, there's no MD5 hash check on the files as I see that's on the roadmap for the docker-images project, but that is an issue I ran into due to the size and poor connectionm issues with Sitecore's servers despite having a 10gbps fiber connection. Again, everything is production ready in so much as that the connection string may be swapped out. The goal is to take lessons from previous projects and have a viable environment that uses the cloud platform of choice with as little resistence as possible, while still being abble to utilize the tool locally through emulation.

## Roadmap

- Complete, if possible, the ability to set login credentials in a local keystore, download and create the equivalent of an asset image. See limitations for known Azure emulation issues. But ideally there'd be a button for each version that would allow the asset image download/extraction process.
- Creation of various microservices in a scaled architecture into separate csproj files. This would allow smaller image sizes, native Linux microservices. See [here](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/platform-specific-configuration?view=aspnetcore-3.1), previously I used decompiliation to do this for other reasons.
- My last project saw a large variety of vendor (not Sitecore) provided depedencies that did not lend themselves to an automated build process. For example the Sitecore Azure toolkit, despite claiming to support update files, does not create a bacpac/dacpac. Decompiling the source creates reveales a method to create a sproc that can be created into a bacpac/dacpac. While this may seem like overkill, it was easier to do this then support multiple installation procedures.
- Again, the last project had third parties providing updates that would require updating .NETCore 2.1 depedency files. This should not be something that is updated manually. When the project runs as a csproj the depedency may be added correctly to the csproj as a project reference and msbuild will correctly update the deps file.
- Due to the commerce composer templates updates in Sitecore 9.x+, the one consistency of templates not being manipulated by content authors is now gone making Unicorn irrelevant.
- Moving to K8s and other various graphical interfaces that make Docker more easily digestable to non-Docker developers and project stakeholders.

## Known Limitations

- Azurite does not support batch operations, it is on their roadmap. The correct or more Azure friendly way of dealing with ETL into a blob container is an Azure Storage function injected into the pipeline that does the ETL, this relies on batching being available. Data Lake supports extraction and manipulation but is not available for emulation at this point.
- While I can get this running on K8s, and then also locally with [aksengine](https://github.com/Azure/aks-engine/blob/master/docs/topics/windows.md) I think treating Sitecore Framework 4.7.1 instances as black boxes is the best way to appraoch projects in the future. Working natively in Linux solves a lot of Docker issues. Plus I'd be very surprised if Sitecore doesn't have a native .NETCore 3.1 release in the near future.
- I completely spent 3 hours today trying to get the font svg sprites looking nice against a custom svg. Sprites are outdated so I'm taking bootstrap out and just doing it by hand.