// Blade™ Client for Grand Theft Auto V (MP)
// (C) BudPlaza & contributors
// See COPYING for more information

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using BudPlaza.BladeXClient.Ui;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using LemonUI.Menus;

namespace BudPlaza.BladeX.Client.Ui
{
    internal class CharactorCreatorMenu
    {
        private readonly NativeMenu menu = new NativeMenu("Charactor Creator", "Menu");
        private readonly NativeListItem<string> itemGender = new NativeListItem<string>("Gender", "~HUD_COLOUR_WHITE~~BLIP_INFO_ICON~ Selects the gender of this character.", "Male", "Female");
        private readonly NativeItem itemContinue = new NativeItem("Continue", "~HUD_COLOUR_WHITE~~BLIP_INFO_ICON~ Continue to the next step.");

        private PedHeadBlendData data = new PedHeadBlendData();
        private readonly NativeMenu menuBasic = new NativeMenu("Charactor Creator", "Basic Properties");
        private readonly NativeSliderItem itemRendererOne = new NativeSliderItem("Renderer 1", "The first affector of the head shape of your character.", 0, 45);
        private readonly NativeSliderItem itemRendererTwo = new NativeSliderItem("Renderer 2", "The second affector of the head shape of your character.", 0, 45);
        private readonly NativeSliderItem itemRendererThree = new NativeSliderItem("Renderer 3", "The third affector of the head shape of your character.", 0, 45);

        private bool _over = false;
        private bool _isInDetail;

        private Camera camera;

        public CharactorCreatorMenu()
        {
            camera = World.CreateCamera(new Vector3(-810f, -189f, 128f), Vector3.ForwardRH, 1.2f);
            Game.PlayerPed.Task.RunTo(Game.PlayerPed.ForwardVector);
            World.RenderingCamera = camera;

            InteractionMenu.PublicPool.Add(menu);
            menu.Add(itemGender);
            menu.Add(itemContinue);
            menu.Closing += Menu_Closing;
            menu.Visible = true;

            InteractionMenu.PublicPool.Add(menuBasic);
            menuBasic.Add(itemRendererOne);
            menuBasic.Add(itemRendererTwo);
            menuBasic.Add(itemRendererThree);
            menuBasic.Closing += Menu_Closing;

            itemContinue.Activated += (sender, e) => 
            {
                _over = true;
                SetOutfit((Gender)itemGender.SelectedIndex);
                menu.Visible = false;
                API.BeginTextCommandDisplayHelp("AMCH_BIKEAV");
                API.EndTextCommandDisplayHelp(0, false, true, 5000);
                camera.Position = GameplayCamera.Position;
                Game.PlayerPed.Heading = 250f;
                camera.PointAt(Game.PlayerPed);
                camera.FieldOfView = 60f;
                _isInDetail = true;
            };
        }

        private async void SetOutfit(Gender gender)
        {
            if (gender == Gender.Male)
            {
                await Game.Player.ChangeModel(PedHash.FreemodeMale01);
                API.SetPedRandomComponentVariation(Game.PlayerPed.Handle, false);
                API.SetPedComponentVariation(Game.PlayerPed.Handle, 0, 0, 0, 0);
                API.SetPedComponentVariation(Game.PlayerPed.Handle, 1, 0, 0, 0);
            }
            else
            {
                await Game.Player.ChangeModel(PedHash.FreemodeFemale01);
                API.SetPedComponentVariation(Game.PlayerPed.Handle, 0, 0, 0, 0);
                API.SetPedComponentVariation(Game.PlayerPed.Handle, 1, 0, 0, 0);
                API.SetPedComponentVariation(Game.PlayerPed.Handle, 3, 14, 0, 0);
                API.SetPedComponentVariation(Game.PlayerPed.Handle, 4, 12, 0, 0);
                API.SetPedComponentVariation(Game.PlayerPed.Handle, 6, 9, 0, 0);
                API.SetPedComponentVariation(Game.PlayerPed.Handle, 8, -1, 0, 0);
                API.SetPedComponentVariation(Game.PlayerPed.Handle, 11, 14, 0, 0);
            }
        }

        private void Menu_Closing(object sender, LemonUI.CancelEventArgs e)
        {
            if (!_over)
            {
                e.Cancel = true;
            }
        }

        public Task Tick()
        {
            InteractionMenu.PublicPool.Process();

            if (_isInDetail)
            {
                if (Game.IsControlPressed(0, Control.FrontendLeft))
                {
                    Game.PlayerPed.Heading -= 2.5f;
                }
                else if (Game.IsControlPressed(0, Control.FrontendRight))
                {
                    Game.PlayerPed.Heading += 2.5f;
                }
                else if (Game.IsControlPressed(0, Control.ReplayCameraUp))
                {
                    camera.Position = new Vector3(camera.Position.X, camera.Position.Y, camera.Position.Z + 0.5f);
                    camera.PointAt(Game.PlayerPed);
                }
                else if (Game.IsControlPressed(0, Control.ReplayCameraDown))
                {
                    camera.Position = new Vector3(camera.Position.X, camera.Position.Y, camera.Position.Z - 0.5f);
                    camera.PointAt(Game.PlayerPed);
                }
            }

            return Task.FromResult(0);
        }

        ~CharactorCreatorMenu()
        {
            World.RenderingCamera = null;

            if (camera?.Exists() == true)
            {
                camera.Delete();
            }
        }
    }
}
