/**
 * Livemaps v0.3
 * author: Nexis <nexis@nexisrealms.com>
 */

/* Default Node Skin Color */
var DeadSkinColor = '#888888';
/* Max Refreshes */
var MaxRefresh = 60;
/* Refresh Interval */
var RefreshInterval = 10000;

/* ================ [ DO NOT EDIT BELOW THIS LINE ] ================ */

/* Connected Players [CSteam64ID] */
var ConnectedPlayers = [];
/* Hidden Players */
var HiddenPlayers = [];
/* Heartbeat Refresh Counter */
var RefreshCounter = 10;
/* Total Refresh Counter */
var TotalRefreshes = 0;
/* First Page Load */
var FirstLoad = true;
/* Page Timeout */
var Timeout = false;

/**
 * INITIATE LIVEMAP
 * This function queries the API for active server data. If any active servers
 * are found, all relevant data is then queried and loaded to the page
 */
function init() {
    console.log("Initializing Livemaps...");

    $.ajax({
        dataType: "json",
        type: "GET",
        url: "api/data.json",

        success: function(data) {
            LoadLivemap(data);
        },
        error: function(xhr, status, error) {
            AjaxErrorHandler(xhr, status, error);
        }
    });
}

/**
 * HEARTBEAT
 * This function serves as the heartbeat for the livemap refresh sequence
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
 * AJAX ERROR HANDLER
 * This function is for debugging, and reports errors to browser console
 */
function AjaxErrorHandler(request, status, error) {
    console.log("ERROR! (" + status + ")");
    console.log("MESSAGE: " + error);
    console.log("API RESPONSE: " + request.responseText);
}

/**
 * LOAD INITIAL LIVEMAP
 * This function loads the initial livemap template and if successful
 * refreshes the livemap to add connected players to view. This function
 * only executes on the first page load.
 * @param {obj} data JSON server data
 */
function LoadLivemap(data) {
    console.log("Loading Livemap...");

    // generate the map
    $.ajax({
        type: "GET",
        url: "templates/layout.livemap.php",
        data: {
            // send server data
            ServerID: data.Server.ID,
            Map: data.Server.Map,
            PlayersOnline: data.Server.PlayersOnline + "/" + data.Server.MaxPlayers,
            ConnectAddress: data.Server.Connect
        },
        success: function(layout) {
            // add generated map to the page
            $(".livemaps").append(layout);
            ChatWorld(data.Server.ID, "Server", "Connected to in-game chat!", "yellow");


            // refresh the livemap, on a delay
            setTimeout(function() {
                LoadMapLabels(data);
                RefreshLivemap(data);
            }, 1000);
        },
        error: function(xhr, status, error) {
            AjaxErrorHandler(xhr, status, error);
        }
    });
}

function LoadMapLabels(data) { 
    $.getJSON("images/maps/"+data.Server.Map.toLowerCase()+"/labels.json", function(json) {
        jQuery.each(json, function(name, position) {
            var pos = CalculateVectorPosition(data.Server.ID,data.Server.Map,position);
            var label = "<div class=\"label\" id=\""+name.toLowerCase().replace(/\s/g, '')+"\" style=\"bottom:"+pos[1]+"px;left:"+pos[0]+"px\">"+name+"</div>";
            $(".livemap-labels").append(label);
            $(".livemap-labels .label#"+name.toLowerCase().replace(/\s/g, '')).velocity("transition.flipXIn");
        });
    });
}

function CycleTick(data) {
    var deg = (data.Server.Time * 360) / data.Server.Cycle;
    $(".livemap-time img").css({'transform' : 'rotate('+ deg +'deg)'});
}

/**
 * TRIGGER LIVEMAP REFRESH
 */
function TriggerLivemapRefresh() {
    $.ajax({
        dataType: "json",
        type: "GET",
        url: "api/data.json",

        success: function(data) {
            RefreshTimeout(data);
            if (!Timeout)
                RefreshLivemap(data);
        },
        error: function(xhr, status, error) {
            AjaxErrorHandler(xhr, status, error);
        }
    });
}

/**
 * REFRESH LIVEMAP
 * This function executes all functions that update the livemap with newest
 * data from server.
 * @param {obj} data JSON server data
 */
function RefreshLivemap(data) {
    console.log("Refreshing Livemap...");

    UpdateConnectedPlayersArray(data);
    UpdateLivemapHUD(data);
    UpdatePlayerBadges(data);
    UpdatePlayerNodes(data);
    UpdateChat(data); 
    CycleTick(data);   
}

