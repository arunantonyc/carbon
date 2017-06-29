/*
Scripts to post current location and retrieve poi info listing 
*/


function getGeocode() {
    var geocoder = new google.maps.Geocoder();
    var address = document.getElementById("Address").value;
    geocoder.geocode({ 'address': address }, function (results, status) {
        if (status === google.maps.GeocoderStatus.OK) {
            var loc = results[0].geometry.location.lat() + ',' + results[0].geometry.location.lng();
            $('#Location').val(loc);
        } else {
            alert('Geocode unsuccessful reason: ' + status);
        }
    });    
}

$(document).ready(function () {        
    $('#getGeocode').click(function () {
        getGeocode();
    });
});

