using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using TownOfUs.NeutralRoles.VultureMod;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Vulture : Role
    {
        private KillButton _eatButton;
        public PlayerControl ClosestPlayer;
        public DateTime LastEaten { get; set; }
        public bool EatenBodies = false;
        public int BodiesEaten = 0;
        public Dictionary<byte, ArrowBehaviour> BodyArrows = new Dictionary<byte, ArrowBehaviour>();

        public Vulture(PlayerControl player) : base(player)
        {
            Name = "Vulture";
            ImpostorText = () => "Devour Corpses";
            TaskText = () => "Eat up dead bodies to win";
            Color = Patches.Colors.Vulture;
            LastEaten = DateTime.UtcNow;
            RoleType = RoleEnum.Vulture;
            AddToRoleHistory(RoleType);
            Faction = Faction.NeutralEvil;
        }

        public KillButton EatButton
        {
            get => _eatButton;
            set
            {
                _eatButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public DeadBody CurrentTarget { get; set; }

        public float EatTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastEaten;
            var num = CustomGameOptions.EatCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__38 __instance)
        {
            var vultTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            vultTeam.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = vultTeam;
        }

        public void DestroyArrow(byte targetPlayerId)
        {
            var arrow = BodyArrows.FirstOrDefault(x => x.Key == targetPlayerId);
            if (arrow.Value != null)
                UnityEngine.Object.Destroy(arrow.Value);
            if (arrow.Value.gameObject != null)
                UnityEngine.Object.Destroy(arrow.Value.gameObject);
            BodyArrows.Remove(arrow.Key);
        }

        internal override bool GameEnd(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead) return true;
            if (!CustomGameOptions.NeutralEvilWinEndsGame) return true;
            if (!EatenBodies) return true;
            Utils.EndGame();
            return false;
        }
    }
}