/* 
 Script files to display google map
*/

var timeInt = 30000;
var zoomLvl = 11;
var cirRadius = 100;

var watchId;
var currPos;
var currCircle;
var currMarker;
var dispMap;
var ctr;

function initialize() {
    var mapOptions = {
        zoom: zoomLvl,
        center: new google.maps.LatLng(12.976750, 77.601420), // just a starting point to start
        mapTypeId: google.maps.MapTypeId.ROADMAP,
        mapTypeControl: false
    };
    dispMap = new google.maps.Map(document.getElementById("map"), mapOptions);
    currMarker = new google.maps.Marker({        
        map: dispMap,
        title: 'You'
    });
    currCircle = new google.maps.Circle({
        strokeColor: '#5F9EA0',
        strokeOpacity: 0.7,
        strokeWeight: 1,
        fillColor: '#B0E0E6',
        fillOpacity: 0.3,
        map: dispMap
    });
    getCurrentLocation();
    watchId = setInterval(getCurrentLocation, timeInt);
    ctr = 1;
}
function getCurrentLocation() {
    if (navigator.geolocation) {
        $('#CurrLoc').html('Acquiring...');

        var options = { maximumAge:60000 };
        navigator.geolocation.getCurrentPosition(gotLocation, showError, options);
        ctr++;
    }    
    else {
        shutdown();
        alert("Not supported!");
    }
}

function gotLocation(position)
{
    currPos = position;
    //dispMap.setZoom(16);
        
    showPosition(position, true, false);
}

function showPosition(position, showCircle, showAddress) {
    cirRadius = position.coords.accuracy.valueOf();
    $('#CurrLoc').html(position.coords.latitude.toFixed(3) + ', ' + position.coords.longitude.toFixed(3) + '  ~' + cirRadius.toFixed(0) + ' m');
    var latLng = new google.maps.LatLng(position.coords.latitude, position.coords.longitude);
    
    // show location marker    
    currMarker.setPosition(latLng);
    currMarker.setTitle('You : ' + ctr);
    // proximity circle
    if (showCircle) {        
        currCircle.setCenter(latLng);
        currCircle.setRadius(cirRadius);
    }
    // rev geo-address
    if (showAddress) {
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
        infowindow.open(map, marker);
    }    
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

function shutdown(){
    if (watchId){
        clearInterval(watchId);
    }
}

window.onload = initialize;
window.onunload = shutdown;