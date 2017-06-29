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
        //$("#partialLoginCaret").hide();
        //$("#partialUserOptions").hide();
        $("#loginLink").show();
        $("#addLink").hide();
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
    $("#partialLogin").show();
    $("#loginLink").hide();
    $("#addLink").show();
    //$("#partialLoginCaret").show();
    //$("#partialUserOptions").show();
    //$("#partialUserOptions").collapse();
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
                //alert("Welcome " + data.Name);
                createCookie("fms", data.Key + "-" + data.Name, 15);
                showCurrentLogin(data.Key, data.Name);
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
    $("#partialLogin").hide();
    $("#loginLink").show();
    clearUserLinks();
    $("#addLink").hide();
    //$("#partialUserOptions").hide();
    //$("#partialLoginName").html("Login");
    //$("#partialLoginKey").html("");
        
    //alert("You have logged out.");
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
                //alert("Welcome " + data.Name);
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

function addLink() {
    var src = {
        'Key': $("#addLinkKey").val(),
        'Status': 1
    }
    $.ajax({
        type: 'PUT',
        contentType: 'application/json',
        async: true,
        //dataType: "text",
        url: '../api/Link',
        data: JSON.stringify(src),
        success: function (reply) {
            var data = JSON.parse(reply);
            if (data.Rst == 1) {
                alert("Link added!");
                getLinks();
            }
            else {
                alert("Link addition failed!");
            }
        },
        error: function (error) {
            jsonValue = jQuery.parseJSON(error.responseText);
        }
    });
    $('#modal-linkAdd').modal('hide');
}

function getLinks() {
    var loginKey = readCookie("fms");
    if (loginKey != null) {
        var cookieVal = loginKey.split("-");
        var src = {
            'k': cookieVal[0]
        }
        $.ajax({
            type: 'POST',
            contentType: 'application/json',
            async: true,
            //dataType: "text",
            url: '../api/Link',
            data: JSON.stringify(src),
            success: function (reply) {
                var data = JSON.parse(reply);
                clearLinks();
                if (data.Rst == 1) {                    
                    showLinks(data);
                }
                else {
                    alert("No links found!");
                }
            },
            error: function (error) {
                jsonValue = jQuery.parseJSON(error.responseText);
            }
        });
    }
}
function showLinks(data) {
    if (data.Count > 0) {
        $('#linksCtr').html(data.Count);
        var row = "";
        var statusHtml = "";
        $.each(data.Users, function (index, item) {
            var itemKey = item.LinkId + "-"+ item.Key + "-" + item.Name + "-" + item.Status;
            if (item.Status == 1)
                statusHtml = "<button class='btn btn-warning btn-xs' aria-label='Requested'><span class='glyphicon glyphicon-exclamation-sign' aria-hidden='true' title='Request pending'></span></button>";
            if (item.Status == 2)
                statusHtml = "<button class='btn btn-danger btn-xs' aria-label='Requested'><span class='glyphicon glyphicon-check' aria-hidden='true' title='Accept link'></span></button>";
            row += "<tr id='" + itemKey + "' class='linkItem' ><td style='width:155px'>" + item.Name + "</td><td style='text-align:center'>" + statusHtml + "</td></tr>";
        });
        $('#linksList').html(row);
        //$('#linksPanelBody').show();
    }
}
function clearLinks() {
    $('#linksList').html("");
    $('#linksCtr').html("");    
}

function userLinkActionModal(linkId, uKey, uName, uStatus) {
    var modalTitle = "";
    $('userLinkId').html("");
    $('userLinkKey').html("");
    $('userLinkName').html("");
    $('userLinkStatus').html("");
    $('#actionLinkAccept').show();
    modalTitle = (uName.length < 1) ? uKey : uName + " (" + uKey + ")";
    $('userLinkId').html(linkId);
    $('userLinkKey').html(uKey);
    $('userLinkName').html(uName);
    $('userLinkStatus').html(uStatus);
    $("#linkTitle").html(modalTitle);
    if (uStatus == 0) {
        $('#userLinkInfo').html("Link is pending with your friend! Would like to remove your request?");
        $('#actionLinkAccept').hide();
    }
    if (uStatus == 1) {
        $('#userLinkInfo').html("Would you like to link with your friend?");
    }
    if (uStatus == 3) {
        $('#userLinkInfo').html("Would you like to block your link to your friend?");
    }
    $("#uLinkDetail").modal('show');
}
function userLinkAccept() {
    var loginKey = readCookie("fms");
    if (loginKey != null) {
        var cookieVal = loginKey.split("-");
        var src = {
            'Key': cookieVal[0],
            'LinkId': $('userLinkId').val(),
            'Status': 2
        }
        $.ajax({
            type: 'PUT',
            contentType: 'application/json',
            async: true,
            //dataType: "text",
            url: '../api/Link',
            data: JSON.stringify(src),
            success: function (reply) {
                var data = JSON.parse(reply);
                if (data.Rst == 1) {
                    alert("Done successfully.")
                }
                else {
                    alert("Internal failure.");
                }
            },
            error: function (error) {
                jsonValue = jQuery.parseJSON(error.responseText);
            }
        });
    }
    $('#uLinkDetail').modal('hide');
}

$(document).ready(function () {
    activeNavbarLink();
    
    // user auth
    $("#partialLogin").hide();
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

    $('body').on('click', '#loginLink', function () {
        //if ($('#partialLoginName').text() == "Login")
            popLogin(0);
    });

    // location
    $('#status').html("Loaded.");

    initializeMap();

    //links
    getLinks();
    setInterval(getLinks(), timeInt);
    $('body').on('click', '#addLink', function () {
        $('#modal-linkAdd').modal('show');
    });
    $('body').on('click', '#addLinkSubmit', function () {
        addLink();
    });
    
    //$('body').on('click', '.linkItem', function (event) {
    //    //linkItem id contains : item.LinkId + "-" + item.Key + "-" + item.Name + "-" + item.Status
    //    var itemKey = $(this).attr("id").split("-");
    //    userLinkActionModal(itemKey[0], itemKey[1], itemKey[2], itemKey[3]);
    //});
    //$('body').on('click', '#actionLinkAccept', function (event) {
    //    userLinkAccept();
    //});
});