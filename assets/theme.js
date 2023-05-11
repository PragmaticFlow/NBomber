"use strict";

!function() {
    if ($("#nav-toggle").length && $("#nav-toggle").on("click", function(t) {
        t.preventDefault(), $("#db-wrapper").toggleClass("toggled");
    }), $(".nav-scroller").length && $(".nav-scroller").slimScroll({
        height: "97%"
    }), $(".notification-list-scroll").length && $(".notification-list-scroll").slimScroll({
        height: 300
    }), $('[data-bs-toggle="tooltip"]').length) [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]')).map(function(t) {
        return new bootstrap.Tooltip(t);
    });
    if ($('[data-bs-toggle="popover"]').length) [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]')).map(function(t) {
        return new bootstrap.Popover(t);
    });
    $('[data-bs-spy="scroll"]').length && [].slice.call(document.querySelectorAll('[data-bs-spy="scroll"]')).forEach(function(t) {
        bootstrap.ScrollSpy.getInstance(t).refresh();
    });
    if ($(".toast").length) [].slice.call(document.querySelectorAll(".toast")).map(function(t) {
        return new bootstrap.Toast(t);
    });    
    if ($(".offcanvas").length) [].slice.call(document.querySelectorAll(".offcanvas")).map(function(t) {
        return new bootstrap.Offcanvas(t);
    });
}(), feather.replace(), function() {
    var t = window.location + "", e = t.replace(window.location.protocol + "//" + window.location.host + "/", "");
    $("ul#sidebarnav a").filter(function() {
        return this.href === t || this.href === e;
    }).parentsUntil(".sidebar-nav").each(function(t) {
        $(this).is("li") && 0 !== $(this).children("a").length ? ($(this).children("a").addClass("active"), 
        $(this).parent("ul#sidebarnav").length, $(this).addClass("active")) : $(this).is("ul") || 0 !== $(this).children("a").length ? $(this).is("ul") && $(this).addClass("in") : $(this).addClass("active");
    });
}();