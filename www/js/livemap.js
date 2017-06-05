/**
 * Disable Specific Livemaps
 * Use the following array to disable specific livemaps that you do not want to
 * display on the page by entering their server instance names below
 * NOTE: **lowercase only**
 */
var DisabledLivemaps = [];
/* var DisabledLivemaps = ['server1', 'pei']; */

/**
 * Livemap Hidding Transitions
 * These are custom Velocity.js transitions for when a player hides/unhides 
 * from a livemap
 */
$.Velocity.RegisterUI("transition.livemapBadgeIn", {
    defaultDuration: 3000,
    calls: [
        [ { scaleX: [1, 0.625], scaleY: [1, 0.625], translateZ: 0 }, 0.1, { easing: "spring" } ],
        [ { rotateX: -45 }, 0.25, { easing: "spring" } ],
        [ { opacity: [0.2, 1], rotateX: 1080 }, 0.25, { easing: "spring" } ]
    ]
});

$.Velocity.RegisterUI("transition.livemapBadgeOut", {
    defaultDuration: 3000,
    calls: [
        [ { scaleX: [1, 0.625], scaleY: [1, 0.625], translateZ: 0 }, 0.1, { easing: "spring" } ],
        [ { rotateX: 90 }, 0.25, { easing: "spring" } ],
        [ { opacity: [0.4, 0.2], rotateX: -1080 }, 0.25, { easing: "spring" } ]
    ]
});

/* Active Instances */
var ActiveInstances = [];

/* Refresh Interval */
var RefreshInterval;

/* Refresh Interval */
var LastWorldChatID = 0;

/**
 * This function queries the API for active server data. If any active servers
 * are found, all relevant data is then queried and loaded to the page
 */
function init() {
    $.ajax({
        dataType: "json",
        type: "GET",
        url: "api/livemap.api.php",
        data: {
            filter: "livemap_server"
        },
        success: function(data) {
            LoadLivemaps(data);
        },
        error: function(e) {
            console.log(e);
        }
    });
}

/**
 * Load All Active Livemaps
 * 
 * This function queries the API for servers that have been updated in the
 * past 30 seconds and returns a populated element that is added to the 
 * .livemaps element found in index.html
 * @param {obj} livemap_server 
 */
function LoadLivemaps(livemap_server) {
    // loop through each server instance returned
    jQuery.each(livemap_server, function(i, val) {
        // if instance exists in livemapInstances, load the livemap
        if (DisabledLivemaps.indexOf(val.server_id.toLowerCase()) == -1) {
            // Add this server to the active instance array
            ActiveInstances.push(val.server_id.toLowerCase());
            
            // generate the map
            $.ajax({
                type: "GET",
                url: "templates/layout.livemap.php",
                data: {
                    // send server data
                    ServerID: val.server_id,
                    Map: val.map,
                    PlayersOnline: returnOnlinePlayerCount(val)
                },
                success: function(data) {
                    // add generated map to the page
                    $(".livemaps").append(data);
                    RefreshLivemap(val.server_id);
                },
                error: function(e) {
                    console.log(e);
                }
            });
        }
    })
}

function TriggerLivemapRefresh() {
    for (var i = 0; i < ActiveInstances.length; i++) {
        RefreshLivemap(ActiveInstances[i]);
    }    
}

/**
 * Refresh Livemap
 * 
 * This function sends a request to the API and retrieves all server table data
 * and then updates the livemap data
 * @param {string} server_id 
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
            UpdateOnlinePlayerCount(data.livemap_server[0]);
            UpdatePlayerBadges(data.livemap_data);
            UpdatePlayerNodes(data.livemap_server[0], data.livemap_data);
            UpdateWorldChat(data.livemap_chat);
        },
        error: function(e) {
            console.log("ERROR: " + e.val);
        }
    });
}

function returnPlayerType(player) {
    if (player.is_admin == 1) {
        return "admin";
    } else if (player.is_pro == 1) {
        return "gold";
    } else if (player.in_vehicle == 1) {
        return "vehicle";
    } else {
        return "normal";
    }
}

function returnPlayerBadgeTypeIcon(player) {
    if (player.is_admin == 1) {
        return "admin";
    } else if (player.is_pro == 1) {
        return "gold";
    }
}

function UpdateOnlinePlayerCount(livemap_server) {
    $(".livemap[data-server-id='" + livemap_server.server_id + "'] .livemap-online-players").html("Players: " + livemap_server.online_players + "/" + livemap_server.max_players);
}

function returnOnlinePlayerCount(livemap_server) {
    return livemap_server.online_players + "/" + livemap_server.max_players;
}

function UpdatePlayerNodes(livemap_server,livemap_data) {
    jQuery.each(livemap_data, function(i, val) {
        $.ajax({
            type: "GET",
            url: "templates/node.livemap.php",
            data: {
                // send player data
                CSteamID: val.CSteamID,
                CharacterName: val.character_name,
                Position: returnNodePositionStyle(livemap_server.server_id, livemap_server.map, val.position)
            },
            success: function(data) {
                // process returned data
                $(".livemap[data-server-id='" + val.server_id + "'] .livemap-nodes").append(data);
                $(".livemap-nodes [data-steam-id='" + val.CSteamID + "']").velocity("transition.bounceDownIn", { stagger: 250 });
            },
            error: function(e) {
                console.log(e);
            }
        });
    })
}

function UpdatePlayerBadges(livemap_data) {
    jQuery.each(livemap_data, function(i, val) {
        $.ajax({
            type: "GET",
            url: "templates/badge.livemap.php",
            data: {
                // send player data
                CSteamID: val.CSteamID,
                CharacterName: val.character_name,
                Reputation: returnReputation(val.reputation),
                Avatar: val.steam_avatar_medium,
                BadgeColor: returnPlayerType(val),
                TypeIcon: returnPlayerBadgeTypeIcon(val)
            },
            success: function(data) {
                // process returned data
                $(".livemap[data-server-id='" + val.server_id + "'] .livemap-badges").append(data);
                $(".livemap-badges [data-steam-id='" + val.CSteamID + "']").velocity("transition.flipYIn", { stagger: 300 });
            },
            error: function(e) {
                console.log(e);
            }
        });
    })
}

/**
 * Update World Chat
 * This function updates world chat by sending a request to the API and then
 * appending the chat window with the newest chat data.
 * @param {object} livemap_chat Livemap chat data
 */
