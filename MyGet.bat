%NUGET% restore Unisync.Async.sln -NoCache -NonInteractive -ConfigFile Nuget.config
msbuild Unisync.Async.sln /p:Configuration=Release