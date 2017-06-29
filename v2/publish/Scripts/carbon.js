// Cookie funcs
function createCookie(name, value, mins) {
    var expires;

    if (mins) {
        var date = new Date();
        date.setTime(date.getTime() + (mins * 60 * 1000)); //days * 24 * 60
        expires = "; expires=" + date.toGMTString();
    } else {
        expires = "";
    }
    document.cookie = encodeURIComponent(name) + "=" + encodeURIComponent(value) + expires + "; path=/";
}
function readCookie(name) {
    var nameEQ = encodeURIComponent(name) + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) === ' ') c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) === 0) return decodeURIComponent(c.substring(nameEQ.length, c.length));
    }
    return null;
}
function eraseCookie(name) {
    createCookie(name, "", -1);
}
// ----------


function activeNavbarLink() {
    var linkName = window.location.pathname;
    var lastSlash = linkName.lastIndexOf("/");
    linkName = linkName.substr(lastSlash);
    if (linkName == "/") { linkName = "/Home"; $('#partialUserView').hide();}
    $(".navbar .navbar-nav > li").each(function () {
        if ($(this).text() == "")
            return true;
        classie.remove(this, "active");
        if (linkName == "/" + $(this).text()) { // software is the name of the page/slug
            classie.add(this, "active");
        }
    });
}

// Login / Register funcs
function popLogin(delay) {
    var loginKey = readCookie("fms");
    if (loginKey == null) {
        $("#partialLoginCaret").hide();
        //$("#partialUserOptions").hide();
        $("#modal-loginKey").val("");
        $("#modal-loginPwd").val("");
        $("#modal-newloginKey").val("");
        $("#modal-newloginPwd").val("");
        $("#modal-newloginName").val("");
        setTimeout(function () { $('#loginModal').modal('show'); }, delay);
    }
    else {
        var cookieVal = loginKey.split("-");
        showCurrentLogin(cookieVal[0], cookieVal[1]);
    }
}
function showCurrentLogin(key, name) {
    $("#partialLoginName").html(name);
    $("#partialLoginKey").html("(" + key + ")");
    $("#partialLoginCaret").show();
}
function loginUser(key, password) {
    var src = {
        'k': key,
        'p': password
    }
    $.ajax({
        type: 'POST',
        contentType: 'application/json',
        async: true,
        //dataType: "text",
        url: '../api/Login',
        data: JSON.stringify(src),
        success: function (reply) {            
            var data = JSON.parse(reply);
            if (data.Rst == 1) {
                alert("Welcome " + data.Name);
                createCookie("fms", data.Key + "-" + data.Name, 5);
                showCurrentLogin(data.Key, data.Name)
            }
            else {
                alert("Login failed, try again !");
            }
        },
        error: function (error) {
            jsonValue = jQuery.parseJSON(error.responseText);
        }
    });
    $('#loginModal').modal('hide');

}
function logoutUser() {
    eraseCookie("fms");
    //$("#partialUserOptions").hide();
    $("#partialLoginName").html("Login");
    $("#partialLoginKey").html("");
    alert("You have logged out.");
}
function registerUser(key, password, name) {
    var src = {
        'k': key,
        'p': password,
        'n': name
    }
    //alert(key + password + name);
    $.ajax({
        type: 'PUT',
        contentType: 'application/json',
        async: true,
        //dataType: "text",
        url: '../api/Login',
        data: JSON.stringify(src),
        success: function (reply) {
            var data = JSON.parse(reply);
            if (data.Rst == 1) {
                alert("Welcome " + data.Name);
                createCookie("fms", data.Key + "-" + data.Name, 5);
                showCurrentLogin(data.Key, data.Name)
            }
            else {
                alert("Registeration failed, try again !");
            }
        },
        error: function (error) {
            jsonValue = jQuery.parseJSON(error.responseText);
        }
    });
    $('#loginModal').modal('hide');
}
// -----------------

$(document).ready(function () {
    activeNavbarLink();
    
    // user auth
    $("#partialLoginCaret").hide();
    popLogin(3000);

    $('body').on('click', '#modal-loginSubmit', function () {
        loginUser($("#modal-loginKey").val(), $("#modal-loginPwd").val());
    });

    $('body').on('click', '#modal-registerSubmit', function () {
        registerUser($("#modal-newloginKey").val(), $("#modal-newloginPwd").val(), $("#modal-newloginName").val());
    });

    $('body').on('click', '#logoutSubmit', function () {
        logoutUser();
    });

    $('body').on('click', '#partialLogin', function () {
        if ($('#partialLoginName').text() == "Login")
            popLogin(0);
    });

    // location
    $('#status').html("Loaded.");

    initializeMap();
});