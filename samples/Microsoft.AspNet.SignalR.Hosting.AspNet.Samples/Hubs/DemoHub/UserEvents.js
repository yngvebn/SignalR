(function (window) {    
    window.events = {
        updateDataItem1: function (data) {
            window.document.getElementById("para1").innerHTML = data;
        },
        updateDataItem2: function (data) {
            window.document.getElementById("para2").innerHTML = data;
        }
    }
})(window);