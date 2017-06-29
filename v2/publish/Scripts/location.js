﻿/*
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

function initializeMap() {

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

    currRangeGrid = new google.maps.Polygon({
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


    getMyLocation();
    watchId = setInterval(getMyLocation, timeInt);
    ctr = 1;
}

function getMyLocation() {
    if (navigator.geolocation) {
        $('#status').html('Acquiring...');

        var options = { maximumAge: 60000 };
        navigator.geolocation.getCurrentPosition(foundMyLocation, showError, options);

        //if (ctr > 2)
        //    sendCurrLoc();
        ctr++;
    }
    else {
        shutdown();
        alert("Not supported!");
    }
}

function foundMyLocation(position) {
    currPos = position;
    showPosition(currPos.coords.latitude, currPos.coords.longitude, currPos.coords.accuracy, true, false, false);
    sendMyLoc(currPos.coords.latitude, currPos.coords.longitude, currPos.coords.accuracy)
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
                infowindow.setContent("&#9993; " + results[0].formatted_address);
            } else { }
        } else { }
    });
    infowindow.open(dispMap, marker);
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

function sendMyLoc(lat, lng, accuracy) {
    $('#feed').html("......");
    var cookieVal = readCookie("fms");
    var cookieKey = "";
    if (cookieVal != null) {
        var cookieSplit = cookieVal.split("-");
        cookieKey = cookieSplit[0];
    }
    
    //if (currPos == null) { return; }
    var src = {
        'x': lat.toFixed(3),
        'y': lng.toFixed(3),
        'r': accuracy.toFixed(0),
        'k': cookieKey
    }
    //alert("sending");
    $.ajax({
        type: 'PUT',
        contentType: 'application/json',
        async: true,
        //dataType: "text",
        url: '../api/Poi',
        data: JSON.stringify(src),
        success: function (reply) {

            //if (reply.val() != '') {
            var data = JSON.parse(reply);
            //displayPoi(data, "ok");
            //alert("Data: " + data + "\nStatus: " + status);
            //}            
        },
        error: function (error) {
            jsonValue = jQuery.parseJSON(error.responseText);
        }
    });
}

function stopMyLocation() {
    if (watchId) {
        clearInterval(watchId);
    }
}