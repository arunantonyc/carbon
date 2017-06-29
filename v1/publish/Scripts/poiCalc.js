/*
Scripts to post current location and retrieve poi info listing 
*/

var timeInt = 30000;
var zoomLvl = 14;
var cirRadius = 100;

var watchId;
var currPos;
var currCircle;
var currMarker;
var currGrid;
var currRangeGrid;
var dispMap;
var ctr;
var imgMrkB;

function initialize() {
    
    var mapOptions = {
        zoom: zoomLvl,
        center: new google.maps.LatLng(12.976750, 77.601420), // just a starting point to start
        mapTypeId: google.maps.MapTypeId.ROADMAP,
        mapTypeControl: false,
        streetViewControl: false
    };
    dispMap = new google.maps.Map(document.getElementById('map'), mapOptions);
    
    currGrid = new google.maps.Polygon({
        strokeColor: '#FF6347',
        strokeOpacity: 0.7,
        strokeWeight: 1,
        fillColor: '#FAA460',
        fillOpacity: 0.2,
        map: dispMap
    });

    currRangeGrid =  new google.maps.Polygon({
        strokeColor: '#32CD32',
        strokeOpacity: 0.7,
        strokeWeight: 1,
        fillColor: '#B0E0E6',
        fillOpacity: 0.0,
        map: dispMap
    });

    
    currCircle = new google.maps.Circle({
        strokeColor: '#5F9EA0',
        strokeOpacity: 0.7,
        strokeWeight: 1,
        fillColor: '#B0E0E6',
        fillOpacity: 0.3,
        map: dispMap
    });

    currMarker = new google.maps.Marker({
        map: dispMap,
        title: 'You'
    });

    
    getCurrentLocation();
    watchId = setInterval(getCurrentLocation, timeInt);
    ctr = 1;
}
function getCurrentLocation() {
    if (navigator.geolocation) {
        $('#status').html('Acquiring...');

        var options = { maximumAge: 60000 };
        navigator.geolocation.getCurrentPosition(gotLocation, showError, options);

        //if (ctr > 2)
        //    sendCurrLoc();
        ctr++;
    }
    else {
        shutdown();
        alert("Not supported!");
    }
}

function gotLocation(position) {
    currPos = position;

    showPosition(currPos.coords.latitude, currPos.coords.longitude, currPos.coords.accuracy, true, false, false);
    //showPosition(12.960218, 77.644960, 100, true, true, false);
        
    sendCurrLoc(currPos.coords.latitude, currPos.coords.longitude, currPos.coords.accuracy)
    //sendCurrLoc(12.960218, 77.644960, 100);
}

function showPosition(lat, lng, accuracy, showCircle, showGrid, showAddress) {
    cirRadius = accuracy
    $('#status').html(lat.toFixed(3) + ', ' + lng.toFixed(3) + '  ~' + cirRadius.toFixed(0) + ' m');
    var latLng = new google.maps.LatLng(lat, lng);

    dispMap.setCenter(latLng);
    //setMapCenter(latLng, 200, 1);

    // show location marker    
    currMarker.setPosition(latLng);
    currMarker.setTitle('You : ' + ctr);
    // show proximity circle
    if (showCircle) {
        currCircle.setCenter(latLng);
        currCircle.setRadius(cirRadius);
    }
    // show grid
    if (showGrid) {
        currGrid.setPaths(showGridBox(lat, lng, 0.01));                       // CENTER GRID
        currRangeGrid.setPaths(showGridBox(lat - 0.01, lng - 0.01, 0.03));  // OUTER GRID
    }
    // rev geo-address
    if (showAddress) { showReverseGeocode(latLng, currMarker); }
}

function setMapCenter(latLng, offsetx, offsety) {
    var point1 = dispMap.getProjection().fromLatLngToPoint(
        (latLng instanceof google.maps.LatLng) ? latLng : dispMap.getCenter()
    );
    alert("P1:" + point1);
    var point2 = new google.maps.Point(
        ((typeof (offsetx) == 'number' ? offsetx : 0) / Math.pow(2, dispMap.getZoom())) || 0,
        ((typeof (offsety) == 'number' ? offsety : 0) / Math.pow(2, dispMap.getZoom())) || 0
    );
    alert("P2:" + point1);
    dispMap.setCenter(dispMap.getProjection().fromPointToLatLng(new google.maps.Point(
        point1.x - point2.x,
        point1.y + point2.y
    )));
}

