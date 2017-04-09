using System;
using Rocket.API;
using Rocket.Unturned.Player;
using Rocket.Unturned.Skills;
using SDG.Unturned;
using UnityEngine;
using System.Xml;
using Rocket.Unturned;
using Steamworks;

namespace NEXIS.Livemap
{
    public class PlayerData
    {
        CSteamID _CSteamID;
        string _CharacterName;
        string _DisplayName;
        CSteamID _SteamGroupID; Uri _SteamAvatarMedium; string _SteamProfileHeadline;
        string _IP; float _Ping;
        bool _IsPro; bool _IsAdmin; bool _GodMode; bool _VanishMode;
        bool _IsInVehicle;
            bool _enabled;
            bool _isDriver;
            uint _instanceID;
            ushort _vehicleID;
            ushort _vehicleFuel;
            ushort _vehicleHealth;
            bool _headlightsOn;
            bool _taillightsOn;
            bool _sirensOn;
            float _speed;
            bool _hasBattery;
            ushort _batteryCharge;
            bool _isExploded;
            bool _isLocked;
            CSteamID _lockedOwner;
        Vector3 _Position; float _Rotation;
        bool _Dead;
        Color _skin;
        byte _hair; byte _face; byte _beard;
        int _visualHat; int _visualGlasses; int _visualMask;
        bool _Bleeding; bool _Broken;
        byte _Health; byte _Stamina; byte _Hunger; byte _Thirst; byte _Infection;
        uint _Experience; int _Reputation;
        byte _Agriculture;
        byte _Cardio;
        byte _Cooking;
        byte _Crafting;
        byte _Dexerity;
        byte _Diving;
        byte _Engineer;
        byte _Exercise;
        byte _Fishing;
        byte _Healing;
        byte _Immunity;
        byte _Mechanic;
        byte _Outdoors;
        byte _Overkill;
        byte _Parkour;
        byte _Sharpshooter;
        byte _Sneakybeaky;
        byte _Strength;
        byte _Survival;
        byte _Toughness;
        byte _Vitality;
        byte _Warmblooded;
        bool _IsHiddenLivemap;

        public PlayerData(CSteamID CSteamID, string CharacterName, string DisplayName, CSteamID SteamGroupID, Uri SteamAvatarMedium,
            string SteamProfileHeadline, string IP, float Ping, bool IsPro, bool IsAdmin, bool GodMode, bool VanishMode, bool IsInVehicle,
            bool enabled, bool isDriver, uint instanceID, ushort vehicleID, ushort vehicleFuel, ushort vehicleHealth, bool headlightsOn, 
            bool taillightsOn, bool sirensOn, float speed, bool hasBattery, ushort batteryCharge, bool isExploded, bool isLocked, 
            CSteamID lockedOwner, Vector3 Position, float Rotation, bool Dead, Color skin, byte hair, byte face, byte beard, int visualHat,
            int visualGlasses, int visualMask, bool Bleeding, bool Broken, byte Health, byte Stamina, byte Hunger, byte Thirst, 
            byte Infection, uint Experience, int Reputation, byte Agriculture, byte Cardio, byte Cooking, byte Crafting, byte Dexerity, 
            byte Diving, byte Engineer, byte Exercise, byte Fishing, byte Healing, byte Immunity, byte Mechanic, byte Outdoors,
            byte Overkill, byte Parkour, byte Sharpshooter, byte Sneakybeaky, byte Strength, byte Survival, byte Toughness, byte Vitality, 
            byte Warmblooded, bool IsHiddenLivemap)
        {
            this._CSteamID = CSteamID;
            this._CharacterName = CharacterName;
            this._DisplayName = DisplayName;
            this._SteamGroupID = SteamGroupID;
            this._SteamAvatarMedium = SteamAvatarMedium;
            this._SteamProfileHeadline = SteamProfileHeadline;
            this._IP = IP;
            this._Ping = Ping;
            this._IsPro = IsPro;
            this._IsAdmin = IsAdmin;
            this._GodMode = GodMode;
            this._VanishMode = VanishMode;
            this._IsInVehicle = IsInVehicle;
            this._enabled = enabled;
            this._isDriver = isDriver;
            this._instanceID = instanceID;
            this._vehicleID = vehicleID;
            this._vehicleFuel = vehicleFuel;
            this._vehicleHealth = vehicleHealth;
            this._headlightsOn = headlightsOn;
            this._taillightsOn = taillightsOn;
            this._sirensOn = sirensOn;
            this._speed = speed;
            this._hasBattery = hasBattery;
            this._batteryCharge = batteryCharge;
            this._isExploded = isExploded;
            this._isLocked = isLocked;
            this._lockedOwner = lockedOwner;
            this._Position = Position;
            this._Rotation = Rotation;
            this._Dead = Dead;
            this._skin = skin;
            this._hair = hair;
            this._face = face;
            this._beard = beard;
            this._visualHat = visualHat;
            this._visualGlasses = visualGlasses;
            this._visualMask = visualMask;
            this._Bleeding = Bleeding;
            this._Broken = Broken;
            this._Health = Health;
            this._Stamina = Stamina;
            this._Hunger = Hunger;
            this._Thirst = Thirst;
            this._Infection = Infection;
            this._Experience = Experience;
            this._Reputation = Reputation;
            this._Agriculture = Agriculture;
            this._Cardio = Cardio;
            this._Cooking = Cooking;
            this._Crafting = Crafting;
            this._Dexerity = Dexerity;
            this._Diving = Diving;
            this._Engineer = Engineer;
            this._Exercise = Exercise;
            this._Fishing = Fishing;
            this._Healing = Healing;
            this._Immunity = Immunity;
            this._Mechanic = Mechanic;
            this._Outdoors = Outdoors;
            this._Overkill = Overkill;
            this._Parkour = Parkour;
            this._Sharpshooter = Sharpshooter;
            this._Sneakybeaky = Sneakybeaky;
            this._Strength = Strength;
            this._Survival = Survival;
            this._Toughness = Toughness;
            this._Vitality = Vitality;
            this._Warmblooded = Warmblooded;
            this._IsHiddenLivemap = IsHiddenLivemap;
        }

