/**
 * Livemaps v0.2
 * author: Nexis <nexis@nexisrealms.com>
 */

/**
 * Disable Specific Livemaps
 * Use the following array to disable specific livemaps that you do not want to
 * display on the page by entering their server instance names below.
 */

/* Disabled Livemaps */
var DisabledLivemaps = []; /* var DisabledLivemaps = ['server1', 'pei']; */
/* Default Node Skin Color */
var DefaultSkinColor = '#CCCC91';
/* Max Refreshes */
var MaxRefresh = 60;

/* ================ [ DO NOT EDIT BELOW THIS LINE ] ================ */

/* Active Instances */
var ActiveInstances = [];
/* Intervals */
var Intervals = [];
/* Refresh Interval */
var RefreshInterval = [];
/* Connected Players [CSteam64ID] */
var ConnectedPlayers = [];
/* Hidden Players */
var HiddenPlayers = [];
/* Refresh Interval */
var LastWorldChatID = [];
/* Last Livemap Data */
var LastLivemapData = [];
/* Heartbeats */
var Heartbeats = [];
/* Refresh Counter */
var RefreshCounter = 10;
/* Total Refresh Counter */
var TotalRefreshes = 0;
/* First Page Load */
var FirstLoad = true;

/**
 * INITIATE LIVEMAP INTERFACE
 * 
 * This function queries the API for active server data. If any active servers
 * are found, all relevant data is then queried and loaded to the page
 */
function init(server_id = null) {
    $.ajax({
        dataType: "json",
        type: "GET",
        url: "api/livemap.api.php",
        data: {
            livemap: server_id,
            filter: "livemap_server"
        },
        success: function(data) {
            LoadLivemaps(data.livemap_server);
        },
        error: function(e) {
            console.log(e);
        }
    });
}

/**
 * RETURN SERVER LIST (default page)
 * 
 * This function returns a list of servers that are actively running and lists 
 * a link to each server Livemap by default.
 */
function returnServerList() {
    $.ajax({
        dataType: "json",
        type: "GET",
        url: "api/livemap.api.php",
        data: {
            filter: "livemap_server"
        },
        success: function(data) {
            // create servers list
            jQuery.each(data.livemap_server, function(index, server) {

                var data = "<div class=\"livemap-status\">" +
                    "<div class=\"server-name\">"+ server.server_name +"</div>" +
                    "<div class=\"server-map\">"+ server.map +"</div>" +
                    "<div class=\"server-version\">"+ server.app_version +"</div>" + 
                    //"<div class=\"server-port\">"+ server.port +"</div>" + 
                    "<div class=\"server-players\">"+ server.online_players +"</div>" + 
                    "<div class=\"server-max-players\">"+ server.max_players +"</div>" + 
                    "<a href=\"?id="+ server.server_id.toLowerCase() +"\" title=\"View Livemap\">" +
                    "<img class=\"status-overlay\" src=\"images/status/livemap_status_online.png\">" + 
                    "<img src=\"images/maps/"+ server.map.toLowerCase() +"/Icon.png\">" +
                    "</a>" +
                    "</div>";                
                
                $(".livemaps").append(data);
            });
        },
        error: function(e) {
            console.log(e);
        }
    });
}

/**
 * HEARTBEAT
 * This function serves as the heartbeat for the livemap refresh sequence.
 */
function Heartbeat() {
    if (RefreshCounter > 10) {
        $("img.heartbeat").attr("src", "images/icons/heart/1.png");
        RefreshCounter = 1;
    } else {
        $("img.heartbeat").attr("src", "images/icons/heart/" + RefreshCounter + ".png");
        $("img.heartbeat").velocity("callout.pulse", { queue:false, duration: 150 });
        
        RefreshCounter++;
    }
}

/**
 * LOAD ALL ACTIVELY RUNNING SERVER INSTANCES
 * 
 * This function queries the API for servers that have been updated in the
 * past 30 seconds and returns a populated element that is added to the 
 * .livemaps element found in index.html
 * @param {obj} livemap_server Returned JSON server data
 */
