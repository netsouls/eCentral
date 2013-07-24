// document ready function
$(document).ready(function() { 	

	//--------------- Data tables ------------------//
	if($('table').hasClass('dynamicTable')){
		$('.dynamicTable').dataTable( {
			"sDom": "<'row-fluid'<'span6'l><'span6'f>r>t<'row-fluid'<'span6'i><'span6'p>>",
			"sPaginationType": "bootstrap",
			"bJQueryUI": true,
			"bAutoWidth": false,
			"oLanguage": {
				"sSearch": "<span>Filter:</span> _INPUT_",
				"sLengthMenu": "<span>_MENU_ entries</span>",
				"oPaginate": { "sFirst": "First", "sLast": "Last" }
			}
		});
		$('.dataTables_length select').uniform();
	}

	if($('table').hasClass('tableTools')){
		$('.tableTools').dataTable( {
			//"sDom": "<'row-fluid'<'span6'l><'span6'f>r>t<'row-fluid'<'span6'i><'span6'p>>",
			"sDom": "<'row-fluid'<'span4'l><'span4'T><'span4'f>r>t<'row-fluid'<'span4'i><'span4'i><'span4'p>>",
			//"sDom": 'T<"clear">lfrtip',
			"oTableTools": {
				"sSwfPath": "plugins/tables/dataTables/swf/copy_csv_xls_pdf.swf",
				"aButtons": [
				"copy",
				"print",
					{
						"sExtends":    "collection",
						"sButtonText": 'Save <span class="caret" />',
						"aButtons":    [ "csv", "xls", "pdf" ]
					}
				]
			},
			"sPaginationType": "bootstrap",
			"bJQueryUI": false,
			"bAutoWidth": false,
			"oLanguage": {
				"sSearch": "<span>Filter:</span> _INPUT_",
				"sLengthMenu": "<span>_MENU_ entries</span>",
				"oPaginate": { "sFirst": "First", "sLast": "Last" }
			}
		});
		$('.dataTables_length select').uniform();
	}

	// Set the classes that TableTools uses to something suitable for Bootstrap
	$.extend( true, $.fn.DataTable.TableTools.classes, {
		"container": "btn-group",
		"buttons": {
			"normal": "btn",
			"disabled": "btn disabled"
		},
		"collection": {
			"container": "DTTT_dropdown dropdown-menu",
			"buttons": {
				"normal": "",
				"disabled": "disabled"
			}
		}
	} );

	// Have the collection use a bootstrap compatible dropdown
	$.extend( true, $.fn.DataTable.TableTools.DEFAULTS.oTags, {
		"collection": {
			"container": "ul",
			"button": "li",
			"liner": "a"
		}
	} );

	//Boostrap modal
	$('#myModal').modal({ show: false});
	
	//add event to modal after closed
	$('#myModal').on('hidden', function () {
	  	console.log('modal is closed');
	})

});//End document ready functions

//sparkline in sidebar area
var positive = [1,5,3,7,8,6,10];
var negative = [10,6,8,7,3,5,1]
var negative1 = [7,6,8,7,6,5,4]