function UpdateWorldChat(livemap_chat = null) {
    if (LastWorldChatID > 0) {
        $.ajax({
            dataType: "json",
            type: "GET",
            url: "api/livemap.api.php",
            data: {
                livemap: "wash",
                filter: "livemap_chat"
            },
            success: function(data) {
                jQuery.each(data.livemap_chat, function(i, val) {
                    if (val.id > LastWorldChatID) {
                        var CharacterName = val.character_name;
                        var Avatar = val.steam_avatar_medium;
                        var Message = val.message;
                        var IsAdmin = val.is_admin;

                        // create new message
                        var data = '<div class="media"><div class="media-left"><img class="media-object" src="'+ Avatar +'" alt=""></div><div class="media-body"><p class="'+ (IsAdmin == 1 ? "admin" : "") +'">[World] '+ CharacterName +': '+ Message +'</p></div></div>';
                        $(".livemap[data-server-id='" + val.server_id + "'] .livemap-chat #mCSB_1_container").prepend(data);
                    }                    

                    if (LastWorldChatID < val.id) { LastWorldChatID = val.id; }
                });
            },
            error: function(e) {
                console.log(e);
            }
        });
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
            $(".livemap[data-server-id='" + val.server_id + "'] .livemap-chat").append(data);
            
            if (LastWorldChatID == 0) {
                LastWorldChatID = val.id;
            }            
        });
    }
    
}

/**
 * Convert Vector3 to Pixels
 * 
 * This function converts player vector3 position data into a
 * location on the livemap
 * @param {string} server_id Server identifier
 * @param {string} vector3 Position data: "(x,z,y)"
 */
function CalculateVectorPosition(server_id, map,vector3) {
    // let's pre-define default map dimensions
    // since all maps are square, we can store just one axis
    var monolith = 1024;
    var pei = 2048;
    var russia = 4096;
    var washington = 2048;
    var yukon = 2048;
    var hawaii = 4096;

    /**
     * this offset is needed due to the difference between map
     * dimensions and player constraints. i.e. washington is set to
     * 2048x2048, total = 4096, but map bounds stop players at
     * 1983.5 on all four sides of the map. 1983.5 * 2 = 3967, so...
     * offset calculation (washington): 4096 - (1983.5 * 2) = 129 
     */
    var offset = 129;

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
 * this function returns a css formatted string for a player node
 * position and is applied to 'node.livemap.php'
 * @param string server_id Server identifier
 * @param string Vector3 position data
 */
function returnNodePositionStyle(server_id,map,vector3) {
    // get the player position
    var vector = CalculateVectorPosition(server_id,map,vector3);
    // return styled position data
    return "left:" + vector[0] + "px;bottom:" + vector[1] + "px";
}

function returnReputation(rep) {
    // neutral reputation
    if (rep == 0) {
        return "Neutral [" + rep + "]";
    // positive reputation
    } else if (rep >= 1 && rep <= 7) {
        return "Vigilante [" + rep + "]";
    } else if (rep >= 8 && rep <= 32) {
        return "Constable [" + rep + "]";
    } else if (rep >= 33 && rep <= 99) {
        return "Deputy [" + rep + "]";
    } else if (rep >= 100 && rep <= 199) {
        return "Sheriff [" + rep + "]";
    } else if (rep >= 200) {
        return "Paragon [" + rep + "]";
    // negative reputation
    } else if (rep <= -1 && rep >= -7) {
        return "Thug [" + rep + "]";
    } else if (rep <= -8 && rep >= -32) {
        return "Outlaw [" + rep + "]";
    } else if (rep <= -33 && rep >= -99) {
        return "Gangster [" + rep + "]";
    } else if (rep <= -100 && rep >= -199) {
        return "Bandit [" + rep + "]";
    } else if (rep <= -200) {
        return "Villain [" + rep + "]";
    }
}

// show loading screen during ajax requests
$(document).on({
    ajaxStart: function() { $(".livemap-loading").removeClass("hidden"); },
    ajaxStop: function() { $(".livemap-loading").addClass("hidden"); $(".livemap-chat").mCustomScrollbar({advanced:{
          autoExpandHorizontalScroll:false,
          updateOnContentResize: true
        }}); }
});

$(document).ready(function() {
    init();

    // set refresh interval
    //setInterval(TriggerLivemapRefresh, 15000);
    setInterval(UpdateWorldChat, 10000);
});