function LoadLivemaps(livemap_server) {
    // loop through each server instance returned
    jQuery.each(livemap_server, function(i, val) {
        // if instance exists in ActiveInstances, load the livemap to view
        if (DisabledLivemaps.indexOf(val.server_id.toLowerCase()) == -1) {
            // Add this server to the active instance array
            ActiveInstances.push(val.server_id);
            // Set refresh interval
            RefreshInterval[val.server_id] = val.refresh_interval * 1000;

            // generate the map
            $.ajax({
                type: "GET",
                url: "templates/layout.livemap.php",
                data: {
                    // send server data
                    ServerID: val.server_id,
                    Map: val.map,
                    PlayersOnline: returnOnlinePlayerCountString(val)
                },
                success: function(data) {
                    // add generated map to the page
                    $(".livemaps").append(data);
                    // refresh the livemap
                    RefreshLivemap(val.server_id);
                },
                error: function(e) {
                    console.log(e);
                }
            });
        }
    })
}

/**
 * TRIGGER LIVEMAP REFRESH
 */
function TriggerLivemapRefresh() {
    if (TotalRefreshes < MaxRefresh) {
        for (var i = 0; i < ActiveInstances.length; i++) {
            RefreshLivemap(ActiveInstances[i]);
        }
        TotalRefreshes++;
    }
    else
    {
        // timeout
        $(".livemap-static-container").removeClass("hidden");
        clearInterval();
        // hide badges
        $(".livemap-badge-container").velocity("transition.bounceOut", { stagger: 100, drag: true });
        // hide world chat
        $(".livemap[data-server-id='" + livemap_server.server_id + "'] .livemap-chat").velocity("transition.flipBounceYOut", { drag: true });
    }
}

/**
 * REFRESH LIVEMAP
 * 
 * This function sends a request to the API and retrieves all server table data
 * and then updates the livemap data
 * @param {string} server_id Server instance name
 */
function RefreshLivemap(server_id) {
    $.ajax({
        dataType: "json",
        type: "GET",
        url: "api/livemap.api.php",
        data: {
            // send `server_id` request to API, get all data
            livemap: server_id
        },
        success: function(data) {
            LastLivemapData[server_id] = data.livemap_data;

            UpdateConnectedPlayersArray(data);
            UpdateLivemapHUD(data.livemap_server[0]);
            UpdatePlayerBadges(data.livemap_data);
            UpdatePlayerNodes(data.livemap_server[0], data.livemap_data);
            UpdateWorldChat(data.livemap_chat, data.livemap_server[0]);
        },
        error: function(e) {
            console.log(e);
        }
    });
}

/**
 * UPDATE CONNECTED PLAYERS ARRAY
 * 
 * This function iterates through the ConnectedPlayers object (before updating
 * anything else) and proceeds to search for a matching steam id within the 
 * most recently returned API request. Matching results are ignored. If a match
 * is not found, the player node and (name)badge will be removed or added.
 * @param {obj} data Returned API JSON data
 */
function UpdateConnectedPlayersArray(data) {
    var disconnectMatch = false;
    var connectMatch = false;
    var connectedPlayers = ConnectedPlayers;
    
    /* check if player connected */
    jQuery.each(data.livemap_data, function(index, player) {
        
        // check if player exists in ConnectedPlayers array
        for (var key in connectedPlayers) {            
            if (player.character_name == connectedPlayers[key]) {
                connectMatch = true;
            }
        }
        // if no match was found, this player has connected
        if (!connectMatch) {
            PlayerConnected(player,data.livemap_server[0]); // create new player
        }
        connectMatch = false; // reset
    })

    /* check if player disconnected */
    for (var key in connectedPlayers) {
        // check if player exists in latest refresh
        jQuery.each(data.livemap_data, function(index, player) {
            if (key == player.CSteamID) {
                disconnectMatch = true;
            }
        })
        // if no match was found, this player has disconnected
        if (!disconnectMatch) {
            PlayerDisconnected(data.livemap_server[0].server_id,key,connectedPlayers[key]); // remove player
        }
        disconnectMatch = false; // reset
    }
}

