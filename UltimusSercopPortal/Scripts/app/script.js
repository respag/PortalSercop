/// <reference path="../jquery-1.10.2.js" />
var root = location.protocol + "//" + location.host;

var url = root + "/UltimusSercopPortal/Home/Initiate?userLoged=true"

$('#myModal').modal('show')

$("#btnLogon").click(function () {
    location.href = "/UltimusSercopPortal/Home/Index";
});

$("#btnClose").click(function () {
    location.href = url;
});

$("#x").click(function () {
    $(".accDenegado").removeClass("oculto");
});


$('[data-toggle=offcanvas]').click(function () {
    $('.row-offcanvas-left').toggleClass('active');
});

$('[data-toggle=offcanvasright]').click(function () {
    $('.row-offcanvas-right').toggleClass('active');
});


$("a.botones").tooltip({
    placement: 'bottom'
});

$(window).resize(function () {
    console.log($(window).width());
});






//***************************************************************
$(document).ready(function () {
    $("a.botones").tooltip({
        placement: 'bottom'
    });

    $("a.boton2").tooltip({
        placement: 'right'
    });

    $("#linkUsuario").click(function () {
        $().toastmessage('showToast', {
            text: $("#usr").text(),
            sticky: true,
            position: 'top-right',
            type: 'notice',
            closeText: '',
            close: function () { console.log("toast is closed ..."); }
        });
    });

    // Agregamos en los botones (tanto del panel izquierdo como del header un atributo data-href con el link
    // a la pagina Initiate
    $("#initiate, #initiate2").attr("data-href", $("#blupInitiate").val());
   
    // Agregamos en los botones (tanto del panel izquierdo como del header un atributo data-href con el link
    // a la pagina Inbox
    $("#inbox, #inbox2").attr("data-href", $("#blup").val());
    
    // Agregamos en los botones (tanto del panel izquierdo como del header un atributo data-href con el link
    // a la pagina Completed
    $("#completed, #completed2").attr("data-href", $("#blupCompleted").val());
  
    // Ahora cuando hacemos click en los links navegamos a las páginas  
    $("#initiate, #initiate2").click(function () {
        location.href = $("#initiate").attr("data-href");
    })
    $("#inbox, #inbox2").click(function () {
        location.href = $("#inbox").attr("data-href");
    })
    $("#completed, #completed2").click(function () {
        location.href = $("#completed").attr("data-href");
    })

    //.wen .nav li   
})
