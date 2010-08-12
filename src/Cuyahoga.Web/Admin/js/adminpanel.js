$(document).ready(function() {

    $(".editorcontextmenu").toggle(
    function() {
        $(this).attr("src", "/Admin/Images/arrow-up-red.png");
        /*$(this).next().next().css({ 'border': '1px solid #52c131' });*/
        $(this).next().next().css({ 'background-color': '#e1ffd3' });
        /*$(this).next().css({ "background-color": "#52c131" });*/
        $(this).next().fadeIn(".5");
    },
    function() {
        $(this).attr("src", "/Admin/Images/arrow-down-green.png");
        $(this).next().next().css({ 'background-color': 'Transparent' });
        $(this).next().next().css({ "border": "0" });
        $(this).next().fadeOut(".5");
    });

});