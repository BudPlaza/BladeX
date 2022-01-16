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
using BudPlaza.BladeX.Client.Ui;

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "RCS1090:Add call to 'ConfigureAwait' (or vice versa).", Justification = "<Pending>")]

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
        private bool _drawAlert;
        private static bool _creation;

        private static InstructionalButtons buttons;
        private static CharactorCreatorMenu menu;

        private readonly List<Ped> _maintainedPed = new List<Ped>();
        private bool runing = false;

        internal static bool IsOp { get; private set; }

        public EntryScript()
        {
            EventHandlers["onClientResourceStart"] += new Action<string>(OnClientResourceStart);
            Util.Chat.Script = this;
        }

        public async Task Replacer()
        {
            if (runing) return;

            runing = true;
            foreach (var ped in World.GetAllPeds())
            {
                await Delay(200);

                if (ped?.Exists() != true || ped.IsInVehicle())
                {
                    continue;
                }

                if (_maintainedPed.Count >= 15 && !ped.IsOnScreen && ped.Position.DistanceToSquared2D(Game.PlayerPed.Position) >= 150f)
                {
                    ped.Delete();
                    continue;
                }

                if (!_maintainedPed.Contains(ped) && !ped.IsPlayer && ped.Model != PedHash.FreemodeFemale01 && !ped.IsPersistent && ped.IsAlive && !ped.IsInVehicle())
                {
                    var pos = ped.Position;
                    var head = ped.Heading;

                    var newPed = await World.CreatePed(PedHash.FreemodeFemale01, pos, head);
                    if (newPed?.Exists() == true)
                    {
                        newPed.SetPedNewUnf();
                        var rdm = new Random();
                        SetPedHeadBlendData(newPed.Handle, rdm.Next(48), rdm.Next(48), rdm.Next(48), 0, 0, 0, 0.5f, 0f, 0f, false);
                        _maintainedPed.Add(newPed);
                        newPed.Task.WanderAround();
                        newPed.Weapons.Give(WeaponHash.Pistol, 120, true, true);
                    }

                    ped.Delete();
                }
            }

            for (var i = 0; i < _maintainedPed.Count; i++)
            {
                await Delay(200);

                Ped ped;
                try
                {
                    ped = _maintainedPed[i];
                }
                catch (Exception ex)
                {
                    continue;
                }

                if (ped?.Exists() != true || (!ped.IsOnScreen && ped.Position.DistanceToSquared2D(Game.PlayerPed.Position) >= 150f) || ped.Position.DistanceToSquared2D(Game.PlayerPed.Position) >= 550f)
                {
                    _maintainedPed.RemoveAt(i);
                    if (ped?.Exists() == true)
                    {
                        ped.Delete();
                    }
                }
            }
            runing = false;
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

            TriggerServerEvent("bladex:clientInquireVessel", Game.Player.ServerId.ToString());
            Debug.WriteLine("Registering commands");
            CommandRegistry.RegisterCommands();

            SetPedPopulationBudget(2);
            SetVehiclePopulationBudget(1);

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

            BeginTextCommandBusyspinnerOn("MP_SPINLOADING");
            EndTextCommandBusyspinnerOn((int)LoadingSpinnerType.RegularClockwise);
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
            if (Game.PlayerPed.Health < Game.PlayerPed.MaxHealth)
            {
                Game.PlayerPed.Health++;
            }

            if (_creation)
            {
                if (menu != null)
                {
                    return menu.Tick();
                }

                return Task.FromResult(0);
            }

            if (_drawAlert)
            {
                Game.Player.ChangeModel(PedHash.FreemodeFemale01);

                var rdm = new Random();
                SetPedHeadBlendData(Game.PlayerPed.Handle, rdm.Next(48), rdm.Next(48), rdm.Next(48), 0, 0, 0, 0.5f, 0f, 0f, false);
                Game.PlayerPed.SetPedNewUnf();

                _drawAlert = false;
                SwitchInPlayer(Game.PlayerPed.Handle);
                LScreen.DisplayHelp("You are playing as a random vessel. To save this character, use Interaction Menu -> Appearance -> Save Vessel.", true, 5000);

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

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Replacer();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            return Task.FromResult(0);
        }

        private void PermissionChange(bool obj)
        {
            IsOp = obj;
        }
    }
}
