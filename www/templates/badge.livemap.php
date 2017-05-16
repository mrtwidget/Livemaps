<!-- player badge container -->
<div class="livemap-badge-container" data-steam-id="<?php echo $_GET["CSteamID"]; ?>">  
    <!-- player badge -->
    <div class="livemap-badge-player">
        <div class="row">
            <div class="col-md-3 col-lg-4">
                <img src="<?php echo $_GET["Avatar"]; ?>" alt="avatar">
                <img class="hidden-md" src="images/icons/reputations/neutral.png" alt="reputation-icon">
            </div>
            <div class="col-md-7 col-lg-6">
                <div class="player-name <?php echo $_GET["BadgeColor"]; ?>"><?php echo $_GET["CharacterName"]; ?></div>
                <div class="player-reputation neutral"><?php echo $_GET["Reputation"]; ?></div>
            </div>
            <div class="col-md-2">
                <img class="livemap-permission" src="images/icons/admin.png" alt="permissions">
                <img class="livemap-skillset hidden" src="images/icons/skillsets/thief.png" alt="skillset">
            </div>
        </div>
    </div><!-- /player badge -->
</div><!-- /player badge container -->