/**
 * PLAYER CONNECTED
 * 
 * This function generates new player elements when a player connects to a
 * server. It adds then to the ConnectedPlayers object and announces the new
 * connection in the associated server's world chat.
 * @param {obj} player_data Returned player data
 * @param {obj} livemap_server Returned server data
 */
function PlayerConnected(player_data,livemap_server) {
    // add player to connected player array
    ConnectedPlayers[player_data.CSteamID] = player_data.character_name;
    // add player elements
    AddPlayerBadge(player_data);
    //$(".livemap-badge-container[data-steam-id='" + player_data.CSteamID + "']").velocity("transition.bounceIn", { stagger: 165, drag: true });
    AddPlayerNode(player_data,livemap_server);
    if (!FirstLoad) {
        // notify chat that player connected
        ChatWorld(player_data.server_id, "Server", player_data.character_name + " has connected!", "lime");
    }
}

/**
 * PLAYER DISCONNECTED
 * 
 * This function removes all player elements when a player disconnects from a
 * server. It removes the player from the ConnectedPlayers object and announces
 * the new connection in the associated server's world chat.
 * @param {string} server_id // Server instance name
 * @param {string} steam_id // Disconnecting player CSteamID
 * @param {string} character_name // Disconnecting player name
 */
function PlayerDisconnected(server_id,steam_id,character_name) {
    // remove player elements
    RemovePlayerBadge(steam_id);
    RemovePlayerNode(steam_id);
    
    // remove player to connected player array
    delete ConnectedPlayers[steam_id];

    //var index = ConnectedPlayers.indexOf(steam_id);
    //if (index > -1) { ConnectedPlayers.splice(index, 1); }

    // notify chat that player disconnected
    ChatWorld(server_id, "Server", character_name + " has disconnected!", "red");
}

/**
 * UPDATE LIVEMAP HUD DATA
 * 
 * This function updates the HUD data with the most recent, as well as updating
 * the (name)badge column's player online count.
 * @param {obj} livemap_server Returned JSON server data
 */
function UpdateLivemapHUD(livemap_server) {
    // update badges online player count
    $(".livemap[data-server-id='" + livemap_server.server_id + "'] .livemap-badges-online-players span.player-count").html(returnOnlinePlayerCountString(livemap_server));
    // update hud online player count
    $(".livemap[data-server-id='" + livemap_server.server_id + "'] .livemap-hud-online-players").html(returnOnlinePlayerCountString(livemap_server));
}

/**
 * UPDATE PLAYER NODES
 * 
 * This function iterates through the returned livemap_data (player data) from 
 * the most recent livemap refresh, and then updates each player node position
 * and details (color, icon, grouping, etc.)
 * @param {obj} livemap_server Returned JSON server data
 * @param {obj} livemap_data Returned JSON player data
 */
function UpdatePlayerNodes(livemap_server,livemap_data) {
    jQuery.each(livemap_data, function(index, player) {
        // check vehicle status
        if (player.in_vehicle > 0) {
            $(".livemap[data-server-id='" + livemap_server.server_id + "'] .livemap-node-container[data-steam-id='" + player.CSteamID + "'] img.livemap-node").attr("src", "images/nodes/vehicles/"+ returnVehicleIcon(player) +".png");
            $(".livemap[data-server-id='" + livemap_server.server_id + "'] .livemap-node-container[data-steam-id='" + player.CSteamID + "'] img.livemap-node").css({"background-color":"transparent",border:0});
        } else {
        // update player node face and color
            $(".livemap[data-server-id='" + livemap_server.server_id + "'] .livemap-node-container[data-steam-id='" + player.CSteamID + "'] img.livemap-node").attr("src", "images/nodes/faces/"+ returnNodeFace(player) +".png");
            $(".livemap[data-server-id='" + livemap_server.server_id + "'] .livemap-node-container[data-steam-id='" + player.CSteamID + "'] img.livemap-node").css({"background-color":(player.skin_color == '#000000' ? '#CCCC91' : player.skin_color)});
        }

        // calculate new player position
        var calcPos = CalculateVectorPosition(player.server_id, livemap_server.map, player.position);

        // animate player node to new position
        $(".livemap[data-server-id='" + livemap_server.server_id + "'] .livemap-node-container[data-steam-id='" + player.CSteamID + "']").velocity({
            left: calcPos[0],
            bottom: calcPos[1],
            rotateZ: (player.in_vehicle > 0 ? player.rotation : 0)
        },
        {
            duration: RefreshInterval[player.server_id], 
            easing: "linear"
        });
    })
}

