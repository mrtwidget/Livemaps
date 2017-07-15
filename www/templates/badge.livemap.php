<!-- player badge container -->
<div class="livemap-badge-container" data-steam-id="<?php echo $_GET["CSteamID"]; ?>">  
    <!-- player badge -->
    <div class="livemap-badge-player">
        <div class="row">
            <div class="col-md-3 col-lg-4">
                <img src="<?php echo $_GET["Avatar"]; ?>" alt="avatar">
                <img class="hidden-md" src="images/icons/reputations/<?php echo $_GET["ReputationName"]; ?>.png" alt="reputation-icon">
            </div>
            <div class="col-md-7 col-lg-6">
                <div class="player-name <?php echo $_GET["BadgeColor"]; ?>"><?php echo $_GET["CharacterName"]; ?></div>
                <div class="player-reputation <?php echo $_GET["ReputationName"]; ?>"><?php echo $_GET["Reputation"]; ?></div>
            </div>
            <div class="col-md-2">
                <?php if (isset($_GET["TypeIcon"])) { ?>
                    <img class="livemap-permission" src="images/icons/<?php echo $_GET["TypeIcon"]; ?>.png" alt="permissions">
                <?php } ?>
                <img class="livemap-skillset hidden" src="images/icons/skillsets/thief.png" alt="skillset">
            </div>
        </div>
    </div><!-- /player badge -->
</div><!-- /player badge container -->