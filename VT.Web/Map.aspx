<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Map.aspx.cs" Inherits="VT_Web.Map" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=gb2312" />
    <style type="text/css">
        body, html, #container
        {
            width: 100%;
            height: 100%;
            overflow: hidden;
            margin: 0;
        }
    </style>
    <title>有方向的折线覆盖物</title>
    <script type="text/javascript" src="http://api.map.baidu.com/api?v=1.3"></script>
    <script src="jquery-1.10.2.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div id="container">
    </div>
    <script type="text/javascript">
        var map = new BMap.Map("container");
        var point = new BMap.Point(121.535615, 31.276752);
        map.centerAndZoom(point, 17);
        var points = [
	  new BMap.Point(121.5356439, 31.2810635),
	  new BMap.Point(121.537615, 31.278752),
	  new BMap.Point(121.535615, 31.276752),
	  new BMap.Point(121.536615, 31.274752),
	  new BMap.Point(121.533615, 31.277752)
	];

        var polyline = new BMap.Polyline(points, { strokeColor: "blue", strokeWeight: 5, strokeOpacity: 0.8 });
        map.addOverlay(polyline);

        for (var i = points.length - 1; i > 0; i--) {
            var angle = getAngle(points[i], points[i - 1]);
            drawMarker(points[i], angle);
        }

        //return: -PI to PI
        function getAngle(pt1, pt2) {
            return Math.atan2(pt2.lat - pt1.lat, pt2.lng - pt1.lng);
        }

        function drawMarker(point, angle) {
            var iconImg = createIcon(angle);
            var marker = new BMap.Marker(point, {
                icon: iconImg
            });

            map.addOverlay(marker);
        }

        function createIcon(angle) {
            //从负Y轴方向开始顺时针查找角度
            var adjAngles = [180, 202, 225, 247, 270, 292, 315, 337, 0, 22, 45, 67, 90, 112, 135, 157];
            adjAngle = angle;

            var adjIndex = 0;
            for (var i = 0; i < 16; i++) {
                if (adjAngle < (-15 / 16 + i / 8) * Math.PI) {
                    adjIndex = i;
                    break;
                }
            }

            icon = new BMap.Icon("images/direction/arrow_" + adjAngles[adjIndex] + ".png", new BMap.Size(22, 22));
            return icon;
        }
</script>
    </form>
</body>
</html>
