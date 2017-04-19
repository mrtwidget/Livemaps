# Livemaps

This plugin was developed using [Rocket Mod](https://rocketmod.net/) libraries for the [Steam](http://store.steampowered.com/) game [Unturned](http://store.steampowered.com/app/304930/). The Rocket Mod plugin collects server, player, and chat data recurrently and then saves the data to the user-configured MySQL database. The API provides an AJAX interface for retrieving the latest JSON-encoded server data. Livemap API responses are used to update the WebUI maps.

*The WebUI portion of this plugin is optional*, and is intended only as a default theme for this project. The API may be used independently to support fully custom livemap themes and features.

**How to Install:**

***Plugin***
1. Compile project
2. Copy `Livemap.dll` to the Rocket Mod plugin directory
3. Start/stop server to generate `Livemap.configuration.xml` and configure MySQL database settings
4. Add Rocket Mod permission for /livemap command to `Permissions.config.xml`
    - *Example*: `<Permission Cooldown="0">livemap</Permission>`
5. Start Server

***WebUI***
1. Copy folder contents of `www` to your web server.

***API***
1. Edit `www/api/config.api.php` and configure MySQL database settings
    - *For standalone API usage, copy only the the `www/api` folder to your web server*
2. Send GET request to `www/api/livemap.api.php?livemap=server_id` for JSON-encoded response
    
    **Filtering**
    - By default, a null GET `filter` returns all livemap table results.

    - Table filtering is accomplished by specifying a table name `filter` within the GET request.
        - *Example*
            - *URI*: `?livemap=server_id&filter=livemap_data`
            - *Result*: Only `livemap_data` table results are returned

    - Player filtering is accomplished by specifying a player "Steam64ID" (*i.e.* `7656#############`) `filter` within the GET request.
        - *Example*
            - *URI*: `?livemap=server_id&filter=7656#############`
            - *Result*: Only specified player data is returned

---

**API Returnable Fields:**
- Server ID
- Server Name
- App Version 
- Map Name
- Online Player Count
- Max Player Count
- PvP
- Gold / Pro
- Has Cheats
- Hide Admins
- Cycle Time
- Cycle Length
- Full Moon
- Up-time
- Packets Received
- Packets Sent
- Port
- Mode

- Player Position
- Player Rotation
- Player Stats
    - Health
    - Stamina
    - Hunger
    - Thirst
    - Infection
    - Bleeding
    - Broken Bones
    - Experience
    - Reputation
    - All Skill Levels
- Steam Data 
    - Avatar
    - Group ID
- Player IP address
- Player status/features
    - Admin
    - Gold
    - God Mode
    - Vanish Mode
    - Dead
    - Dead Body Location
    - In Vehicle
        - Is Driver
        - Instance ID
        - Vehicle ID
        - Fuel
        - Health
        - Headlights On/Off
        - Taillights On/Off
        - Sirens On/Off
        - Speed
        - Has Battery
        - Battery Charge
        - Exploded
        - Locked
- Player Appearance
    - Skin Color
    - Hair Type
    - Face Type
    - Beard Type
    - Hat
    - Glasses
    - Mask

- Livemap Hidden Status

---

**Requirements:**
- PHP 7.0.+
- MySQL 5.6.+

---

**Resources:**
- Trello my development process: [https://trello.com/b/4GiQoxyK](https://trello.com/b/4GiQoxyK)
- Nexis Realms Unturned Server: [nexisrealms.com](http://www.nexisrealms.com)

---

*creator: Nexis (steam:iamtwidget) <[mrtwidget@gmail.com](mailto:mrtwidget@gmail.com)>*