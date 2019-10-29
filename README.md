# Livemaps

This plugin was developed using [Rocket Mod](https://rocketmod.net/) libraries for the [Steam](http://store.steampowered.com/) game [Unturned](http://store.steampowered.com/app/304930/). This Rocket Mod plugin collects server, player, and chat data recurrently and then saves the retrieved data to a user-configured MySQL database or uploads valid JSON to a web address of your choosing. The API provides an AJAX interface for retrieving the latest JSON-encoded server data. Livemap API responses are then used to update the WebUI maps.

*The WebUI portion of this plugin is optional*, and is intended only as a default theme for this project. The API may be used independently to support fully custom livemap themes and features.

Current Release:
----------------
![alt text](http://nexisrealms.com/livemap/livemapexample.gif "Livemaps v0.3")

- **Livemap v0.3**
    - Livemap Demo: [nexisrealms.com/livemap](http://nexisrealms.com/livemap)

How to Install:
---------------
There are three parts to this plugin: 
1. **[Rocket Mod plugin](https://rocketmod.net/plugins)** *(livemap.dll)*
2. **WebUI** *- the website files*
3. **API** *- how the WebUI gets updates from the server*

***Livemap Rocket Plugin***
1. Compile this project
2. Copy the compiled `Livemap.dll` to your Rocket Mod plugin directory
3. Start/stop your server to generate `Livemap.configuration.xml`
4. Edit `Livemap.configuration.xml` and configure MySQL database settings (if desired)
5. Add a Rocket Mod permission for the /hide command by adding it to your `Permissions.config.xml`
    - *Example*: `<Permission Cooldown="0">hide</Permission>`
6. Start Unturned Server

***WebUI***
1. Copy the complete contents of the `www` folder to your web server.

** INFORMATION BELOW HAS CHANGED. API SUPPORT WILL BE ADDED IN UPCOMING UPDATE **

***API***
1. Edit `www/api/config.api.php` and configure your MySQL database settings
    - *Note: For standalone API usage, copy only the `www/api` folder to your web server*
2. Send a GET request to the API to retrieve a JSON-encoded response:

    **Sending Requests**
    - Here is a JavaScript example of how to send an API request using AJAX:
    ```javascript
        $.ajax({
            dataType: "json",
            type: "GET",
            url: "api/livemap.api.php",
            data: {
                livemap: server_id,
                filter: null
            },
            success: function(data) {
                console.log(data);
            },
            error: function(e) {
                console.log(e);
            }
        });
    ```
    
    - You may also view results in a browser:
        - *Example*
            - Navigate to `http://www.example.com/api/livemap.api.php?livemap=server_id`; replacing `server_id` with your own server instance name
            - *Result*: The results of the request will be displayed directly on the page in JSON format

    **Filtering**
    - By default, a `filter` parameter is not required to successfully process an API GET request. When no `filter` parameter is passed all of the tables are processed and returned in the request. The tables include `livemap_server`, `livemap_data` and `livemap_chat`.

    - Table filtering is accomplished by passing a specific MySQL table name to the `filter` parameter within an API GET request:
        - *Example*
            - *URI*: `http://www.example.com/api/livemap.api.php?livemap=server_id&filter=livemap_data`
            - *Result*: Only `livemap_data` table results are returned

    - Player filtering is accomplished by passing a specific player "Steam64ID" (*i.e.* `7656#############`) to the `filter` parameter within an API GET request:
        - *Example*
            - *URI*: `http://www.example.com/api/livemap.api.php?livemap=server_id&filter=7656#############`
            - *Result*: Only specified player data is returned

    3. Append the returned data to your website

---

API Returnable Fields:
----------------------
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

**Price:**
- *Free*, and it always will be.

---

**Requirements:**
- PHP 5.6.+
- MySQL 5.6.5+
    - *It is **required** to have a minimum MySQL version of **5.6.5** to use this plugin (if mysql is enabled). This is to support the use of tables with multiple timestamp fields.*

---

**Resources:**
- Livemap Demo: [nexisrealms.com](http://nexisrealms.com/livemap)

---

*author: Nexis (steam:iamtwidget) <[nexis@nexisrealms.com](mailto:nexis@nexisrealms.com)>*