        public CSteamID CSteamID { get { return _CSteamID; } }
        public string CharacterName { get { return _CharacterName; } }
        public string DisplayName { get { return _DisplayName; } }
        public CSteamID SteamGroupID { get { return _SteamGroupID; } }
        public Uri SteamAvatarMedium { get { return _SteamAvatarMedium; } }
        public string SteamProfileHeadline { get { return _SteamProfileHeadline; } }
        public string IP { get { return _IP; } }
        public float Ping { get { return _Ping; } }
        public bool IsPro { get { return _IsPro; } }
        public bool IsAdmin { get { return _IsAdmin; } }
        public bool GodMode { get { return _GodMode; } }
        public bool VanishMode { get { return _VanishMode; } }
        public bool IsInVehicle { get { return _IsInVehicle; } }
            public bool enabled { get { return _enabled; } }
            public bool isDriver { get { return _isDriver; } }
            public uint instanceID { get { return _instanceID; } }
            public ushort vehicleID { get { return _vehicleID; } }
            public ushort vehicleFuel { get { return _vehicleFuel; } }
            public ushort vehicleHealth { get { return _vehicleHealth; } }
            public bool headlightsOn { get { return _headlightsOn; } }
            public bool taillightsOn { get { return _taillightsOn; } }
            public bool sirensOn { get { return _sirensOn; } }
            public float speed { get { return _speed; } }
            public bool hasBattery { get { return _hasBattery; } }
            public ushort batteryCharge { get { return _batteryCharge; } }
            public bool isExploded { get { return _isExploded; } }
            public bool isLocked { get { return _isLocked; } }
            public CSteamID lockedOwner { get { return _lockedOwner; } }
        public Vector3 Position { get { return _Position; } }
        public float Rotation { get { return _Rotation; } }
        public bool Dead { get { return _Dead; } }
        public Color skin { get { return _skin; } }
        public byte hair { get { return _hair; } }
        public byte face { get { return _face; } }
        public byte beard { get { return _beard; } }
        public int visualHat { get { return _visualHat; } }
        public int visualGlasses { get { return _visualGlasses; } }
        public int visualMask { get { return _visualMask; } }
        public bool Bleeding { get { return _Bleeding; } }
        public bool Broken { get { return _Broken; } }
        public byte Health { get { return _Health; } }
        public byte Stamina { get { return _Stamina; } }
        public byte Hunger { get { return _Hunger; } }
        public byte Thirst { get { return _Thirst; } }
        public byte Infection { get { return _Infection; } }
        public uint Experience { get { return _Experience; } }
        public int Reputation { get { return _Reputation; } }
        public byte Agriculture { get { return _Agriculture; } }
        public byte Cardio { get { return _Cardio; } }
        public byte Cooking { get { return _Cooking; } }
        public byte Crafting { get { return _Crafting; } }
        public byte Dexerity { get { return _Dexerity; } }
        public byte Diving { get { return _Diving; } }
        public byte Engineer { get { return _Engineer; } }
        public byte Exercise { get { return _Exercise; } }
        public byte Fishing { get { return _Fishing; } }
        public byte Healing { get { return _Healing; } }
        public byte Immunity { get { return _Immunity; } }
        public byte Mechanic { get { return _Mechanic; } }
        public byte Outdoors { get { return _Outdoors; } }
        public byte Overkill { get { return _Overkill; } }
        public byte Parkour { get { return _Parkour; } }
        public byte Sharpshooter { get { return _Sharpshooter; } }
        public byte Sneakybeaky { get { return _Sneakybeaky; } }
        public byte Strength { get { return _Strength; } }
        public byte Survival { get { return _Survival; } }
        public byte Toughness { get { return _Toughness; } }
        public byte Vitality { get { return _Vitality; } }
        public byte Warmblooded { get { return _Warmblooded; } }
        public bool IsHiddenLivemap { get { return _IsHiddenLivemap; } }
    }
}
