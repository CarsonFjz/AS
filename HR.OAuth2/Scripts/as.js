function bindSelectedSidebar() {
    var path = window.location.pathname;
    var sidebarArray = ["ResetPassword", "AddUser"];

    for (var i = 0; i < sidebarArray.length; i++) {
        if (path.toLowerCase().indexOf(sidebarArray[i].toLowerCase()) > -1) {

            selectedTab = document.getElementById(sidebarArray[i]);
            selectedTab.className = "selected";

            break;
        }
    }
}

function buildMenu() {
    $.ajax({
        type: "GET",
        url: "/Menu/GetAll",
        success: function (menus) {
            if(!$.isEmptyObject(menus) && menus.length > 0) {
                for (var i = 0; i < menus.length; i++) {
                    $(".appr_side_list").append("<li id='" + menus[i].AltTag + "'><a href='" + menus[i].URL + "'>" + menus[i].DisplayName + "</a></li>")
                }                
            }

            // bind selected sidebar.
            bindSelectedSidebar();
        }
    });
}