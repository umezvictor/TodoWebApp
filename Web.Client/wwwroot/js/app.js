$(document).ready(function () {
   // alert('hello');


    //$.ajax({
    //    url: 'http://localhost:3002/api/GetCompanies',  
    //        type: 'GET',
    //        dataType: 'json', 
    //        success: function (data, textStatus, xhr) {
    //        console.log(data);
        
    //},
    //    error: function (xhr, textStatus, errorThrown) {
    //        console.log('Error in Database');
                           
    //    }  
    //    });



    //$(document).on('click', '.edit', function () {

    //    var title = $('#title').text();

        
    //    $('#editModal').modal('show');//load modal


    //    $('#etitle').val(title);
      

    //});


    // code to read selected table row cell data (values).
    $("#basic-datatable").on('click', '.edit', function () {
        // get the current row
        var currentRow = $(this).closest("tr");

        //var id = currentRow.find("td:eq(0)").html(); // get current row 1st table cell TD value
        var title = currentRow.find("td:eq(0)").html(); // get current row 2nd table cell TD value
        //var col3 = currentRow.find("td:eq(2)").html(); // get current row 3rd table cell  TD value
       // var data = col1 + "\n" + col2 + "\n" + col3;

        //alert(data);

        $('#editModal').modal('show');//load modal


        $('#etitle').val(title);
    });




    // code to read selected table row cell data (values).
    $("#basic-datatable").on('click', '.delete', function () {
        // get the current row
        var myid = $(this).closest("tr").attr('id');

       
        alert(myid);

       
    });

});


//data-toggle="modal" data-target="#editModal"


//add todo
$("#addTodoForm").submit(function (event) {
    event.preventDefault();
    //get values
    //var todo = new Object();
    //todo.name = $('#name').val();
    //person.surname = $('#surname').val();
    var title = $('#title').val();
    alert(title);
    //$.ajax({
    //    url: 'http://localhost:3413/api/person',
    //    type: 'POST',
    //    dataType: 'json',
    //    data: person,
    //    success: function (data, textStatus, xhr) {
    //        console.log(data);
    //    },
    //    error: function (xhr, textStatus, errorThrown) {
    //        console.log('Error in Operation');
    //    }
    //});
});



//edit todo
$("#editTodoForm").submit(function (event) {
    event.preventDefault();
    //get values
    //var todo = new Object();
    //todo.name = $('#name').val();
    //person.surname = $('#surname').val();

    var tr = ele.target.parentNode.parentNode;
    var title = $('#title').val();
    alert(title);
    //$.ajax({
    //    url: 'http://localhost:3413/api/person',
    //    type: 'POST',
    //    dataType: 'json',
    //    data: person,
    //    success: function (data, textStatus, xhr) {
    //        console.log(data);
    //    },
    //    error: function (xhr, textStatus, errorThrown) {
    //        console.log('Error in Operation');
    //    }
    //});
});




$("#delTodo").click(function () {
    var isConfirmed = confirm("Are you sure?");
    if (isConfirmed) {
        alert('hello');
    }

    //var person = new Object();
    //person.name = $('#name').val();
    //person.surname = $('#surname').val();
    //$.ajax({
    //    url: 'http://localhost:3413/api/person',
    //    type: 'DELETE',
    //    dataType: 'json',
    //    data: person,
    //    success: function (data, textStatus, xhr) {
    //        console.log(data);
    //    },
    //    error: function (xhr, textStatus, errorThrown) {
    //        console.log('Error in Operation');
    //    }
    //});
});