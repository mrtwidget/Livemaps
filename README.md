# Livemaps

This plugin was developed using [Rocket Mod](https://rocketmod.net/) libraries for the [Steam](http://store.steampowered.com/) game [Unturned](http://store.steampowered.com/app/304930/). The Rocket Mod plugin collects server, player, and chat data recurrently and then saves the data to the user-configured MySQL database. The API provides an AJAX interface for retrieving the latest JSON-encoded server data. Livemap API responses are used to update the WebUI maps.

*The WebUI portion of this plugin is optional*, and is intended only as a default theme for this project. The API may be used independently to support fully custom livemap themes and features.

**How to Install:**

***Plugin***
1. Compile project
2. Copy `Livemap.dll` to Unturned rocket plugin directory
3. Start/stop server to generate config and configure MySQL database
4. Add Rocket permission for `/livemap` command to `Permissions.config.xml`
    - *Example*: `<Permission Cooldown="0">livemap</Permission>`
5. Restart Server

***WebUI***
1. Copy folder contents from `www` to web server.

***API***
1. Edit `www/api/config.api.php` and configure MySQL database connection settings
2. Send GET request to `www/api/livemap.api.php?livemap=server_id` for JSON-encoded response
    
    **Filtering Results**
    1. Table filtering is accomplished by specifying a table name `filter` within the GET request. By default, a null GET `filter` returns all livemap table results.
        - *Example*
            - *URI*: `?livemap=server_id&filter=livemap_data`
            - *Result*: Only `livemap_data` table results are returned

---

**Features:**
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
- Trello: [https://trello.com/b/4GiQoxyK](https://trello.com/b/4GiQoxyK)
- Nexis Realms [nexisrealms.com](http://www.nexisrealms.com)

---