/**
 * ADD PLAYER NODE
 * 
 * This function creates a new player map node via ajax request to the node
 * template, and then appends the Livemap with the new player data.
 * @param {obj} player_data 
 * @param {obj} livemap_server 
 */
function AddPlayerNode(player_data,livemap_server) {
    // for some reason player skin color defaults to black when a player first
    // connects, which makes it hard to see the faces, so let's change it to
    // the default skin color, for now
    if (player_data.skin_color == '#000000') player_data.skin_color = DefaultSkinColor;

    $.ajax({
        type: "GET",
        url: "templates/node.livemap.php",
        data: {
            // send player data
            CSteamID: player_data.CSteamID,
            CharacterName: player_data.character_name,
            Position: returnNodePositionStyle(livemap_server.server_id, livemap_server.map, player_data.position),
            SkinColor: "background-color:" + player_data.skin_color,
            Face: returnNodeFace(player_data)
        },
        success: function(data) {
            // add node to Livemap
            $(".livemap[data-server-id='" + livemap_server.server_id + "'] .livemap-nodes").append(data);
            // only animate the node if this is NOT the first page load
            if (!FirstLoad) {
                $(".livemap[data-server-id='" + livemap_server.server_id + "'] .livemap-node-container[data-steam-id='" + player_data.CSteamID + "']").velocity("transition.bounceIn", { drag: true });
            }
        },
        error: function(e) {
            console.log(e);
        }
    });
}
// removes player node
function RemovePlayerNode(steam_id) {
    $(".livemap-node-container[data-steam-id='" + steam_id + "']").hide();
}

/**
 * UPDATE PLAYER BADGES
 * 
 * This function iterates through returned livemap_data (player data) from 
 * the most recent livemap refresh, then updates each player badge accordingly
 * @param {obj} livemap_data Returned JSON player data
 */
function UpdatePlayerBadges(livemap_data) {
    jQuery.each(livemap_data, function(index, player) {
        // update badge opacity if player is hidden or vanished
        if (player.is_hidden == "1" && !$(".livemap[data-server-id='" + player.server_id + "'] .livemap-badge-container[data-steam-id='" + player.CSteamID + "']").hasClass("is_hidden")) {
            // hide the player badge and node
            $(".livemap[data-server-id='" + player.server_id + "'] .livemap-badge-container[data-steam-id='" + player.CSteamID + "']").addClass("is_hidden");
            $(".livemap[data-server-id='" + player.server_id + "'] .livemap-badge-container[data-steam-id='" + player.CSteamID + "']").velocity("transition.livemapBadgeHideIn", { stagger: 300 });
            $(".livemap[data-server-id='" + player.server_id + "'] .livemap-node-container[data-steam-id='" + player.CSteamID + "']").velocity("transition.whirlOut", { stagger: 1000 });
            // notify world chat
            ChatWorld(player.server_id, "Server", player.character_name + " has hidden from the Livemap!", "yellow");
        }
        else if (player.is_hidden == "0" && $(".livemap[data-server-id='" + player.server_id + "'] .livemap-badge-container[data-steam-id='" + player.CSteamID + "']").hasClass("is_hidden")) {
            // unhide the player
            $(".livemap[data-server-id='" + player.server_id + "'] .livemap-badge-container[data-steam-id='" + player.CSteamID + "']").removeClass("is_hidden");
            $(".livemap[data-server-id='" + player.server_id + "'] .livemap-badge-container[data-steam-id='" + player.CSteamID + "']").velocity("transition.livemapBadgeUnhideIn", { stagger: 300 });
            $(".livemap[data-server-id='" + player.server_id + "'] .livemap-node-container[data-steam-id='" + player.CSteamID + "']").velocity("transition.whirlIn", { stagger: 1000 });
            // notify world chat
            ChatWorld(player.server_id, "Server", player.character_name + " has unhidden from the Livemap!", "yellow");
        }
    })
}

