* https://godotengine.org/article/multiplayer-in-godot-4-0-scene-replication/
* https://docs.godotengine.org/en/stable/tutorials/networking/high_level_multiplayer.html
* https://www.youtube.com/watch?v=_ItA2r69c-Q
* https://www.youtube.com/watch?v=e0JLO_5UgQo
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
  * bin/Release/net462/Polly.dll
  * bin/Release/net462/Newtonsoft.Json.dll
  * Add ItemGroup with dependencies to .csproj
* Add ConfigurationManager NuGet package (for log4net)
  * dotnet add package System.Configuration.ConfigurationManager
* Add Security Permissions NuGet package (for Newtonsoft.Json)
  * dotnet add package System.Security.Permissions

## Testing Locally (?old?)

* Test using GameLiftLocal
  * https://docs.aws.amazon.com/gamelift/latest/developerguide/integration-testing-local.html
  * java -jar GameLiftLocal.jar
    * Can override the port here if necessary, defaults to 8080
  * aws gamelift describe-game-sessions --endpoint-url http://localhost:8080 --fleet-id fleet-123
  * aws gamelift create-game-session --endpoint-url http://localhost:8080 --maximum-player-session-count 2 --fleet-id fleet-123
  * aws gamelift describe-instances --endpoint-url http://localhost:8080 --fleet-id fleet-123

## Testing Locally (Anywhere)

* https://docs.aws.amazon.com/gamelift/latest/developerguide/fleets-creating-anywhere.html
