function RefreshLivemap(server_id) {
    $.ajax({
        type: "GET",
        url: "api/livemap.api.php",
        data: {
            // send `server_id` request to API, get all data
            livemap: server_id
        },
        success: function(data) {
            console.log(data);
        },
        error: function(e) {
            console.log("RefreshLivemap() ERROR: " . e);
        }
    });
}

$(document).on({
    ajaxStart: function() { $(".livemap-loading").removeClass("hidden"); },
    ajaxStop: function() { $(".livemap-loading").addClass("hidden"); }
});

$(document).ready(function() {
    RefreshLivemap("hawaii");
    
    // hide loading screen when clicked [TEMP DEV]
    $(".livemap-loading").click(function() {
        $(".livemap-loading").velocity("transition.bounceOut");
    });
});