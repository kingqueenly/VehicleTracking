<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="VT_Web.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <style type="text/css">
        body, html, #allmap
        {
            width: 100%;
            height: 100%;
            overflow: hidden;
            margin: 0;
        }
        #l-map
        {
            height: 100%;
            width: 78%;
            float: left;
            border-right: 2px solid #bcbcbc;
        }
        #r-result
        {
            height: 100%;
            width: 20%;
            float: left;
        }
    </style>
    <script type="text/javascript" src="http://api.map.baidu.com/api?v=1.5&ak=9fb983ecd9b505f8fedcc9ab07c65e3e"></script>
    <script type="text/javascript" src="http://developer.baidu.com/map/jsdemo/demo/convertor.js"></script>
    <title>GPS</title>
    <script src="jquery-1.10.2.js" type="text/javascript"></script>
</head>
<body>
    <div style="height: 40px;">
        <table>
            <tr>
                <td>
                    GPS坐标：
                </td>
                <td>
                    <input type="text" id="zuobiao" />
                </td>
                <td>
                    <input type="button" onclick="locate();" value="Locate" />
                </td>
            </tr>
        </table>
    </div>
    <div id="allmap">
    </div>
</body>
</html>
<script type="text/javascript">

    function locate() {
        var txtZb = jQuery("#zuobiao");
        var parts = txtZb.val().split(" ");
        var vs = parts[0] + parts[1] + parts[2] + parts[3] + parts[4] + parts[5] + parts[6] + parts[7];
        var v1 = vs.substr(0, 3);
        var v11 = vs.substr(3, 2) + "." + vs.substr(5, 3);
        var v2 = vs.substr(8, 2);
        var v21 = vs.substr(10, 2) + "." + vs.substr(12, 4);

        var xx = parseFloat(v1) + parseFloat(v11) / 60;
        var yy = parseFloat(v2) + parseFloat(v21) / 60;

        var gpsPoint = new BMap.Point(xx, yy);

        var bm = new BMap.Map("allmap");
        bm.centerAndZoom(gpsPoint, 15);
        bm.addControl(new BMap.NavigationControl());

        var markergps = new BMap.Marker(gpsPoint);
        bm.addOverlay(markergps);
        var labelgps = new BMap.Label("gps", { offset: new BMap.Size(20, -10) });
        markergps.setLabel(labelgps);


        translateCallback = function (point) {
            var marker = new BMap.Marker(point);
            bm.addOverlay(marker);
            var label = new BMap.Label("baidu", { offset: new BMap.Size(20, -10) });
            marker.setLabel(label);
            bm.setCenter(point);
            //alert("baidu:" + point.lng + "," + point.lat);
        }

        setTimeout(function () {
            BMap.Convertor.translate(gpsPoint, 0, translateCallback);
        }, 2000);
    }
</script>
