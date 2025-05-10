# SimplePictureCapture

A [ResoniteModLoader](https://github.com/resonite-modding-group/ResoniteModLoader) mod for [Resonite](https://resonite.com/) that adds a way to take pictures that go into the screenshots folder from ProtoFlux.

To use this just put a comment component with the text `me.art0007i.SimplePictureCapture` on the same slot as the camera. Then use the `RenderToTextureAsset` ProtoFlux node on that camera.

Additionally this mod will parent the taken pictures under the first slot of the `ExcludeRender` list of the camera. (If the ExcludeRender list is empty, or the first element is null, this feature will be disabled)

This mod is a workaround for [#515](https://github.com/Yellow-Dog-Man/Resonite-Issues/issues/515)

## Installation
1. Install [ResoniteModLoader](https://github.com/resonite-modding-group/ResoniteModLoader).
1. Place [SimplePictureCapture.dll](https://github.com/art0007i/SimplePictureCapture/releases/latest/download/SimplePictureCapture.dll) into your `rml_mods` folder. This folder should be at `C:\Program Files (x86)\Steam\steamapps\common\Resonite\rml_mods` for a default install. You can create it if it's missing, or if you launch the game once with ResoniteModLoader installed it will create the folder for you.
1. Start the game. If you want to verify that the mod is working you can check your Resonite logs.