/**
 * ADD PLAYER BADGE
 * 
 * This function passes a subset of player data to a badge template via AJAX
 * which returns a player populated (name)badge element, which is appended to
 * .livemap-badges container element
 * @param {obj} player_data Returned JSON player data
 */
function AddPlayerBadge(player_data) {
    $.ajax({
        type: "GET",
        url: "templates/badge.livemap.php",
        data: {
            // send player data
            CSteamID: player_data.CSteamID,
            CharacterName: player_data.character_name,
            Reputation: returnPlayerBadgeReputationString(player_data.reputation),
            ReputationName: returnPlayerBadgeReputationName(player_data.reputation),
            Avatar: player_data.steam_avatar_medium,
            BadgeColor: returnPlayerType(player_data),
            TypeIcon: returnPlayerBadgeTypeIcon(player_data)
        },
        success: function(data) {
            $(".livemap[data-server-id='" + player_data.server_id + "'] .livemap-badges-container").append(data);
            if (!FirstLoad) {
                $(".livemap[data-server-id='" + player_data.server_id + "'] .livemap-badge-container[data-steam-id='" + player_data.CSteamID + "']").velocity("transition.bounceIn", { drag: true });
            }
        },
        error: function(e) {
            console.log(e);
        }
    });
}
function RemovePlayerBadge(steam_id) {
    $(".livemap-badge-container[data-steam-id='" + steam_id + "']").hide();
}

/**
 * UPDATE WORLD CHAT
 * 
 * This function updates world chat by sending a request to the API and then
 * appending the chat window with the newest chat data
 * @param {object} livemap_chat Livemap chat data
 * @param {object} livemap_server Livemap server data
 */
function UpdateWorldChat(livemap_chat = null, livemap_server) {
    // if `server_id` key does not exist, create it
    if (!(livemap_server.server_id in LastWorldChatID)) {
        LastWorldChatID[livemap_server.server_id] = 0;
    }

    // check if chat is empty
    if (livemap_chat !== null)
    {
        // check for initial page load
        if (LastWorldChatID[livemap_server.server_id] > 0) {
            var newestMsgID = 0;
            jQuery.each(livemap_chat, function(i, val) {
                // show chat if it's hidden
                if ($(".livemap[data-server-id='" + val.server_id + "'] .livemap-chat").hasClass("hidden")) {
                    $(".livemap[data-server-id='" + val.server_id + "'] .livemap-chat").removeClass("hidden");
                    $(".livemap[data-server-id='" + val.server_id + "'] .livemap-chat").velocity("transition.flipBounceYIn", { drag: true });
                }

                // generate new message
                if (val.id > LastWorldChatID[livemap_server.server_id]) {

                    var CharacterName = val.character_name;
                    var Avatar = val.steam_avatar_medium;
                    var Message = val.message;
                    var IsAdmin = val.is_admin;

                    // create new message
                    var data = '<div class="media" id="msg' + val.id + '"><div class="media-left"><img class="media-object" src="'+ Avatar +'" alt=""></div><div class="media-body"><p class="'+ (IsAdmin == 1 ? "admin" : "") +'">[World] '+ CharacterName +': '+ Message +'</p></div></div>';
                    $(".livemap[data-server-id='" + val.server_id + "'] .livemap-chat").append(data);
                    
                    // scroll to bottom of chat
                    updateWorldChatScroll(val.server_id);

                    // animate new message
                    $("#msg" + val.id).velocity("transition.slideUpIn", { stagger: 150, drage: true });
                }

                if (newestMsgID < val.id) { newestMsgID = val.id; }
            });

            LastWorldChatID[livemap_server.server_id] = newestMsgID;
        }
        else
        {
            // initial page load
            jQuery.each(livemap_chat, function(i, val) {
                var CharacterName = val.character_name;
                var Avatar = val.steam_avatar_medium;
                var Message = val.message;
                var IsAdmin = val.is_admin;

                // create new message
                var data = '<div class="media"><div class="media-left"><img class="media-object" src="'+ Avatar +'" alt=""></div><div class="media-body"><p class="'+ (IsAdmin == 1 ? "admin" : "") +'">[World] '+ CharacterName +': '+ Message +'</p></div></div>';
                $(".livemap[data-server-id='" + val.server_id + "'] .livemap-chat").prepend(data);

                // update last message id
                if (LastWorldChatID[livemap_server.server_id] == 0) {
                    LastWorldChatID[livemap_server.server_id] = val.id;
                }
            });

            // scroll to bottom of chat
            updateWorldChatScroll(livemap_server.server_id);

            // show the chat
            $(".livemap[data-server-id='" + livemap_server.server_id + "'] .livemap-chat").velocity("transition.flipBounceYIn", { drag: true });
            // $(".livemap[data-server-id='" + livemap_server.server_id + "'] .livemap-chat .media").velocity("transition.flipBounceYIn", { stagger: 50, drag: true, backwards: true });
        }
    }
    else
    {
        // chat is empty, hide it
        $(".livemap[data-server-id='" + livemap_server.server_id + "'] .livemap-chat").addClass("hidden");
    }
    
    // div is draggable
    $(".livemap[data-server-id='" + livemap_server.server_id + "'] .livemap-chat").draggable();
}