function RefreshTimeout(data) {
    var now = new Date().getTime();
    var lastRefresh = new Date(data.Server.LastRefresh).getTime();

    if ((now - lastRefresh) > 30000) {
        console.log("SERVER TIMEOUT");

        if ($(".livemap-static-container").hasClass("hidden")) {
            Timeout = true;
            $(".livemap-static-container").removeClass("hidden");
            ChatWorld(data.Server.ID, "Server", "Livemap connection has been lost!", "yellow");
        }        
    } else {
        if (!$(".livemap-static-container").hasClass("hidden")) {
            Timeout = false;
            $(".livemap-static-container").addClass("hidden");
            ChatWorld(data.Server.ID, "Server", "Livemap connection has been restored!", "yellow");
        }           
    }
}

/**
 * UPDATE CONNECTED PLAYERS ARRAY
 * This function iterates through the ConnectedPlayers object (before updating
 * anything else) and proceeds to search for a matching steam id within the 
 * most recently returned JSON request. Matching results are ignored. If a 
 * match is/is not found, the player node and (name)badge will be removed/added
 * @param {obj} data JSON server data
 */
function UpdateConnectedPlayersArray(data) {
    
    var disconnectMatch = false;
    var connectMatch = false;
    var connectedPlayers = ConnectedPlayers;
    var server = data.Server;
    var players = data.Players;

    /* check if player connected */
    jQuery.each(players, function(index, player) {

        // check if player exists in ConnectedPlayers array
        for (var key in connectedPlayers) {          
            if (player.CharacterName == connectedPlayers[key]) {
                connectMatch = true;                
            }
        }
        
        // if no match was found, this player has connected
        if (!connectMatch) {
            PlayerConnected(index,player,server); // create new player
            console.log(player.CharacterName +" connected");
        }
        connectMatch = false; // reset
    });

    /* check if player disconnected */
    for (var key in connectedPlayers) {
        // check if player exists in latest refresh
        jQuery.each(players, function(index, player) {

            if (key == index) {
                disconnectMatch = true;
            }
        });
        // if no match was found, this player has disconnected
        if (!disconnectMatch) {
            console.log(connectedPlayers[key] +" disconnected");
            PlayerDisconnected(server.ID,key,connectedPlayers[key]); // remove player
        }
        disconnectMatch = false; // reset
    }    
}

/**
 * PLAYER CONNECTED
 * This function generates new player DOM elements when a player connects to a
 * server. It adds the player to the ConnectedPlayers object and announces the
 * new connection in chat
 * @param {string} id Steam ID
 * @param {obj} player Player data
 * @param {obj} server Server data
 */
function PlayerConnected(id,player,server) {
    // add player to connected player array
    ConnectedPlayers[id] = player.CharacterName;

    AddPlayerBadge(id,player,server);
    AddPlayerNode(id,player,server);

    // notify chat that player connected
    ChatWorld(server.ID, "Server", player.CharacterName + " has connected!", "lime");
}

/**
 * PLAYER DISCONNECTED
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
    
    // remove player from ConnectedPlayers array
    delete ConnectedPlayers[steam_id];

    // notify chat that player disconnected
    ChatWorld(server_id, "Server", character_name + " has disconnected!", "red");
}

/**
 * UPDATE LIVEMAP HUD DATA
 * This function updates the HUD data with the most recent, as well as updating
 * the (name)badge column's player online count.
 * @param {obj} data JSON server data
 */
function UpdateLivemapHUD(data) {
    // server title name
    $(".server-name").text(data.Server.Name);
    // badges online player count
    $(".livemap[data-server-id='" + data.Server.ID + "'] .livemap-badges-online-players span.player-count").html(data.Server.PlayersOnline + "/" + data.Server.MaxPlayers);
    // online player count
    $(".livemap[data-server-id='" + data.Server.ID + "'] .livemap-hud-online-players").html(data.Server.PlayersOnline + "/" + data.Server.MaxPlayers);
}

/**
 * UPDATE PLAYER NODES
 * This function iterates through each player and updates their node's position
 * on the livemap
 * @param {obj} data JSON server data
 */
