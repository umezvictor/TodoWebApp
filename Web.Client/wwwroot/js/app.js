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
                $('#addModal').modal('hide');
                notyf.success(data.message);
                loadTodos();
               
            } else {
                document.getElementById('title').value = "";
                $('#addModal').modal('hide');
                notyf.error(data.message);
                loadTodos();
            }
        }, complete: function () {
        }, error: function (data) {
            notyf.success(data.message);
            loadTodos();
        }
    });
});



//edit todo
$("#editTodoForm").submit(function (event) {
    event.preventDefault();

    var notyf = new Notyf({
        duration: 5000,
        position: {
            x: 'right',
            y: 'top',
        }
    });

    var titleText = $('#etitle').val();
    var id = $('#todoId').val();

    var completed = false;

    //check if todo is completed
    if (document.getElementById('isCompleted').checked) {
         completed = true;
    }

    $.ajax({
        contentType: "application/json; charset=utf-8",
        url: `https://localhost:5002/api/Todo/${id}`,
        type: 'PUT',
        dataType: 'json',
        data: JSON.stringify({ "title": titleText, "id": id, "completed": completed }),
        beforeSend: function () {
        }, success: function (data) {
            if (data.success) {
                document.getElementById('etitle').value = "";
                document.getElementById("isCompleted").checked = false;
                $('#editModal').modal('hide');
                notyf.success(data.message);
                loadTodos();

            } else {
                document.getElementById('etitle').value = "";
                document.getElementById("isCompleted").checked = false;
                $('#editModal').modal('hide');
                notyf.error(data.message);
                loadTodos();
            }
        }, complete: function () {
        }, error: function (data) {
            notyf.success(data.message);
            loadTodos();
        }
    });
});




function deleteTodo(id) {
    var notyf = new Notyf({
        duration: 5000,
        position: {
            x: 'right',
            y: 'top',
        }
    });

    
    $.ajax({
        url: `https://localhost:5002/api/Todo/${id}`,
        type: 'DELETE',
        beforeSend: function () {
        }, success: function (data) {
            if (data.success) {
                
                notyf.success(data.message);
                loadTodos();

            } else {
               
                notyf.error(data.message);
                loadTodos();
            }
        }, complete: function () {
        }, error: function (data) {
            notyf.success(data.message);
            loadTodos();
        }
    });
}




function loadTodos() {

    var notyf = new Notyf({
        duration: 5000,
        position: {
            x: 'right',
            y: 'top',
        }
    });

    var tableHolderDiv = document.getElementById('tableHolder');
    //clear the table
    tableHolderDiv.innerHTML = "";

    fetch('https://localhost:5002/api/todo', {
        method: "GET"
    })
        .then(response => response.json())
        .then(res => {
            if (res.success) {

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
                notyf.error("No record found")
            }
        })
        .catch (err => notyf.error("An error occured"));

}
