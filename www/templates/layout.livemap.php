<!-- .livemap -->
<div class="livemap" data-server-id="<?php echo $_GET["ServerID"]; ?>">
    <div class="row">
        <div class="col-lg-8 col-md-9 col-xs-12">
            <div class="livemap-map">
                <!-- loading screen -->
                <div class="livemap-loading">
                    <div class="livemap-loading-container">
                        <img src="images/favicon.png" alt="loading">
                        <p>loading</p>
                    </div>
                </div>
                <!-- map image -->
                <img class="img-responsive" src="images/maps/<?php echo $_GET["Map"]; ?>/Map.jpg" alt="">
                <!-- player nodes -->
                <div class="livemap-nodes"></div>
                <!-- map overlay -->
                <div class="livemap-hud">
                    <div class="livemap-hud-map"><?php echo strtoupper($_GET["Map"]); ?></div>
                    <div class="livemap-hud-online"><?php echo $_GET["PlayersOnline"]; ?></div>
                </div>
            </div>
            <!-- world chat -->
            <div class="livemap-chat mCustomScrollbar" data-mcs-theme="light"></div>            
        </div>
        <div class="col-lg-4 col-md-3 hidden-xs" style="padding-left:0">
            <div class="livemap-badges">
                <div class="livemap-container"><img src="images/icons/players-online.png" alt=""> <span class="livemap-online-players">Players: 0/0</span></div>
                
            </div>
        </div>
    </div><!-- /.row -->
</div><!-- /.livemap -->