/**
 * SEND MESSAGE TO WORLD CHAT
 * 
 * This function allows for sending chat messages to a specific world chat
 * overlay. If no color option is passed, it defaults to "lime"
 * @param {string} server_id Server instance name
 * @param {string} name Name of sender
 * @param {string} message Message text
 * @param {string} color Optional color of message
 */
function ChatWorld(server_id,name,message,color=null) {
    var data = '<div class="media" id="msgSYSTEM"><div class="media-left"><img class="media-object" src="images/avatars/server.jpg" alt=""></div><div class="media-body"><p style="font-weight:bold;color:'+ (color == null ? "lime" : color) +'">[World] ' + name + ': ' + message +'</p></div></div>';
    $(".livemap[data-server-id='" + server_id + "'] .livemap-chat").append(data);
    // scroll to bottom of chat
    updateWorldChatScroll(server_id);
}

/**
 * This function updates a targeted world chat overlay scroll bar, and forces
 * it to scroll to the bottom of the container
 * @param {string} server_id Server instance name
 */
function updateWorldChatScroll(server_id){
    var livemapChatScroll = $(".livemap[data-server-id='" + server_id + "'] .livemap-chat");
    $(livemapChatScroll).stop().scrollTop($(livemapChatScroll)[0].scrollHeight);
}

/**
 * CONVERT VECTOR3 TO PIXEL COORDINATES
 * 
 * This function converts player vector3 position data into a
 * location on the livemap
 * @param {string} server_id Server identifier
 * @param {string} vector3 Position data: "(x,z,y)"
 */
function CalculateVectorPosition(server_id,map,vector3) {
    // let's pre-define default map dimensions
    // since all maps are square, we can store just one axis
    var monolith = 1024;
    var pei = 2048;
    var russia = 4096;
    var washington = 2048;
    var yukon = 2048;
    var hawaii = 4096;
    var germany = 4096;

    /**
     * this offset is needed due to the difference between map
     * dimensions and player constraints. i.e. washington is set to
     * 2048x2048, total = 4096, but map bounds stop players at
     * 1983.5 on all four sides of the map. 1983.5 * 2 = 3967, so...
     * offset calculation (washington): 4096 - (1983.5 * 2) = 129 
     */
    var offset = 129; //(eval(map.toLowerCase()) * 2) / 960 * 100;

    // store the targeted livemap element for safe keeping
    var livemap = $(".livemap[data-server-id='" + server_id + "'] .livemap-nodes");
    
    // set the mapsize variable to the targeted map 
    var mapsize = eval(map.toLowerCase());
    
    // seperate position data
    vector3 = vector3.substring(1); // remove (
    vector3 = vector3.substring(0, vector3.length - 1); // remove )
    vector3 = vector3.split(",");
    
    // calculate the map aspect ratio [dependent on screen/map dimensions]
    var aspectRatio = (mapsize - offset) / livemap.width();
    
    // calculate the x and y locations, pixel-style! *dances*
    var x = ((vector3[0] / aspectRatio) + livemap.width() / 2);
    var y = ((vector3[2] / aspectRatio) + livemap.height() / 2);
    
    return [x,y];
}

