* https://godotengine.org/article/multiplayer-in-godot-4-0-scene-replication/
* https://docs.godotengine.org/en/stable/tutorials/networking/high_level_multiplayer.html
* https://www.youtube.com/watch?v=_ItA2r69c-Q
* https://github.com/zwometer/Godot4MultiplayerMVP

# GameLift SDK

## Building the SDK

* https://docs.aws.amazon.com/gamelift/latest/developerguide/integration-server-sdk5-csharp.html
* Add .gdignore to GameLift SDK directory if under Godot project directory
  * This doesn't actually seem to work
* msbuild GameLiftServerSDK.csproj /restore
* msbuild GameLiftServerSDK.csproj /property:Configuration=Release

## Integrating

* Copy dependencies to Godot project
  * bin/Release/net462/GameLiftServerSDK.dll
  * bin/Release/net462/log4net.dll
  * bin/Release/net462/websocket-sharp-core.dll
  * Add ItemGroup with GameLiftServerSDK, log4net, and websocket-sharp-core to .csproj
* Add ConfigurationManager NuGet package (for log4net)
  * dotnet add package System.Configuration.ConfigurationManager
