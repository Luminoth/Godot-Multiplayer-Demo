* https://docs.godotengine.org/en/stable/tutorials/networking/high_level_multiplayer.html
* https://www.youtube.com/watch?v=_ItA2r69c-Q

# GameLift SDK

* https://docs.aws.amazon.com/gamelift/latest/developerguide/integration-server-sdk5-csharp.html
* Add .gdignore to GameLift SDK directory if under Godot project directory
  * This doesn't actually seem to work
* msbuild GameLiftServerSDK.csproj /restore
* msbuild GameLiftServerSDK.csproj /property:Configuration=Release
* Copy bin/Release/net462/GameLiftServerSDK.dll to Godot project
  * Add GameLiftServerSDK ItemGroup to .csproj
