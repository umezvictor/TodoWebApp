$(document).ready(function () {
    loadTodos();

});


//add todo
$("#addTodoForm").submit(function (event) {
    event.preventDefault();
    var notyf = new Notyf({
        duration: 5000,
        position: {
            x: 'right',
            y: 'top',
        }
    });
    //get values
    var titleText = $('#title').val();
      
    $.ajax({
        contentType: "application/json; charset=utf-8",
        url: 'https://localhost:5002/api/Todo',
        type: 'POST',
        dataType: 'json',
        data: JSON.stringify({ "title": titleText }),
        beforeSend: function () {
        }, success: function (data) {
            if (data.success) {
                document.getElementById('title').value = "";
                notyf.success(data.message);
                loadTodos();
               
            } else {
                notyf.error(data.message);
            }
        }, complete: function () {
        }, error: function (data) {
            notyf.success(data.message);
        }
    });
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


function deleteTodo(todoId) {
    alert(todoId);
}

//https://localhost:5002/api/todo


function loadTodos() {

    //alert('hello');

    //var notyf = new Notyf({
    //    duration: 5000,
    //    position: {
    //        x: 'right',
    //        y: 'top',
    //    }
    //});

    var tableHolderDiv = document.getElementById('tableHolder');
    //clear the table
    tableHolderDiv.innerHTML = "";

    fetch('https://localhost:5002/api/todo', {
        method: "GET"
    })
        .then(response => response.json())
        .then(res => {
            if (res.success) {


                //console.log(res.payload);
                const table = document.createElement('table');
                table.setAttribute("id", "basic-datatable");
                table.setAttribute("class", "table table-striped table-bordered table-sm key-button");

                const tableHead = document.createElement('thead');
                const trmain = document.createElement('tr');

                const thTitle = document.createElement('th');
                thTitle.innerText = "Title";

                const thCompleted = document.createElement('th');
                thCompleted.innerText = "Completed";


               const thActions = document.createElement('th');
               thActions.innerText = "Actions";

                trmain.appendChild(thTitle);
                trmain.appendChild(thCompleted);
                trmain.appendChild(thActions);


                tableHead.appendChild(trmain);
                table.appendChild(tableHead);

                var tbody = document.createElement('tbody');

                for (let i = 0; i < res.payload.length; i++) {                  
                   //create a tr 
                    let tr = document.createElement('tr');
                   
                    //create tds
                    let titleTd = document.createElement('td');                   
                    let completeTd = document.createElement('td');
                    let actionsTd = document.createElement('td');

                    //add edit button
                    var editBtn = document.createElement("button");
                    editBtn.innerHTML = "Edit";
                    editBtn.setAttribute('type', 'button');
                    editBtn.setAttribute('class', 'btn btn-success btn-sm');

                    editBtn.addEventListener("click", function (event) {
                        //populate and display edit modal here
                        $('#editModal').modal('show');
                        $('#todoId').val(res.payload[i].id);
                        $('#etitle').val(res.payload[i].title);
                    });
                    actionsTd.appendChild(editBtn);


                    //add delete button
                    var deleteBtn = document.createElement("button");
                    deleteBtn.innerHTML = "Delete";
                    deleteBtn.setAttribute('type', 'button');
                    deleteBtn.setAttribute('class', 'btn btn-danger btn-sm ml-2');

                    deleteBtn.addEventListener("click", function (event) {
                        deleteTodo(res.payload[i].id);
                    });
                    actionsTd.appendChild(deleteBtn);


                    let titleText = document.createTextNode(res.payload[i].title);                   
                    let completedText = document.createTextNode(res.payload[i].completed);           

                   //set text nodes inside tds
                    titleTd.appendChild(titleText);
                    completeTd.appendChild(completedText);


                   //appends tds to tr
                    tr.appendChild(titleTd);
                    tr.appendChild(completeTd);
                    tr.appendChild(actionsTd);

            

                   //append tr to tbody
                   tbody.appendChild(tr);
                }
               
                table.appendChild(tbody);
                tableHolderDiv.appendChild(table);
                $('#basic-datatable').DataTable();
            

            }else {
                console.log('bad');
            }
        })
        .catch (err => notyf.error("An error occured"));

}
