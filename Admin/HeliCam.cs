using Client.Cameras;
using RAGE;
using RAGE.Elements;
using RAGE.Game;
using System;
using System.Collections.Generic;

namespace Client.Admin
{
    internal class HeliCam : Events.Script
    {
    private bool helicam = false;
    private float fov_max = 80.0f;
    private float fov_min = 10.0f;
    private float zoomspeed = 2.0f;
    private float speed_lr = 3.0f;
    private float speed_ud = 3.0f;
    private int toggle_vision = 25;
    private int toggle_lock_on = 22;    
    private float fov;
    private int vision_state = 0;
    private int cam = -1;
    private RAGE.Elements.Vehicle locked_on_vehicle = null;
    private Scaleform scaleform;

    public HeliCam()
    {
        Events.Tick += Render;
            Events.Add("client:HeliCam", HeliCamToggle);
    }

        private void HeliCamToggle(object[] args)
    {
            if (helicam)
            {
                RAGE.Game.Invoker.Invoke(0x0F07E7745A236711);
                RAGE.Game.Invoker.Invoke(0x31B73D1EA9F01DA2);
                RAGE.Game.Cam.RenderScriptCams(false, false, 0, true, false, 0);
                if (scaleform != null)
                {
                    scaleform.Dispose();
                }

                if (cam != -1)
                {
                    RAGE.Game.Cam.DestroyCam(cam, true);
                    cam = -1;
                }

                helicam = false;
                RAGE.Game.Graphics.SetSeethrough(false);
                RAGE.Game.Graphics.SetNightvision(false);
                vision_state = 0;
                locked_on_vehicle = null;
            }
            else
            {
                RAGE.Game.Graphics.SetTimecycleModifier("heliGunCam");
                RAGE.Game.Graphics.SetTimecycleModifierStrength(0.3f);

                scaleform = new Scaleform("HELI_CAM");
                RAGE.Elements.Player p = RAGE.Elements.Player.LocalPlayer;
                RAGE.Elements.Vehicle heli = p.Vehicle;

                cam = RAGE.Game.Cam.CreateCamWithParams("DEFAULT_SCRIPTED_FLY_CAMERA", p.Position.X, p.Position.Y, p.Position.Z, p.GetRotation(5).X, p.GetRotation(5).Y, p.GetRotation(5).Z, 60f, true, 2);
                RAGE.Game.Cam.SetCamActive(cam, true);
                
                RAGE.Game.Cam.SetCamActive(cam, true);
                RAGE.Game.Cam.SetCamRot(cam, 0f, 0f, heli.GetHeading(), 5);
                fov = (fov_max + fov_min) * 0.5f;
                RAGE.Game.Cam.SetCamFov(cam, fov);
                RAGE.Game.Cam.RenderScriptCams(true, false, 0, true, false, 0);
                RAGE.Game.Cam.AttachCamToEntity(cam, heli.Id, 0f, 0f, -1.5f, true);

                RAGE.Game.Graphics.PushScaleformMovieFunction(scaleform.Handle, "SET_CAM_LOGO");
                RAGE.Game.Graphics.PushScaleformMovieFunctionParameterInt(1);
                RAGE.Game.Graphics.PopScaleformMovieFunctionVoid();

                helicam = true;
            }
        
    }