function showError(error) {
    var ele = document.getElementById("map");
    if (error.code == 1) {
        ele.innerHTML = "User denied the request for Geolocation."
    }
    else if (err.code == 2) {
        ele.innerHTML = "Location information is unavailable."
    }
    else if (err.code == 3) {
        ele.innerHTML = "The request to get user location timed out."
    }
    else {
        ele.innerHTML = "An unknown error occurred."
    }
}

function shutdown() {
    if (watchId) {
        clearInterval(watchId);
    }
}
function showGridBox(lat, lng, precision) {
    var lati = parseFloat(Math.floor(lat * 100) / 100);
    var longi = parseFloat(Math.floor(lng * 100) / 100);
    //alert(lati +','+ longi);
    var gridCoords = [{ lat: lati, lng: longi },
                      { lat: lati + precision, lng: longi },
                      { lat: lati + precision, lng: longi + precision },
                      { lat: lati, lng: longi + precision },
                      { lat: lati, lng: longi }
    ];
    return gridCoords;
}
function showReverseGeocode(latLng, marker) {
    var geocoder = new google.maps.Geocoder();
    var infowindow = new google.maps.InfoWindow;
    geocoder.geocode({ 'location': latLng }, function (results, status) {
        if (status == google.maps.GeocoderStatus.OK) {
            if (results[0]) {
                //map.setZoom(13);
                infowindow.setContent(results[0].formatted_address);
            } else { }
        } else { }
    });
    infowindow.open(dispMap, marker);
}

function sendCurrLoc(lat, lng, accuracy) {
    $('#feed').html("......");
    //if (currPos == null) { return; }
    var src = {
        'x': lat.toFixed(3),
        'y': lng.toFixed(3),
        'r': accuracy.toFixed(0)
    }

    $.ajax({
        type: 'POST',
        contentType: 'application/json',
        async: true,
        //dataType: "text",
        url: '../api/Poi',
        data: JSON.stringify(src),
        success: function (reply) {
            
            //if (reply.val() != '') {
            var data = JSON.parse(reply);
            displayPoi(data, "ok");
                //alert("Data: " + data + "\nStatus: " + status);
            //}            
        },
        error: function (error) {
            jsonValue = jQuery.parseJSON(error.responseText);
        }
    });
}

var storeMrkrs = [];
var clrs = ["0000FF", "FF00FF", "00FF00", "8FBC8F", "FF0000", "FFA500", "DB7093"];
var clrIndx = 0;
function displayPoi(data, status) {
    var row = "";
    $('#feed').html(".");
    if (storeMrkrs.length > 0) {
        $.each(storeMrkrs, function (i, val) {
            val.setMap(null);
        });
        storeMrkrs = [];
    }
    var storeCtr = 0;
    clrIndx = 0; 
    $.each(data, function (index, item) {
        // get a color
        var pinColor = clrs[clrIndx];
        clrIndx++;
        if (clrIndx > (clrs.length - 1))
            clrIndx = 0;
        // set row
        //data-toggle='modal' data-target='#poiDetail'
        row += "<tr id='" + item.I + "' class='poiItem' ><td  style='color:#" + pinColor + "; width:120px' data-address='"+ item.A +"'>" + item.N + "</td><td style='text-align:center'>" + item.T + " </td><td style='text-align:center'>" + item.D + " </td></tr>";
        // set marker icon
        var pinImage = new google.maps.MarkerImage("http://www.googlemapsmarkers.com/v1/" + pinColor + "/");
        // show marker
        var storeMarker = new google.maps.Marker({
            position: { lat: item.X, lng: item.Y },
            map: dispMap,            
            draggable: false,
            //label: item.N,
            icon: pinImage
        });
        storeMrkrs.push(storeMarker);
        storeCtr++;
        
    });
    $('#poiList').html(row);
    $('#poiCtr').html(storeCtr);
    //if (storeCtr > 0)
    //    $('#poiPanelBody').addClass("in");
}

function poiClick(e) {
    //alert(e.attr('id'));
    $('#storeName').html(e.find("td:eq(0)").text());
    $('#trvlTime').html(e.find("td:eq(1)").text());
    $('#trvlDist').html(e.find("td:eq(2)").text());
    $('#poiAddrs').html(e.find("td:eq(0)").data('address'));
    
    $('#poiDetail').modal('show');
}

$(document).ready(function () {
    $('#status').html("Loaded.");
    initialize();
    
    $('#getInfo').click(function () {
        sendCurrLoc(currPos.coords.latitude, currPos.coords.longitude, currPos.coords.accuracy);
    });

    //$('#poiDetail').on('shown.bs.modal', function (e) {
    //    alert(e.innerHTML);
        
    //    $('#brandName').html("done");
    //})

    $('body').on('click', '.poiItem', function () {
        poiClick($(this));
        
    });
   
});

