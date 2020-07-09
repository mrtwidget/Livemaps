using System;
using Steamworks;

namespace NEXIS.Livemap
{
    public class Nodes
    {
        public CSteamID SteamID { get; set; }
        public string CharacterName { get; set; }
        public string Avatar { get; set; }
        public string Position { get; set; }
        public uint Experience { get; set; }
        public int Reputation { get; set; }
        public bool Dead { get; set; }
        public byte Health { get; set; }
        public byte Hunger { get; set; }
        public byte Infection { get; set; }
        public byte Stamina { get; set; }
        public byte Thirst { get; set; }
        public bool Bleeding { get; set; }
        public bool Broken { get; set; }
        public bool GodMode { get; set; }
        public bool VanishMode { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsInVehicle { get; set; }
        public ushort VehicleId { get; set; }
        public bool IsDriver { get; set; }
        public float Ping { get; set; }
        public string IP { get; set; }
        public string Face { get; set; }
        public bool Gold { get; set; }
        public bool Hidden { get; set; }
        public string LastDeadPosition { get; set; }
        public DateTime ConnectionTime { get; set; }
        public DateTime DisconnectTime { get; set; }
        public int SkillAgriculture { get; set; }
        public int SkillCardio { get; set; }
        public int SkillCooking { get; set; }
        public int SkillCrafting { get; set; }
        public int SkillDexerity { get; set; }
        public int SkillDiving { get; set; }
        public int SkillEngineer { get; set; }
        public int SkillExercise { get; set; }
        public int SkillFishing { get; set; }
        public int SkillHealing { get; set; }
        public int SkillImmunity { get; set; }
        public int SkillMechanic { get; set; }
        public int SkillOutdoors  { get; set; }
        public int SkillOverkill { get; set; }
        public int SkillParkour { get; set; }
        public int SkillSharpshooter { get; set; }
        public int SkillSneakybeaky { get; set; }
        public int SkillStrength { get; set; }
        public int SkillSurvival { get; set; }
        public int SkillToughness { get; set; }
        public int SkillVitality { get; set; }
        public int SkillWarmblooded { get; set; }
    }
}
