//Adds functionality to the display of a database table to add, edit, and delete its items. also decorates with some color highlighting.
$(document).ready(function() 
{	
	$('#EditArea').hide();
	
	$("#add").click(function () 
	{
		memberAdd();
	});
	$("#save").click(function () 
	{
		memberSave();
	});
	$("#cancel").click(function() 
	{		
		$("#EditArea").slideUp(1000);		
		refreshTR_BG();
	});
	getTableData();

});
function getTableData() 
{	
	$.getJSON("ajaxtest.php", "", 
		function (data)
		{
			displayTable(data);
			console.log("displaying table");
		}
	)			
}
var lastcolor = "";
function displayTable(dt) 
{
	$("#FacultyTable").empty();
	console.log("inside displayTable");
	$.each(dt, function(key, val) 
	{
		console.log("each");
		
		var movie = '<td>' + val['title'] + '</td><td>' + val['year'] + '</td><td>' + val['studio'] + '</td><td>' + val['price'] + '</td>';
		var id = val['id'];
		var btn1 = "<input type='button' value='Edit' class='btnEdit' data-id='" + id + "'>";
		var btn2 = "<input type='button' value='Delete' class='btnDelete' data-id='" + id + "'>";

		//$("#FacultyTable").append('<tr data-id="' + id + '">' + movie + '<td>' + btn1 + '</td><td>' + btn2 + '</td></tr>' + "</br>");		
		$("#FacultyTable").append('<tr data-id="a'+id+'">' + movie + '<td>' + btn1 + '</td><td>' + btn2 + '</td></tr>' + "</br>");		
	});
	
	$(".btnEdit").click(function() 
	{	
		var id = $(this).attr("data-id");
		refreshTR_BG();
		$('[data-id="a'+id+'"]').css("background-color", "#83c489");
		lastcolor = "#83c489";
		memberEdit(id);
	});
	$(".btnDelete").click(function() 
	{		
		//slide up edit area in case 'edit' was pressed before
		$("#EditArea").slideUp(1000);		
		refreshTR_BG();
		
		var id = $(this).attr("data-id");		
		$('[data-id="a'+id+'"]').css("background-color", "#c49a83");
		lastcolor = "#c49a83";
		memberDelete(id);
	});
	
	$("tr").mouseenter(function() 
	{
		//console.log("mouseenter");
		lastcolor = $(this).css('background-color');
		$(this).css("background-color", "#a2c1d3");
	});
	$("tr").mouseleave(function() 
	{		
		console.log("mouseleave");
		$(this).css("background-color", lastcolor);
	});
	refreshTR_BG();
}
function getTableData2() 
{
	$("#FacultyTable").empty();
	$("#FacultyTable").load("ajaxtest.php");	
}		
function memberEdit(id) 
{
	console.log("memberEdit");		
	$.post('facultyRetrieve.php', { id: id}, 
	function(returnedData)
	{
		rd = JSON.parse(returnedData)[0];
		// paint edit fields with data
		$("#action").val("edit");
		$("#id").val(rd["id"]);
		$("#title").val(rd["title"]);
		$("#year").val(rd["year"]);
		$("#studio").val(rd["studio"]);
		$("#price").val(rd["price"]);
		$("#EditArea").slideDown(1000);
	});

	$("#title").focus();
}

function memberDelete(id) 
{	
	// get faculty info by id
	$.post('facultyRetrieve.php', { id: id}, 
		function(returnedData)
		{
			rd = JSON.parse(returnedData)[0];
			
			id = rd["id"];
			title = rd["title"];
			year = rd["year"];
			studio = rd["studio"];
			price = rd["price"];
	
			if (confirm("Do you really want to delete:\n" + title + ", " + year + ", " + studio + ", " + price + "?") ) 
			{
				console.log("CONFIRMED!");
				$.post('facultyDelete.php', { id: id}, function(returnedData)
					{
						getTableData();
					}); 
				console.log("back from delete.php...");				
			}
			else
			{
				console.log("calling delete's refresh");				
				lastcolor = "#83adc4";
				refreshTR_BG();
			}
			
			
		}
	);
}

function memberAdd() 
{
	// paint edit fields with blanks
	$("#action").val("add");
	$("#id").val("");
	$("#title").val("");
	$("#year").val("");
	$("#studio").val("");
	$("#price").val("");
	$("#EditArea").slideDown(1000);

	$("#title").focus();
}
function memberSave() 
{
	$("#EditArea").slideUp(1000);
	
	action = $("#action").val();
	id = $("#id").val();
	title = $("#title").val();
	year = $("#year").val();
	studio = $("#studio").val();
	price = $("#price").val();
	
	$.post('facultySave.php', { action: action, id: id, title: title, year: year, studio:studio, price:price},
		function(returnedData) 
		{
			getTableData();
		}
	);
}

function refreshTR_BG()//sets background color for all rows
{
	console.log("refreshed");
	$("#FacultyTable tr").css("background-color", "#83adc4");	//refresh
}