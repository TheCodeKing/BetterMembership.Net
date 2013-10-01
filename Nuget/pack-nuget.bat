pushd ..\
for /D %%D in (%SYSTEMROOT%\Microsoft.NET\Framework\v4*) do set msbuild=%%D\MSBuild.exe
%msbuild% Source\BetterMembership.Net.sln -P:Configuration=Signed
Source\.nuget\NuGet.exe pack Nuget/BetterMembership.Net.nuspec -Prop Configuration=Signed -o Nuget
popd

