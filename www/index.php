<!DOCTYPE html>
<html>
    <head>
        <title>Livemap v0.2</title>
        <meta name="viewport" content="width=device-width, initial-scale=1">
        <meta name="theme-color" content="#942a30">
        <link rel="icon" sizes="192x192" href="images/favicon.png">
        <link href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css" rel="stylesheet">
        <link href="https://fonts.googleapis.com/css?family=Gloria+Hallelujah" rel="stylesheet">
        <link href="css/livemap.css" rel="stylesheet">
        <script src="https://code.jquery.com/jquery-3.2.1.min.js" integrity="sha256-hwg4gsxgFZhOsEEamdOYGBf13FyQuiTwlAQgxVSNgt4=" crossorigin="anonymous"></script>
        <script src="js/jquery-ui.min.js"></script>
        <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js"></script>
        <script src="js/jquery-ui.min.js"></script>
        <script src="js/velocity.min.js"></script>
        <script src="js/velocity.ui.js"></script>
        <script src="js/livemap.js"></script>
        <script><?php echo(isset($_GET['id']) ? "init('". $_GET['id'] ."');" : "returnServerList();"); ?></script>
    </head>

    <body>
        <video loop muted autoplay poster="images/fbg.jpg" class="fullscreen-bg__video">
            <source src="images/bg.mp4" type="video/mp4"> 
        </video>

        <div class="author"><small>Livemaps v0.2 by <a href="mailto:nexis@nexisrealms.com">Nexis</a></small></div>

        <div class="container">
            <h2>Livemap Plugin <small>v0.2</small></h2>
            <div class="livemaps"></div>
        </div>
    </body>
</html>