function UpdatePlayerNodes(data) {
    jQuery.each(data.Players, function(index, player) {
        // update stats
        $(".livemap-node-container[data-steam-id='" + index + "']").data("health", player.Health);
        $(".livemap-node-container[data-steam-id='" + index + "']").data("hunger", player.Hunger);
        $(".livemap-node-container[data-steam-id='" + index + "']").data("thirst", player.Thirst);
        $(".livemap-node-container[data-steam-id='" + index + "']").data("infection", player.Infection);
        $(".livemap-node-container[data-steam-id='" + index + "']").data("stamina", player.Stamina);

        // hide player if vanished
        if (player.VanishMode == "True" || player.Hidden == "True") {
            if (!(player.CharacterName in HiddenPlayers)) {
                $(".livemap[data-server-id='" + data.Server.ID + "'] .livemap-badge-container[data-steam-id='" + index + "']").velocity("transition.livemapBadgeHideIn");
                $(".livemap-node-container[data-steam-id='" + index + "']").velocity("transition.bounceUpOut", { drag: true });

                HiddenPlayers[player.CharacterName] = player.CharacterName;
            }
        } else {
            if (player.CharacterName in HiddenPlayers) {
                $(".livemap[data-server-id='" + data.Server.ID + "'] .livemap-badge-container[data-steam-id='" + index + "']").velocity("transition.livemapBadgeUnhideIn");
                $(".livemap-node-container[data-steam-id='" + index + "']").velocity("transition.bounceDownIn", { drag: true });

                delete HiddenPlayers[player.CharacterName];
            }
        }

        // update player node icon
        if (player.Dead == "True") {
            // player is dead
            $(".livemap[data-server-id='" + data.Server.ID + "'] .livemap-node-container[data-steam-id='" + index + "'] img.livemap-node").attr("src", "images/nodes/faces/dead.png");
            $(".livemap[data-server-id='" + data.Server.ID + "'] .livemap-node-container[data-steam-id='" + index + "'] img.livemap-node").css({"background-color": DeadSkinColor});
        } else if (player.IsInVehicle == "True") {
            // update player node icon to vehicle icon
            $(".livemap[data-server-id='" + data.Server.ID + "'] .livemap-node-container[data-steam-id='" + index + "'] img.livemap-node").attr("src", "images/nodes/vehicles/apc.png");
            $(".livemap[data-server-id='" + data.Server.ID + "'] .livemap-node-container[data-steam-id='" + index + "'] img.livemap-node").css({"background-color":"transparent",border:0});
        } else {
            // update player node face and color
            $(".livemap[data-server-id='" + data.Server.ID + "'] .livemap-node-container[data-steam-id='" + index + "'] img.livemap-node").attr("src", "images/nodes/faces/"+ returnNodeFace(player) +".png");
            $(".livemap[data-server-id='" + data.Server.ID + "'] .livemap-node-container[data-steam-id='" + index + "'] img.livemap-node").css({"background-color":returnNodeFaceColor(player)});
        }

        // calculate new player position
        var calcPos = CalculateVectorPosition(data.Server.ID, data.Server.Map, player.Position);

        // animate player node to new position
        $(".livemap[data-server-id='" + data.Server.ID + "'] .livemap-node-container[data-steam-id='" + index + "']").velocity({
            left: calcPos[0],
            bottom: calcPos[1]
        },
        {
            duration: 10000, 
            easing: "linear"
        });
    });
}

/**
 * ADD PLAYER NODE
 * This function creates a new player node and places them on the map where
 * they are currently located in-game
 * @param {string} id Steam ID
 * @param {obj} player Player data
 * @param {obj} server Server data
 */
function AddPlayerNode(id,player,server) {
    $.ajax({
        type: "GET",
        url: "templates/node.livemap.php",
        data: {
            // send player data
            CSteamID: id,
            CharacterName: player.CharacterName,
            Position: returnNodePositionStyle(server.ID, server.Map, player.Position),
            SkinColor: "background-color:" + returnNodeFaceColor(player),
            Face: returnNodeFace(player),
            Health: player.Health,
            Hunger: player.Hunger,
            Thirst: player.Thirst,
            Infection: player.Infection,
            Stamina: player.Stamina
        },
        success: function(data) {
            // add node to Livemap and animate
            $(".livemap[data-server-id='" + server.ID + "'] .livemap-nodes").append(data);
            $(".livemap[data-server-id='" + server.ID + "'] .livemap-node-container[data-steam-id='" + id + "']").velocity("transition.bounceDownIn", { drag: true });
        },
        error: function(xhr, status, error) {
            AjaxErrorHandler(xhr, status, error);
        }
    });
}
/** REMOVE PLAYER NODE */
function RemovePlayerNode(steam_id) {
    $(".livemap-node-container[data-steam-id='" + steam_id + "']").velocity("transition.bounceUpOut", { drag: true });
    setTimeout(function() {
        $(".livemap-node-container[data-steam-id='" + steam_id + "']").remove();
    }, 1000);
}

