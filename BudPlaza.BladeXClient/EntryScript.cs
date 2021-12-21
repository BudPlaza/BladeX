// Blade™ Client for Grand Theft Auto V (MP)
// (C) BudPlaza & contributors
// See COPYING for more information

using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core.UI;
using BudPlaza.BladeXClient.Commands;
using BudPlaza.BladeXClient.Util;
using BudPlaza.BladeXClient.Ui;
using System.Runtime.InteropServices;
using LemonUI.Scaleform;

namespace BudPlaza.BladeXClient
{
    public class EntryScript : BaseScript
    {
        private bool _staminaDisplayed;
        private static InteractionMenu _menu;
        private const int _uploadTotal = 120000;
        private static bool _mi;
        private static bool _prevMi;
        private static int _miCooldown;
        private static int _miTimeout;

        private static string _alertTitle;
        private static string _alertDescription;
        private static bool _drawAlert;
        private static bool _creation;

        private static InstructionalButtons buttons;

        internal static bool IsOp { get; private set; }

        public EntryScript()
        {
            EventHandlers["onClientResourceStart"] += new Action<string>(OnClientResourceStart);
            Util.Chat.Script = this;
        }

        private void DisplayMsg(string text)
        {
            Debug.WriteLine("Receiving client event requesting to send " + text);
            TriggerEvent("chat:addMessage", new
            {
                color = new[] { 100, 25, 250 },
                args = new[] { "[LDev]", text }
            });
        }

        private void DisplayWelcome()
        {
            LScreen.DisplayHelp("HEY EVERY!", true, 1000);
        }

        private async void OnClientResourceStart(string name)
        {
            if (name != GetCurrentResourceName()) return;

            AddTextEntry("BX_NOVES", "You do not have a vessel. To continue, you must create a vessel, or use the default one. Which option you do wants to select?");

            Debug.WriteLine("BladeX™ Software for Microsoft Windows® Clients");
            Debug.WriteLine("Powered by CitizenFX.re");

            Debug.WriteLine("Registering events");
            EventHandlers["bladex:displayWelcomeMessage"] += new Action(DisplayWelcome);
            EventHandlers["bladex:sendMessage"] += new Action<string>(DisplayMsg);
            EventHandlers["bladex:broadcast"] += new Action<string>(DisplayBroadcast);
            EventHandlers["bladex:permissionChange"] += new Action<bool>(PermissionChange);
            EventHandlers["bladex:vesselResponse"] += new Action<bool>(VesselResponse);
            EventHandlers["bladex:creationGoAhead"] += new Action(GoAheadCreation);

            Debug.WriteLine("Registering commands");
            CommandRegistry.RegisterCommands();

#if SERVER_AUTHORITY_SYNC
            Debug.WriteLine("Disabling client authority ambience");
            SetPedPopulationBudget(0);
            SetVehiclePopulationBudget(0);
#endif
            Debug.WriteLine("Starting to load");
            SwitchOutPlayer(Game.Player.Character.Handle, 0, 1);

            StatSetInt((uint)GetHashKey("BANK_BALANCE"), 500, true);

            _menu = new InteractionMenu();

            do
            {
                await Delay(100);
            }
            while (Game.IsLoading);

            this.Tick += EntryScript_Tick;

            TriggerServerEvent("bladex:clientInquireVessel", Game.Player.ServerId.ToString());

            BeginTextCommandBusyspinnerOn("MP_SPINLOADING");
            EndTextCommandBusyspinnerOn((int)LoadingSpinnerType.RegularClockwise);
        }

