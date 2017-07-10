<!-- .livemap -->
<div class="livemap" data-server-id="<?php echo $_GET["ServerID"]; ?>">
    <div class="row">
        <div class="col-lg-8 col-md-9 col-xs-12">
            <div class="livemap-map">
                <!-- loading screen -->
                <div class="livemap-loading">
                    <div class="livemap-loading-container">
                        <img class="heartbeat" src="images/icons/heart/10.png" alt="heart">
                    </div>
                </div>
                <div class="livemap-static-container hidden">
                    <div class="livemap-static-header">
                        <h1>Are you still there?</h1>
                        <p>The Livemaps fell asleep! Refresh this page to continue.</p>
                    </div>
                    <div class="livemap-static"></div>
                </div>
                <!-- map image -->
                <img class="img-responsive" src="images/maps/<?php echo strtolower($_GET["Map"]); ?>/Map.jpg" alt="">
                <!-- player nodes -->
                <div class="livemap-nodes"></div>
                <!-- map overlay -->
                <div class="livemap-hud">
                    <div class="livemap-hud-map"><?php echo strtoupper($_GET["Map"]); ?></div>
                    <div class="livemap-hud-online-players"><?php echo $_GET["PlayersOnline"]; ?></div>
                </div>
                <!-- world chat -->
            <div class="livemap-chat"></div>
            </div>            
        </div>
        <div class="col-lg-4 col-md-3 hidden-xs" style="padding-left:0">
            <div class="livemap-badges">
                <div class="livemap-online-players">
                    <div class="livemap-badge-player">
                        <img src="images/icons/players-online.png" alt=""> 
                        <span class="livemap-badges-online-players">Players: <span class="player-count">0/0</span></span>
                    </div>
                </div>
                <div class="livemap-badges-container"></div>
            </div>
        </div>
    </div><!-- /.row -->
</div><!-- /.livemap -->