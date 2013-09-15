pushd ..\
copy LICENSE bin
for /D %%D in (%SYSTEMROOT%\Microsoft.NET\Framework\v4*) do set msbuild=%%D\MSBuild.exe
%msbuild% Source\BetterMembershipProvider.sln -P:Configuration=Signed
Source\.nuget\NuGet.exe pack Nuget/BetterMembershipProvider.nuspec -Prop Configuration=Signed -o Nuget
popd