        private async void GoAheadCreation()
        {
            var model = new Model(PedHash.FreemodeFemale01);
            model.Request();

            while (!model.IsLoaded) { }

            await Game.Player.ChangeModel(model);
            _creation = true;

            model.MarkAsNoLongerNeeded();

            SwitchInPlayer(Game.PlayerPed.Handle);

            ClearInteriorForEntity(Game.PlayerPed.Handle);
            BusyspinnerOff();
            LScreen.DisplayHelp("You are about to create a character.", true, 3000);
            Screen.Hud.IsVisible = false;
            await Delay(1000);

            SetEntityVisible(Game.PlayerPed.Handle, true, false);
            SetPedRandomComponentVariation(Game.PlayerPed.Handle, false);
        }

        private void VesselResponse(bool obj)
        {
            if (!obj)
            {
                _alertDescription = "BX_NOVES";
                _alertTitle = "GLOBAL_ALERT_DEFAULT";
                _drawAlert = true;

                BusyspinnerOff();

                buttons = new InstructionalButtons(new InstructionalButton("Random vessel", Control.FrontendAccept)
                    , new InstructionalButton("Create new", Control.FrontendCancel));
                buttons.Update();
            }
        }

        private void DisplayBroadcast(string obj)
        {
            Screen.ShowNotification(obj);
        }

        private Task EntryScript_Tick()
        {
            //if (_creation)
            //{
            //    Game.PlayerPed.IsVisible = true;
            //    return Task.FromResult(0);
            //}

            if (_drawAlert)
            {
                buttons?.Process();

                DisplayHelpTextThisFrame("BX_NOVES", false);

                if (Game.IsControlJustPressed(0, Control.FrontendAccept))
                {
                    _drawAlert = false;
                    SwitchInPlayer(Game.PlayerPed.Handle);
                    LScreen.DisplayHelp("You are playing as a random vessel. The appearance will not be saved and clothing & accessories are unavailable.", true, 5000);
                    buttons = null;
                }

                if (Game.IsControlJustPressed(0, Control.FrontendCancel))
                {
                    _drawAlert = false;
                    Game.PlayerPed.Position = new Vector3(2535.243f, -383.799f, 92.993f);
                    Game.PlayerPed.IsInvincible = true;
                    Game.PlayerPed.IsPositionFrozen = true;
                    BeginTextCommandBusyspinnerOn("MP_SPINLOADING");
                    EndTextCommandBusyspinnerOn((int)LoadingSpinnerType.RegularClockwise);
                    TriggerServerEvent("bladex:characterCreation", Game.Player.ServerId.ToString());

                    buttons = null;
                }

                return Task.FromResult(0);
            }

            _menu.Update();
            if (_miCooldown > 0) _miCooldown--;
            if (_miTimeout > 0)
            {
                _miTimeout--;
            }
            else if (_mi)
            {
                _mi = false;
            }

            if (Game.IsControlPressed(0, Control.InteractionMenu))
            {
                _menu.OpenOrClose();
            }

            if (Game.IsControlPressed(0, Control.MultiplayerInfo) && _miCooldown == 0 && !IsPlayerSwitchInProgress())
            {
                _mi = !_mi;
                _miCooldown = 75;
                _miTimeout = 150;
            }

            if (_mi && !_prevMi)
            {
                N_0x170f541e1cadd1de(true);
                SetMultiplayerWalletCash();
                SetMultiplayerBankCash();
                N_0x170f541e1cadd1de(false);
                SetBigmapActive(true, false);
                _prevMi = true;
            }

            if (!_mi && _prevMi)
            {
                RemoveMultiplayerWalletCash();
                RemoveMultiplayerBankCash();
                SetBigmapActive(false, false);
                _prevMi = false;
            }

#if SERVER_AUTHORITY_SYNC
            if (_uploadInterval > 0)
            {
                _uploadInterval--;
            }
            else
            {
                _uploadInterval = _uploadTotal;
                TriggerServerEvent("bladex:gamedataUpdate", Game.Player.Handle, Game.PlayerPed.Health, Game.PlayerPed.Armor);
            }
#endif
            return Task.FromResult(0);
        }

        private void PermissionChange(bool obj)
        {
            IsOp = obj;
        }
    }
}
