/// <reference path="../jquery-1.10.2.js" />
/// <reference path=""~/Scripts/DataTables-1.10.2/jquery.dataTables.min.js"/>
var domain = "soce.int";
var usr = "alfresco";

$(document).ready(function () {
    console.log(ip);
    var flag = true;
    $.getJSON(addrApi + "api/Initiates/" + domain + "/" + usr, function (data) {
    var obj = jQuery.parseJSON(data)

        var oTable = $('#example').dataTable({
            language: {
                processing: "Procesando...",
                search: " ",
                lengthMenu: "Mostrando _MENU_ registros",
                info: "Registros <b style='color:red'>_START_ a _END_</b> de un total de <b class='medalla'>_TOTAL_</b>",
                infoEmpty: "No hay registros para mostrar.",
                infoFiltered: "(filtrado de _MAX_ registros en total)",
                infoPostFix: "",
                loadingRecords: "Cargando registros...",
                zeroRecords: "No hay registros para mostrar.",
                emptyTable: "No hay datos disponibles en la tabla",
                paginate: {
                    first: "Primero",
                    previous: "Anterior",
                    next: "Siguiente",
                    last: "Ultimo"
                },
                aria: {
                    sortAscending: ": activar para ordenar la columna ascendentemente",
                    sortDescending: ": activar para ordenar la columna descendentemente"
                }
            },
            "fnDrawCallback": function (oSettings) {
                $("#initiateLargo").text($("#example_wrapper #example_info b.medalla").text());
                $("#initiateLargo").css("visibility", "visible");

                $("#example_wrapper #example_filter input").css({
                    "border": "1px solid silver", "border-radius": "4px", "border-top-right-radius": "0",
                    "border-bottom-right-radius": "0", "height": "34px", "Width": "196px", "position": "relative", "top": "2px"
                });

                // Con el flag evitamos que se cargue la imagen de la lupa cada vez qu ese dibuja la tabla y solo lo hace la primera vez
                if (flag) {
                    $("#example_wrapper #example_filter input").after("<button class='btn btn-default' type='submit'>" +
                        "<span class='glyphicon glyphicon-search' aria-hidden='true'></span></button>");
                    flag = false
                }

                //Aplica estilos al select para paginación para que sea parecido al input de busqueda
                $("#example_length label select").css({
                    "border": "1px solid silver", "border-top-left-radius": "4px", "border-top-right-radius": "0",
                    "border-bottom-right-radius": "0", "border-bottom-left-radius": "4px", "height": "34px", "position": "relative", "top": "2px",
                    "padding": "6px 12px", "color": "rgb(85, 85, 85)", "font-size": "14px"
                });

                // Pinta de celeste claro las barras 
                $("#example_wrapper").css("background-color", "lightsteelblue");

                // Coloca el placeholder Buscar al input
                $("#example_wrapper #example_filter input").attr("placeholder", "Buscar");

                // Aplica 4 estilos al cuadro de texto de busqueda (el padding, el autofoco, el color y el tamaño de la fuente)
                $("#example_wrapper #example_filter input").css({ "padding": "6px 12px", "autofocus": "true", "color": "#555555", "font-size": "14px" });

                $("#example_wrapper #example_length").css({ "float": "left" });

                $("#example_wrapper #example_filter").css("float", "right");
            },
            "aLengthMenu": [[5, 10, 25, 50], [5, 10, 25, 50]],
            "iDisplayLength": 10,
            "aaData": obj,
            "order": [],
            "aoColumns": [
                 { "mData": "NombreProceso" },
                 { "mData": "VersionProceso", "sClass": "alignCenter" },
                 { "mData": "NombreTarea" },
                 { "mData": "Url" }
            ],
        });
    });

    $('#example thead, #example tfoot').css({ "background-color": "#202020", "color": "white" });

    $('#example').on('click', 'tbody tr', function (event) {
        var correo = $("#correo").text();
        var rol = $("#rol").text();
        var cedula = $("#cedula").text();
        var empresa = $("#empresa").text();
        var tipoEmpresa = ($("#tipoEmpr").text()).trim();
        var ruc = $("#ruc").text();
        var tId = ($(this).find('td:last').text()).substring(52, 83);
        var idSercopEmpresa = $("#IDSercopEmpresa").text();
        var idUltimusEmpresa = $("#IDUltimusEmpresa").text();
        var idSercopSucursal = $("#IDSercopSucursal").text();
        var idUltimusSucursal = $("#IDUltimusSucursal").text();

        var dir = rutaFormularios + "?UserID=" + domain + "/" + usr + "&TaskID=" + tId +
                     "&empresa=" + empresa + "&ruc=" + ruc + "&tipoEmpresa=" + tipoEmpresa +
                     "&cedula=" + cedula + "&rol=" + rol + "&correo=" + correo +
                     "&SercopEmpresa=" + idSercopEmpresa + "&UltimusEmpresa=" + idUltimusEmpresa +
                     "&SercopSucursal=" + idSercopSucursal + "&UltimusSucursal=" + idUltimusSucursal;

        window.open(dir, "newWindow", "scrollbars=yes, height=" + screen.height + ", width =" + screen.width);
    });
});