/**
 * RETURN PLAYER NODE POSITIONING CSS STRING
 * 
 * this function returns a css formatted string for a player node
 * position and is applied to 'node.livemap.php'
 * @param {string} server_id Server identifier
 * @param {string} Vector3 position data
 */
function returnNodePositionStyle(server_id,map,vector3) {
    // get the player position
    var vector = CalculateVectorPosition(server_id,map,vector3);
    // return styled position data
    return "left:" + vector[0] + "px;bottom:" + vector[1] + "px";
}

/**
 * RETURN PLAYER NODE FACE
 * 
 * This function returns the player face id. By default, unchanged faces return
 * as '255', so these are defaulted to 1
 * @param {obj} player_data 
 */
function returnNodeFace(player_data) {
    if (player_data.face == "255") {
        return "1";
    } else {
        return parseInt(player_data.face) + 1;
    }
}

function returnPlayerBadgeReputationName(reputation) {
    // neutral reputation
    if (reputation == 0) {
        return "neutral";
    // positive reputation
    } else if (reputation >= 1 && reputation <= 7) {
        return "vigilante";
    } else if (reputation >= 8 && reputation <= 32) {
        return "constable";
    } else if (reputation >= 33 && reputation <= 99) {
        return "deputy";
    } else if (reputation >= 100 && reputation <= 199) {
        return "sheriff";
    } else if (reputation >= 200) {
        return "paragon";
    // negative reputation
    } else if (reputation <= -1 && reputation >= -7) {
        return "thug";
    } else if (reputation <= -8 && reputation >= -32) {
        return "outlaw";
    } else if (reputation <= -33 && reputation >= -99) {
        return "gangster";
    } else if (reputation <= -100 && reputation >= -199) {
        return "bandit";
    } else if (reputation <= -200) {
        return "villain";
    }
}
/**
 * RETURN PLAYER BADGE REPUTATION STRING
 * 
 * This function returns a formatted string containing a player's reputation 
 * level name, and reputation integer (i.e. "Villain [-2560]")
 * @param {int} reputation Player reputation integer
 */
function returnPlayerBadgeReputationString(reputation) {
    // neutral reputation
    if (reputation == 0) {
        return "Neutral [" + reputation + "]";
    // positive reputation
    } else if (reputation >= 1 && reputation <= 7) {
        return "Vigilante [" + reputation + "]";
    } else if (reputation >= 8 && reputation <= 32) {
        return "Constable [" + reputation + "]";
    } else if (reputation >= 33 && reputation <= 99) {
        return "Deputy [" + reputation + "]";
    } else if (reputation >= 100 && reputation <= 199) {
        return "Sheriff [" + reputation + "]";
    } else if (reputation >= 200) {
        return "Paragon [" + reputation + "]";
    // negative reputation
    } else if (reputation <= -1 && reputation >= -7) {
        return "Thug [" + reputation + "]";
    } else if (reputation <= -8 && reputation >= -32) {
        return "Outlaw [" + reputation + "]";
    } else if (reputation <= -33 && reputation >= -99) {
        return "Gangster [" + reputation + "]";
    } else if (reputation <= -100 && reputation >= -199) {
        return "Bandit [" + reputation + "]";
    } else if (reputation <= -200) {
        return "Villain [" + reputation + "]";
    }
}

