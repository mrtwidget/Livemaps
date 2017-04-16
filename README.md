# Livemaps

**How to Install:**

***Plugin***
1. Compile project
2. Copy `Livemap.dll` to Unturned rocket plugin directory
3. Start/stop server to generate config and configure MySQL database
4. Add Rocket permission for `/livemap` command to `Permissions.config.xml`
    - *Example*: `<Permission Cooldown="0">livemap</Permission>`
5. Restart Server

***API***
1. Edit `www/api/config.api.php` and configure MySQL database connection settings
2. Send GET request to `www/api/livemap.api.php?livemap=server_id` for JSON-encoded response
    
**Filtering Results**
Filtering is accomplished by specifying a table name with `filter`. A null `filter` returns all livemap table results.
    - *Example*
        - *URI*: `?livemap=server_id&filter=livemap_data`
        - *Result*: Only `livemap_data` table results are returned

***Web UI***

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
