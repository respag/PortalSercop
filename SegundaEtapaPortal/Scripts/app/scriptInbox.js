/// <reference path="../moment-with-locales.min.js" />
/// <reference path=""~/Scripts/DataTables-1.10.2/jquery.dataTables.min.js"/>

/*
************************************************************
* Copyright © Ultimus.                                     *
* Diseño y desarrollo: Rubén Enrique Spagnuolo (respag)    *
* Panamá - 2014                                            *
************************************************************
*/
var usr = $("#nombre").text().substring(0, 30).trim();
$(document).ready(function () {
     var flag = true;
   
     $.getJSON(addrApi + "api/Inbox/" + dom + "/" + usr, function (data) {
         var obj = data;
         var oTable = $('#example').dataTable({
             language: {
                 processing: "Procesando...",
                 search: " ",
                 lengthMenu: "Mostrando _MENU_ registros",
                 info: "Registros <b style='color:red'>_START_ a _END_</b> de un total de <b class='medalla'>_TOTAL_</b>.",
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
                 $("#inboxLargo").text($("#example_wrapper #example_info b.medalla").text());
                 $("#inboxLargo").css("visibility", "visible");

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

                 //Si no hay elementos en la tabla entonces cambia el cursor al normal
                 if ($("b.medalla").text() == "")
                     $("#example tbody").css("cursor", "default");
             },

             //Esta función permite un " post proceso" de cada fila después de que se ha generado 
             //pero antes de presentarla en la pantalla.
             "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
                 //Agrega a las primeras columnas de cada fila el icono
                 $('td:eq(0)', nRow).html("<span style='font-size:18px; color:darkblue'><i class='fa fa-picture-o'></i></span>")

                 //// Selecciona el quinto td para cada fila y le cambia el texto ejecutando
                 //// la funcion devuelveEstadoTarea, pasándole como parámetro el valor de 
                 //// la columna Status
                 //$('td:eq(5)', nRow).text(devuelveEstadoTarea(parseInt(aData["STATUS"])));
                 ////Selecciona la cuarta columna para cada fila y formatea la fecha en el formato dd/MM/yyyy.
                 $('td:eq(4)', nRow).text(moment.parseZone(aData["FechaProgramada"]).format('DD-MM-YYYY hh:mm A'));
             },
             "aLengthMenu": [[5,10, 25, 50], [5, 10, 25, 50]],
             "iDisplayLength": 5,
             "aaData": obj,
             "order": [],
             "aoColumns": [
                 { "mDataProp": null, bSearchable: false, bSortable: false },
                 { "mData": "PROCESSNAME" },
                 { "mData": "STEPLABEL" },
                 { "mData": "CodigoProceso" },
                 { "mData": "FechaProgramada" },
                 { "mData": "ESTADOPROCESO", "sClass": "alignCenter" },
                 { "mData": "ObjetoContratacion" },
                 { "mData": "INCIDENT", "sClass": "columnaOculta" },
                 { "mData": "PROCESSVERSION", "sClass": "columnaOculta" },
                 { "mData": "TASKID", "sClass": "columnaOculta" }
             ],
         });
     });
 });

$('#example thead, #example tfoot').css({ "background-color": "#202020", "color": "white" });

function devuelveEstadoTarea(st){
    if(st===1) {
        return "Activo";
    }
    else if (st === 3) {
        return "Completado";
    }
    else {
        return st;
    }
 }

$('#example').on('click', 'tbody tr td:not(:first-child)', function (event) {
    var correo = $("#correo").text();
    var rol = $("#rol").text();
    var cedula = $("#cedula").text();
    var empresa = $("#empresa").text();
    var tipoEmpresa = ($("#tipoEmpr").text()).trim();
    var ruc = $("#ruc").text();
    var id = $(this).parent().find("td:nth-child(10)").text();
    var idSercopEmpresa = $("#IDSercopEmpresa").text();
    var idUltimusEmpresa = $("#IDUltimusEmpresa").text();
    var idSercopSucursal = $("#IDSercopSucursal").text();
    var idUltimusSucursal = $("#IDUltimusSucursal").text();

    var url = rutaFormularios + "?UserID=" + dom + "/" +
                      usr + "&TaskID=" + id + "&empresa=" + empresa + "&ruc=" + ruc + "&tipoEmpresa=" +
                      tipoEmpresa + "&cedula=" + cedula + "&rol=" + rol + "&correo=" + correo +
                      "&SercopEmpresa=" + idSercopEmpresa + "&UltimusEmpresa=" + idUltimusEmpresa +
                      "&SercopSucursal=" + idSercopSucursal + "&UltimusSucursal=" + idUltimusSucursal;
    window.open(url, "newWindow", "scrollbars=yes, height=" + screen.height + ", width =" + screen.width);
});


// Al hacer click en la primera columna de cada fila (la imagen)
$('#example').on('click', 'tbody tr td:first-child', function (event) {
    var nombre = $(this).parent().find("td:nth-child(2)").text(); 
    var inc = $(this).parent().find("td:nth-child(8)").text();
    var ver = $(this).parent().find("td:nth-child(9)").text();
    if (inc !== "")
        //location.href = baseUrl + "home/MuestraImagen?processName=" + nombre.replace(" ", "+").trim() + "&incidente=" + inc + "&version=" + ver;
        location.href = baseUrl + "home/MuestraImagen?processName=" + nombre.split(" ").join('+').trim() + "&incidente=" + inc + "&version=" + ver;
});