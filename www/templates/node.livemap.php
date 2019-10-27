<!-- player node container -->
<div class="livemap-node-container" data-steam-id="<?php echo $_GET["CSteamID"]; ?>" data-name="<?php echo $_GET["CharacterName"]; ?>" data-health="<?php echo $_GET["Health"]; ?>" data-hunger="<?php echo $_GET["Hunger"]; ?>" data-thirst="<?php echo $_GET["Thirst"]; ?>" data-infection="<?php echo $_GET["Infection"]; ?>" data-stamina="<?php echo $_GET["Stamina"]; ?>" style="<?php echo $_GET["Position"]; ?>">  
    <!-- player node -->
    <div class="livemap-node-name"><?php echo $_GET["CharacterName"]; ?></div>
    
    <div class="livemap-node-icon">
        <img class="livemap-node" style="<?php echo $_GET["SkinColor"]; ?>" src="images/nodes/faces/<?php echo $_GET["Face"]; ?>.png" alt="player">
    </div>
</div><!-- /player node container -->