/**
 * UPDATE PLAYER BADGES
 * This function iterates through each player's newest JSON data and updates  
 * each badge element accordingly
 * @param {obj} data JSON server data
 */
function UpdatePlayerBadges(data) {
    jQuery.each(data.Players, function(index, player) {
        // update reputation text and color
        $(".livemap-badge-container[data-steam-id='" + index + "'] .player-reputation").attr('class', 'player-reputation');
        $(".livemap-badge-container[data-steam-id='" + index + "'] .player-reputation").addClass(returnPlayerBadgeReputationName(player.Reputation));
        $(".livemap-badge-container[data-steam-id='" + index + "'] .player-reputation").text(returnPlayerBadgeReputationString(player.Reputation));
        // update reputation icon
        $(".livemap-badge-container[data-steam-id='" + index + "'] .player-reputation-icon").attr("src", "images/icons/reputations/"+returnPlayerBadgeReputationName(player.Reputation)+".png");
    });
}

/**
 * ADD PLAYER BADGE
 * This function passes a subset of player data to a badge template via AJAX
 * which returns a player populated (name)badge element.
 * @param {string} id Steam ID
 * @param {obj} player Player data
 * @param {obj} server Server data
 */
function AddPlayerBadge(id,player,server) {
    $.ajax({
        type: "GET",
        url: "templates/badge.livemap.php",
        data: {
            // send player data
            CSteamID: id,
            CharacterName: player.CharacterName,
            Reputation: returnPlayerBadgeReputationString(player.Reputation),
            ReputationName: returnPlayerBadgeReputationName(player.Reputation),
            Avatar: player.Avatar,
            BadgeColor: returnPlayerType(player),
            TypeIcon: returnPlayerBadgeTypeIcon(player)
        },
        success: function(data) {
            // add badge to livemap and animate
            $(".livemap[data-server-id='" + server.ID + "'] .livemap-badges-container").append(data);
            $(".livemap[data-server-id='" + server.ID + "'] .livemap-badge-container[data-steam-id='" + id + "']").velocity("transition.flipBounceYIn", { drag: true });
        },
        error: function(xhr, status, error) {
            AjaxErrorHandler(xhr, status, error);
        }
    });
}
/** REMOVE PLAYER BADGE */
function RemovePlayerBadge(steam_id) {
    $(".livemap-badge-container[data-steam-id='" + steam_id + "']").velocity("transition.bounceOut", { drag: true });
    setTimeout(function() {
        $(".livemap-badge-container[data-steam-id='" + steam_id + "']").remove();
    }, 1000);
}

/**
 * UPDATE CHAT
 * This function adds any new world chat messages to the HUD
 * @param {obj} data JSON server data
 */
function UpdateChat(data) {    
    jQuery.each(data.Chat, function(index, msg) {
        var message = '<div class="media" id="msg'+index+'"><div class="media-left"><img class="media-object" src="'+ msg.Avatar +'" alt=""></div><div class="media-body"><p class="'+ (msg.isAdmin == "True" ? "admin" : "") +'">'+ msg.CharacterName +': '+ msg.Message +'</p></div></div>';
        
        $(".livemap-chat").append(message);
        updateWorldChatScroll(data.Server.ID);

        console.log("Chat: " + msg.CharacterName + ": " + msg.Message);
    });
}

/**
 * SEND MESSAGE TO WORLD CHAT
 * This function allows for sending chat messages to a specific world chat
 * overlay. If no color option is passed, it defaults to "lime"
 * @param {string} server_id Server instance name
 * @param {string} name Name of sender
 * @param {string} message Message text
 * @param {string} color Optional color of message
 */