    private void Render(List<Events.TickNametagData> nametags)
    {
            RAGE.Elements.Player p = RAGE.Elements.Player.LocalPlayer;
            if (helicam)
        {
            if (cam != -1 && RAGE.Game.Cam.IsCamActive(cam) && RAGE.Game.Cam.IsCamRendering(cam))
            {
                Pad.DisableAllControlActions(2);

                float x = Pad.GetDisabledControlNormal(7, 1) * speed_lr;
                float y = Pad.GetDisabledControlNormal(7, 2) * speed_ud;
                float zoomIn = Pad.GetDisabledControlNormal(2, 40) * zoomspeed;
                float zoomOut = Pad.GetDisabledControlNormal(2, 41) * zoomspeed;

                    Vector3 currentRot = RAGE.Game.Cam.GetCamRot(cam, 5);
                currentRot = new Vector3(currentRot.X - y, 0, currentRot.Z - x);
                    RAGE.Game.Cam.SetCamRot(cam, currentRot.X, currentRot.Y, currentRot.Z, 5);

                if (zoomIn > 0)
                {
                    float currentFov = RAGE.Game.Cam.GetCamFov(cam);
                    currentFov -= zoomIn;
                    if (currentFov < fov_min) currentFov = fov_min;
                        RAGE.Game.Cam.SetCamFov(cam,currentFov);
                    }
                else if (zoomOut > 0)
                {
                    float currentFov = RAGE.Game.Cam.GetCamFov(cam);
                        currentFov += zoomOut;
                    if (currentFov > fov_max) currentFov = fov_max;
                        RAGE.Game.Cam.SetCamFov(cam, currentFov);
                    }
            }

            if (Pad.IsDisabledControlJustPressed(0, toggle_vision))
            {
                RAGE.Game.Audio.PlaySoundFrontend(-1, "SELECT", "HUD_FRONTEND_DEFAULT_SOUNDSET", false);
                ChangeVision();
            }

                if (locked_on_vehicle != null)
                {
                    RAGE.Game.Cam.PointCamAtCoord(cam, locked_on_vehicle.Position.X, locked_on_vehicle.Position.Y, locked_on_vehicle.Position.Z);
                    RenderVehicleInfo(locked_on_vehicle);
                    if (Pad.IsDisabledControlJustPressed(0, toggle_lock_on))
                    {
                        RAGE.Game.Audio.PlaySoundFrontend(-1, "SELECT", "HUD_FRONTEND_DEFAULT_SOUNDSET", false);
                        locked_on_vehicle = null;
                        
                        RAGE.Elements.Vehicle heli = p.Vehicle;
                        Vector3 currentRot = RAGE.Game.Cam.GetCamRot(cam, 5);
                        float currentFov = RAGE.Game.Cam.GetCamFov(cam);
                        RAGE.Game.Cam.DestroyCam(cam, true);

                        cam = RAGE.Game.Cam.CreateCamWithParams("DEFAULT_SCRIPTED_FLY_CAMERA", p.Position.X, p.Position.Y, p.Position.Z, p.GetRotation(5).X, p.GetRotation(5).Y, p.GetRotation(5).Z, 60f, true, 2);
                        RAGE.Game.Cam.SetCamActive(cam, true);
                        RAGE.Game.Cam.SetCamRot(cam, 0f, 0f, heli.GetHeading(), 5);
                        RAGE.Game.Cam.SetCamFov(cam, fov);
                        RAGE.Game.Cam.RenderScriptCams(true, false, 0, true, false, 0);
                        RAGE.Game.Cam.AttachCamToEntity(cam, heli.Id, 0f, 0f, -1.5f, true);
                    }
                    else
                    {
                        locked_on_vehicle = null;
                        RAGE.Elements.Vehicle heli = p.Vehicle;
                        Vector3 currentRot = RAGE.Game.Cam.GetCamRot(cam, 5);
                        float currentFov = RAGE.Game.Cam.GetCamFov(cam);

                        RAGE.Game.Cam.DestroyCam(cam, true);
                        cam = RAGE.Game.Cam.CreateCamWithParams("DEFAULT_SCRIPTED_FLY_CAMERA", p.Position.X, p.Position.Y, p.Position.Z, p.GetRotation(5).X, p.GetRotation(5).Y, p.GetRotation(5).Z, 60f, true, 2);
                        RAGE.Game.Cam.SetCamActive(cam, true);
                        RAGE.Game.Cam.SetCamRot(cam, 0f, 0f, heli.GetHeading(), 5);
                        RAGE.Game.Cam.SetCamFov(cam, fov);
                        RAGE.Game.Cam.RenderScriptCams(true, false, 0, true, false, 0);
                        RAGE.Game.Cam.AttachCamToEntity(cam, heli.Id, 0f, 0f, -1.5f, true);
                    }
                }
                else
                {
                    /*RAGE.Elements.Vehicle vehicle_detected = PointingAt(cam);
                    if (vehicle_detected != null)
                    {
                        if (Pad.IsDisabledControlJustPressed(0, toggle_lock_on))
                        {
                            Audio.PlaySoundFrontend(-1, "SELECT", "HUD_FRONTEND_DEFAULT_SOUNDSET", false);
                            locked_on_vehicle = vehicle_detected;
                        }
                    }*/
                }

                RAGE.Game.Graphics.PushScaleformMovieFunction(scaleform.Handle, "SET_ALT_FOV_HEADING");
               
                RAGE.Game.Graphics.PushScaleformMovieFunctionParameterFloat(p.Position.Z);
                RAGE.Game.Graphics.PushScaleformMovieFunctionParameterFloat(RAGE.Game.Cam.GetCamFov(cam));
                
                RAGE.Game.Graphics.PushScaleformMovieFunctionParameterFloat(RAGE.Game.Cam.GetCamRot(cam, 5).Z);
                RAGE.Game.Graphics.PopScaleformMovieFunctionVoid();

                RAGE.Game.Graphics.DrawScaleformMovieFullscreen(scaleform.Handle, 255, 255, 255, 255, 0);

            RAGE.Game.Graphics.PopScaleformMovieFunctionVoid();
        }
    }

    private void ChangeVision()
    {
        if (vision_state == 0)
        {
            RAGE.Game.Graphics.SetNightvision(true);
            vision_state = 1;
        }
        else if (vision_state == 1)
        {
            RAGE.Game.Graphics.SetNightvision(true);
            RAGE.Game.Graphics.SetSeethrough(true);
            vision_state = 2;
        }
        else
        {
            RAGE.Game.Graphics.SetSeethrough(false);
            RAGE.Game.Graphics.SetNightvision(false);
            vision_state = 0;
        }
    }

    private void RenderVehicleInfo(RAGE.Elements.Vehicle vehicle)
    {
        string vehname = RAGE.Game.Ui.GetLabelText(RAGE.Game.Vehicle.GetDisplayNameFromVehicleModel((uint)vehicle.Model));
        string licenseplate = vehicle.GetNumberPlateText();

        //RAGE.Game.GraphicAPI.DrawText($"Model: {vehname}\nPlate: {licenseplate}", 0.5f, 0.9f, 0, 255, 255, 255, 185, 0, 0, 0, 0, 0);
    }
        /*
    private RAGE.Elements.Vehicle PointingAt(int camera)
    {
        float distance = 100;
        Vector3 position = RAGE.Game.Cam.GetCamCoord(cam);
        Vector3 direction = 

            camera.Direction;
        Vector3 farAway = new Vector3(direction.X * distance + position.X, direction.Y * distance + position.Y, direction.Z * distance + position.Z);

        API.DrawLine(position.X, position.Y, position.Z, farAway.X, farAway.Y, farAway.Z, 255, 0, 0, 255); // Is in line of sight
        var result = API.Raycast(position, farAway, (int)(IntersectOptions.Everything | IntersectOptions.Map | IntersectOptions.Objects), null);

        if (result.DidHitEntity)
        {
            if (result.HitEntity.Type == EntityType.Player) return null;
            if (result.HitEntity.Type == EntityType.Vehicle) return new Vehicle(result.HitEntity.Handle);
            return null;
        }
        return null;
    }*/

    }
}