function returnVehicleIcon(player_data) {
    switch (player_data.vehicle_id) {
        case "34":
            return "firetruck";
        case "53":
            return "apc";
        case "54":
            return "ambulance";
        case "58":
            return "explorer";
        case "86":
            return "bus";
        case "140":
            return "jet";
        default:
            return "apc";
    }
}

/**
 * RETURN ONLINE PLAYER COUNT STRING
 * 
 * This function returns a string containing the current online player count / 
 * max player count [i.e. "16/24"] of the livemap_server that is passed.
 * @param {obj} livemap_server Returned JSON server data
 */
function returnOnlinePlayerCountString(livemap_server) {
    return livemap_server.online_players + "/" + livemap_server.max_players;
}

/**
 * RETURN PLAYER TYPE
 * 
 * This function returns the player type based on a priority order. For example
 * if the player has gold but is an admin, they will be shown as an admin. 9
 * @param {obj} player_data Returned player data
 */
function returnPlayerType(player_data) {
    if (player_data.is_admin == 1) { // admin
        return "admin";
    } else if (player_data.is_pro == 1) { // gold
        return "gold";
    } else if (player_data.in_vehicle == 1) { // in vehicle
        return "vehicle";
    } else {
        return "normal"; // normal player
    }
}

function returnPlayerBadgeTypeIcon(player) {
    if (player.is_admin == 1) {
        return "admin";
    } else if (player.is_pro == 1) {
        return "gold";
    }
}

function setRefreshIntervals() {
    ActiveInstances.forEach(function(instance) {
        Intervals[instance.toLowerCase()] = setInterval(TriggerLivemapRefresh, RefreshInterval[instance]);
        Heartbeats[instance.toLowerCase()] = setInterval(Heartbeat, 1000); // 1 second
    });
}

/**
 * Custom Velocity.js Transitions
 * 
 * This codeblock contains custom Velocity.js transitions used to animate
 * player (name)badges and node icons.
 */
$.Velocity.RegisterUI("transition.livemapBadgeHideIn", {
    defaultDuration: 3000,
    calls: [
        [ { scaleX: [1, 0.625], scaleY: [1, 0.625], translateZ: 0 }, 0.1, { easing: "spring" } ],
        [ { rotateX: -45 }, 0.25, { easing: "spring" } ],
        [ { opacity: [0.2, 1], rotateX: 1080 }, 0.25, { easing: "spring" } ]
    ]
});
$.Velocity.RegisterUI("transition.livemapBadgeUnhideIn", {
    defaultDuration: 3000,
    calls: [
        [ { scaleX: [1, 0.625], scaleY: [1, 0.625], translateZ: 0 }, 0.1, { easing: "spring" } ],
        [ { rotateX: 90 }, 0.25, { easing: "spring" } ],
        [ { opacity: [1, 0.2], rotateX: -1080 }, 0.25, { easing: "spring" } ]
    ]
});

// show loading screen during ajax requests
$(document).on({
    ajaxStart: function() { 
        $("img.heartbeat").velocity("callout.tada", { queue:false, duration: 20 });
        $("img.heartbeat").attr("src", "images/icons/heart/10.png");
    },
    ajaxStop: function() { 
        //$(".livemap-loading").addClass("hidden");
        $("img.heartbeat").attr("src", "images/icons/heart/1.png");
        RefreshCounter = 1;

        if (FirstLoad) {
            // animate load sequence
            $(".livemap-badge-container").velocity("transition.bounceIn", { stagger: 165, drag: true });
            $(".livemap-node-container").velocity("transition.bounceIn", { stagger: 165, drag: true });

            // set refresh intervals
            setRefreshIntervals();
            FirstLoad = false;
        }
    }
});

/*
$(document).on('mouseenter', '.livemap-badge-container', function(e) {
    $(".livemap-node-container[data-steam-id='" + $(e.target).data('steam-id') + "'] .livemap-node-name").velocity("transition.fadeIn", {queue: false});
}).on('mouseleave', '.livemap-badge-container', function(e) {
    $(".livemap-node-name").velocity("transition.fadeOut", {queue: false});
});
*/