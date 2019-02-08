let html =
    """
    <!DOCTYPE HTML>
    <html>
    <head>
        <meta charset="utf-8" />
        <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no" />
        <meta http-equiv="x-ua-compatible" content="ie=edge" />
        <title>NBomber</title>
        <link rel="stylesheet" href="assets/css/fontawesome.css" />
        <link rel="stylesheet" href="assets/css/bootstrap.css" />
        <link rel="stylesheet" href="assets/css/sidebar.css" />
        <link rel="stylesheet" href="assets/css/style.css" />
    </head>
    <body>
        <!-- Navbar -->
        <nav class="navbar navbar-dark">
            <a class="navbar-brand" href="#">
                <img src="assets/img/logo_300x100.png" height="50" class="d-inline-block align-top" alt="" />
            </a>
        </nav>
        <!-- Navbar -->

        <!-- Sidebar -->
        <div id="wrapper" class="active">
            <div id="sidebar-wrapper">

        <ul id="sidebar_menu" class="sidebar-nav">
            <li class="sidebar-brand"><a id="menu-toggle" href="#">Menu<span class="main_icon fa fa-align-justify"></span></a></li>
        </ul>

        <ul class="sidebar-nav nav nav-pills mb-3" role="tablist">
            <li>
        <a data-toggle="pill" href="#env-view" role="tab" aria-controls="env-view">Environment <span class="sub_icon fas fa-flask"></span></a>
    </li><li>
        <a data-toggle="pill" href="#global-view" role="tab" aria-controls="global-view">Global <span class="sub_icon fas fa-globe"></span></a>
    </li>
        </ul>

    </div>
        </div>
        <!-- Sidebar -->

        <!-- ContentView -->
        <div id="page-content-wrapper">
            <div class="container">
                <div class="tab-content" id="pills-tabContent">
                    <div class="tab-pane fade" id="env-view" role="tabpanel">

        <div class="row">
            <div class="col">
                <div class="card custom">
        <div class="card-header custom">Cluster info</div>
        <div class="card-body">
            <table class="table table-hover table-sm table-responsive-lg">
                <thead>
                    <tr>
                        <th>machine name</th>
                        <!--<th>NBomber</th>-->
                        <!--<th>local ip</th>-->
                        <th>OS</th>
                        <th>.NET runtime</th>
                        <th>processor</th>
                        <th>cores</th>
                        <!--<th>RAM</th>-->
                    </tr>
                </thead>
                <tbody>
                    <tr><td>IFM10N12257</td><td>Microsoft Windows NT 6.2.9200.0</td><td>.NETCoreApp,Version=v2.2</td><td>Intel64 Family 6 Model 142 Stepping 10, GenuineIntel</td><td>8</td></tr>
                </tbody>
            </table>
        </div>
    </div>
            </div>
        </div>

    </div><div class="tab-pane fade show active" id="global-view" role="tabpanel">

        <div class="row">
            <div class="col">
                <div class="card custom scenario" style="margin-bottom: 20px">
        <div class="card-header custom">Statistics for Scenario: <b>alarms</b>, Duration: <b>00:00:10</b>, RPS: <b>0</b>, Concurrent Copies: <b>1</b></div>
        <div class="card-body">

            <table class="table table-hover table-sm table-responsive-lg">
                <thead>
                    <tr>
                        <th></th>
                        <th colspan="3">Request count</th>
                        <th colspan="4">Request time</th>
                        <th colspan="4">Request time percentile</th>
                        <th colspan="4">Data transfer</th>
                    </tr>
                    <tr>
                        <th>step</th>
                        <th>All</th>
                        <th>OK</th>
                        <th>Failed</th>
                        <th>RPS</th>
                        <th>min</th>
                        <th>mean</th>
                        <th>max</th>
                        <th>50%</th>
                        <th>75%</th>
                        <th>95%</th>
                        <th>StdDev</th>
                        <th>min, Kb</th>
                        <th>mean, Kb</th>
                        <th>max, Kb</th>
                        <th>all, MB</th>
                    </tr>
                </thead>
                <tbody>
                    <tr><td>Create & close alarm HTTP</td>
                        <td>58</td>
                        <td>0</td>
                        <td>58</td>
                        <td>0</td>
                        <td>0</td>
                        <td>0</td>
                        <td>0</td>
                        <td>0</td>
                        <td>0</td>
                        <td>0</td>
                        <td>0</td>
                        <td>-</td>
                        <td>-</td>
                        <td>-</td>
                        <td>-</td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
            </div>
        </div>

        <div class="row" style="margin-top: 20px">
            <div class="col-7">
                <div class="card custom">
        <div class="card-header custom">Indicators</div>
        <div class="card-body">
            <canvas id="indicators_chart-global-view" height="100"></canvas>
        </div>
    </div>
            </div>
            <div class="col-5">
                <div class="card custom">
        <div class="card-header custom">Number of requests</div>
        <div class="card-body">
            <canvas id="num_req_chart-global-view" height="150"></canvas>
        </div>
    </div>
            </div>
        </div>

    </div>

                </div>
            </div>
        </div>
        <!-- ContentView -->

        <script type="text/javascript" src="assets/js/jquery-3.3.1.js"></script>
        <script type="text/javascript" src="assets/js/bootstrap.bundle.js"></script>
        <script type="text/javascript" src="assets/js/nbomber.js"></script>
        <script type="text/javascript" src="assets/js/chart.bundle.js"></script>

        <script type="text/javascript">
            %js%
        </script>
    </body>
    </html>    """.Trim()

let unencoded = "Create & close alarm HTTP"
let title = System.Web.HttpUtility.HtmlEncode(unencoded)
let s : string = null
let ss = sprintf "<tr>%s</tr>" s