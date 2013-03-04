(function (window) {    
    window.events = {
        updateEmailItem1: function (data) {
            window.document.getElementById("para1").innerHTML = data;
        },
        updateUsersItem2: function (data) {
            window.document.getElementById("para2").innerHTML = data;
        }
    }
})(window);