function ChatWorld(server_id,name,message,color=null) {
    var data = '<div class="media" id="msgSYSTEM"><div class="media-left"><img class="media-object" src="images/avatars/server.jpg" alt=""></div><div class="media-body"><p style="font-weight:bold;color:'+ (color == null ? "lime" : color) +'">' + name + ': ' + message +'</p></div></div>';
    
    $(".livemap[data-server-id='" + server_id + "'] .livemap-chat").append(data);
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
 * This function converts player vector3 position data into a
 * location on the livemap
 * @param {string} server_id Server identifier
 * @param {string} map Map name
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
    var belgium = 4096;
    var riodejaneiro = 4096;

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
    
    //remove spaces from map name
    map = map.replace(/\s/g, '');
    
    // set the mapsize variable to the targeted map 
    var mapsize = eval(map.toLowerCase());
    
    // seperate position data
    //vector3 = vector3.substring(1); // remove (
    //vector3 = vector3.substring(0, vector3.length - 1); // remove )
    vector3 = vector3.replace(/\(|\)/g, '');
    vector3 = vector3.split(",");
    
    // calculate the map aspect ratio [dependent on screen/map dimensions]
    var aspectRatio = (mapsize - offset) / livemap.width();
    
    // calculate the x and y locations, pixel-style! *dances*
    var x = ((vector3[0] / aspectRatio) + livemap.width() / 2);
    var y = ((vector3[2] / aspectRatio) + livemap.height() / 2);
    
    // icon correction
    x -= 10;
    y -= 10;

    return [x,y];
}

/**
 * RETURN PLAYER NODE POSITIONING CSS STRING
 * this function returns a css formatted string for a player node
 * position and is applied to 'node.livemap.php'
 * @param {string} server_id Server identifier
 * @param {string} map Map name
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
 * This function returns the player face id. By default, unchanged faces return
 * as '255', so these are defaulted to 1
 * @param {obj} player Player data 
 */
function returnNodeFace(player) {
    if (player.Dead == "True")
        return "dead";
    if (player.Face == "255") {
        return "1";
    } else {
        return parseInt(player.Face) + 1;
    }
}

function returnNodeFaceColor(player) {
    if (player.Dead == "True")
        return DeadSkinColor;
    else
        return "#CCCC91";
}

/**
 * RETURN REPUTATION NAME
 * This function returns the name of the reputation associated with the 
 * player's current reputation level
 * @param {int} reputation Reputation level
 */
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
 * This function returns a formatted string containing a player's reputation 
 * level name, and reputation integer (i.e. "Villain [-2560]")
 * @param {int} reputation Reputation level
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

/**
 * RETURN PLAYER TYPE
 * This function returns the player type based on a priority order. For example
 * if the player has gold but is an admin, they will be shown as an admin.
 * @param {obj} player Player data
 */
function returnPlayerType(player) {
    if (player.Admin == 1) { // admin
        return "admin";
    } else if (player.Pro == 1) { // gold
        return "gold";
    } else if (player.Vehicle == 1) { // in vehicle
        return "vehicle";
    } else {
        return "normal"; // normal player
    }
}

/**
 * RETURN BADGE TYPE
 * This function returns the player's badge type (admin/gold)
 * @param {obj} player Player data
 */
function returnPlayerBadgeTypeIcon(player) {
    if (player.Admin == 1) {
        return "admin";
    } else if (player.Pro == 1) {
        return "gold";
    }
}

function setRefreshIntervals() {
    setInterval(TriggerLivemapRefresh, RefreshInterval);
    setInterval(Heartbeat, 1000);
}

$(document).on("mouseenter", ".livemap-node-container", function() {
    $(".node-info").css("display", "inline");
    var steamId = $(this).data("steam-id");
    var name = $(this).data("name");
    var health = $(this).data("health");
    var hunger = $(this).data("hunger");
    var thirst = $(this).data("thirst");
    var infection = $(this).data("infection");
    var stamina = $(this).data("stamina");

    $(".node-info .name").text(name);
    $(".node-info .health .progress-bar").css("width", health+"%");
    $(".node-info .hunger .progress-bar").css("width", hunger+"%");
    $(".node-info .thirst .progress-bar").css("width", thirst+"%");
    $(".node-info .infection .progress-bar").css("width", infection+"%");
    $(".node-info .stamina .progress-bar").css("width", stamina+"%");
});

$(document).on("mouseleave", ".livemap-node-container", function() {
    $(".node-info").css("display", "none");
});

/**
 * Custom Velocity.js Transitions
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

$(document).on({
    ajaxStart: function() { 
        $("img.heartbeat").velocity("callout.tada", { queue:false, duration: 20 });
        $("img.heartbeat").attr("src", "images/icons/heart/10.png");
    },
    ajaxStop: function() { 
        $("img.heartbeat").attr("src", "images/icons/heart/1.png");
        RefreshCounter = 1;

        if (FirstLoad) {
            // set refresh intervals
            setRefreshIntervals();
            FirstLoad = false;
        }
    }
});


