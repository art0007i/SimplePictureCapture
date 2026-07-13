# SimplePictureCapture
[![Thunderstore Badge](https://modding.resonite.net/assets/available-on-thunderstore.svg)](https://thunderstore.io/c/resonite/)

A [Resonite](https://resonite.com/) mod that adds a way to take pictures that go into the screenshots folder from ProtoFlux.

To use this just put a comment component with the text `me.art0007i.SimplePictureCapture` on the same slot as the camera. Then use the `RenderToTextureAsset` ProtoFlux node on that camera.

Additionally this mod will parent the taken pictures under the first slot of the `ExcludeRender` list of the camera. (If the ExcludeRender list is empty, or the first element is null, this feature will be disabled)

This mod is a workaround for [#515](https://github.com/Yellow-Dog-Man/Resonite-Issues/issues/515)

## Installation (Manual)
1. Install [BepisLoader](https://github.com/ResoniteModding/BepisLoader) for Resonite.
2. Download the latest release ZIP file (e.g., `art0007i-SimplePictureCapture-1.0.0.zip`) from the [Releases](https://github.com/art0007i/SimplePictureCapture/releases) page.
3. Extract the ZIP and copy the `plugins` folder to your BepInEx folder in your Resonite installation directory:
   - **Default location:** `C:\Program Files (x86)\Steam\steamapps\common\Resonite\BepInEx\`
4. Start the game. If you want to verify that the mod is working you can check your BepInEx logs.
