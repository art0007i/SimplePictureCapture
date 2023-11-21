using HarmonyLib;
using ResoniteModLoader;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;
using FrooxEngine;
using ProtoFlux.Runtimes.Execution.Nodes.FrooxEngine.Rendering;
using System.Runtime;
using Elements.Core;
using ProtoFlux.Core;
using FrooxEngine.ProtoFlux;
using ProtoFlux.Runtimes.Execution;

namespace SimplePictureCapture
{
    public class SimplePictureCapture : ResoniteMod
    {
        public override string Name => "SimplePictureCapture";
        public override string Author => "art0007i";
        public override string Version => "1.0.0";
        public override string Link => "https://github.com/art0007i/SimplePictureCapture/";
        public override void OnEngineInit()
        {
            Harmony harmony = new Harmony("me.art0007i.SimplePictureCapture");
            harmony.PatchAll();

        }
        [HarmonyPatch(typeof(RenderToTextureAsset), "RunAsync")]
        class SimplePictureCapturePatch
        {
            // Fun fact: this mod is 99% copy pasted froox code
            // and most of it coming from interactive camera

            public static bool Prefix(RenderToTextureAsset __instance, FrooxEngineContext context, ref Task<IOperation> __result)
            {
                var cam = __instance.Camera.Evaluate(context);
                if (cam?.Slot.GetComponent<Comment>()?.Text.Value == "me.art0007i.SimplePictureCapture")
                {
                    __result = Task.Run<IOperation>(async () =>
                    {
                        var world = Engine.Current.WorldManager.FocusedWorld;
                        // get values from flux
                        var res = __instance.Resolution.Evaluate(context); // 2048,y
                        var fmt = __instance.Format.Evaluate(context, "webp"); // webp
                        var qual = __instance.Quality.Evaluate(context, 200); // 200

                        // prepare render settings
                        var renderSettings = cam.GetRenderSettings(res);

                        if (cam.World.IsUserspace())
                        {
                            // fix pos and rot (THIS IS THE CAUSE OF ALL EVIL)
                            var uroot = world.LocalUser.Root;
                            if (uroot == null)
                            {
                                Error("You have no user root? How did you click the button wtf");
                                return __instance.OnFailed.Target;
                            }
                            renderSettings.position = uroot.Slot.LocalPointToGlobal(renderSettings.position);
                            renderSettings.rotation = uroot.Slot.LocalRotationToGlobal(renderSettings.rotation);
                        }

                        // hide lasers :)
                        if (renderSettings.excludeObjects == null || cam.World.IsUserspace())
                            renderSettings.excludeObjects = new List<Slot>();
                        InteractionHandler.GetLaserRoots(world.AllUsers, renderSettings.excludeObjects);

                        // capture the photo asynchronously
                        Uri texture = await world.Render.RenderToAsset(renderSettings, fmt, qual);

                        // spawn the photo object :)
                        var photo = cam.LocalUserSpace.AddSlot("Photo in " + world.Name);
                        var tex = photo.AttachTexture(texture);

                        // Last argument is FALSE! this is because we attach metadata manually
                        ImageImporter.SetupTextureProxyComponents(photo, tex, StereoLayout.None, ImageProjection.Perspective, false);

                        PhotoMetadata meta = photo.AttachComponent<PhotoMetadata>();

                        // manually assign all the metadata, since its in a different world
                        SetupMetadataFromWorld(meta, world);

                        meta.Is360.Value = false;
                        meta.StereoLayout.Value = StereoLayout.None;

                        meta.CameraManufacturer.Value = "art0007i"; // :)
                        meta.CameraModel.Value = "SimplePictureCapture Mod";
                        meta.CameraFOV.Value = cam.FieldOfView;

                        // attach grabbable, mesh, and box collider
                        photo.AttachComponent<Grabbable>().Scalable.Value = true;

                        var model = photo.AttachMesh<QuadMesh, UnlitMaterial>();
                        model.material.Texture.Target = tex;
                        model.material.Sidedness.Value = Sidedness.Double;
                        var textureSizeDriver = photo.AttachComponent<TextureSizeDriver>();
                        textureSizeDriver.Texture.Target = tex;
                        textureSizeDriver.DriveMode.Value = TextureSizeDriver.Mode.Normalized;
                        textureSizeDriver.Target.Target = model.mesh.Size;
                        BoxCollider coll = photo.AttachComponent<BoxCollider>();
                        coll.Size.DriveFromXY(model.mesh.Size);
                        coll.Type.Value = ColliderType.NoCollision;

                        // position the picture, the idea is we use first thing of exclude render as the pic root, its actually smart trust me
                        var first = cam.IsRemoved ? null : cam.ExcludeRender.FirstOrDefault();
                        var shouldDelete = first == null;
                        if (!shouldDelete)
                        {
                            var vec = first.ChildrenCount * new float3(0, 0, 0.05f);
                            photo.Parent = first;
                            photo.LocalPosition = vec;
                            photo.LocalRotation = floatQ.Identity;
                            photo.LocalScale = float3.One;
                        }
                        else
                        {
                            photo.LocalPosition = new float3(0f, -10000f);
                        }

                        await meta.NotifyOfScreenshot();
                        if (shouldDelete)
                        {
                            photo.Destroy();
                        }

                        if (texture != null)
                        {
                            __instance.RenderedAssetURL.Write(texture, context);
                            return __instance.OnRendered.Target;
                        }
                        return __instance.OnFailed.Target;
                    });
                    return false;
                }
                return true;
            }


            public static void SetupMetadataFromWorld(PhotoMetadata meta, World wld)
            {
                meta.LocationName.Value = wld.Name;
                meta.LocationURL.Value = wld.RecordURL;
                meta.LocationHost.Target = wld.HostUser;
                meta.LocationAccessLevel.Value = wld.AccessLevel;
                meta.LocationHiddenFromListing.Value = wld.HideFromListing;
                meta.TimeTaken.Value = DateTime.UtcNow;
                meta.TakenBy.Target = meta.LocalUser;
                meta.AppVersion.Value = meta.Engine.VersionString;
                foreach (User user in wld.AllUsers)
                {
                    var userInfo = meta.UserInfos.Add();
                    userInfo.User.Target = user;
                    userInfo.SessionJoinTimestamp.Value = user.SessionJoinTimestamp;
                    userInfo.IsPresent.Value = user.IsPresent;
                    userInfo.IsInVR.Value = user.VR_Active;
                    if (user.Root != null)
                    {
                        userInfo.HeadPosition.Value = user.Root.HeadPosition;
                        userInfo.HeadOrientation.Value = user.Root.HeadRotation;
                    }
                }
                meta.TakenGlobalPosition.Value = meta.Slot.GlobalPosition;
                meta.TakenGlobalRotation.Value = meta.Slot.GlobalRotation;
                meta.TakenGlobalScale.Value = meta.Slot.GlobalScale;
            }
        }
    }
}