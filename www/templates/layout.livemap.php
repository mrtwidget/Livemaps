<!-- .livemap -->
<div class="title-nav">
    <div class="row">
        <div class="col-md-10">
            <h1 class="server-name">Loading, please wait...</h1>
        </div>
        <div class="col-md-2">
            <a class="btn btn-success btn-xs pull-right" href="steam://connect/<?php echo $_GET["ConnectAddress"]; ?>" role="button">Play Now!</a>
        </div>
    </div>
</div>
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
                        <h1>TIMEOUT</h1>
                        <p>The Livemap has timed out. The server may be offline...</p>
                    </div>
                    <div class="livemap-static"></div>
                </div>
                <!-- map image -->
                <img class="img-responsive" src="images/maps/<?php echo strtolower($_GET["Map"]); ?>/Map.jpg" alt="">
                <div class="livemap-time">
                    <img src="images/icons/cycle.png" alt="">
                </div>
                <!-- player nodes -->
                <div class="livemap-nodes"></div>
                <!-- map overlay -->
                <div class="livemap-hud">
                    <div class="livemap-hud-map"><?php echo strtoupper($_GET["Map"]); ?></div>
                    <div class="livemap-hud-online-players"><?php echo $_GET["PlayersOnline"]; ?></div>
                </div>
                <!-- world chat -->
                <div class="livemap-chat"></div>
                <!-- Map labels -->
                <div class="livemap-labels"></div>
                <!-- Info Bars -->
                <div class="node-info">
                    <p class="name"></p>
                    <img src="images/icons/health.png" alt="">
                    <div class="progress health">                        
                        <div class="progress-bar progress-bar-danger" role="progressbar" aria-valuenow="40" aria-valuemin="0" aria-valuemax="100" style="width: 0%"></div>
                    </div>
                    <img src="images/icons/hunger.png" alt="">
                    <div class="progress hunger">
                        <div class="progress-bar progress-bar-warning" role="progressbar" aria-valuenow="40" aria-valuemin="0" aria-valuemax="100" style="width: 0%"></div>
                    </div>
                    <img src="images/icons/thirst.png" alt="">
                    <div class="progress thirst">
                        <div class="progress-bar progress-bar-info" role="progressbar" aria-valuenow="40" aria-valuemin="0" aria-valuemax="100" style="width: 0%"></div>
                    </div>
                    <img src="images/icons/infection.png" alt="">
                    <div class="progress infection">
                        <div class="progress-bar progress-bar-success" role="progressbar" aria-valuenow="40" aria-valuemin="0" aria-valuemax="100" style="width: 0%"></div>
                    </div>
                    <img src="images/icons/stamina.png" alt="">
                    <div class="progress stamina">
                        <div class="progress-bar progress-bar-yellow" role="progressbar" aria-valuenow="40" aria-valuemin="0" aria-valuemax="100" style="width: 0%"></div>
                    </div>
                </div>
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