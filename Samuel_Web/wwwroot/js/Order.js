var dataTable;
$(document).ready(function () {
    var url = window.location.search;
    if (url.includes("inprocess")) {
        loadDataTable("inprocess");
    }
    else {
        if (url.includes("completed")) {
            loadDataTable("completed");
        }
        else {
            if (url.includes("pending")) {
                loadDataTable("pending");
            }
            else {
                if (url.includes("approved")) {
                    loadDataTable("approved");
                }
                else {
                    loadDataTable("all")
                }
            }
        }
    }

}
);


function loadDataTable(status) {
    dataTable = $('#tbldata').DataTable({
        "ajax": {
            url: '/admin/order/getall?status=' + status
        },
        "columns": [
            { data: 'id', "width": "10%"  ,className: "text-start text-center" },
            { data: 'name', "width": "18%", className: "text-start text-center" },
            { data: 'phoneNumber', "width": "18%", className: "text-start text-center" },
            /// emial 
            { data: 'applicationUser.email', "width": "13%", className: "text-start text-center" },
            { data: 'orderStatus', "width": "13%", className: "text-start text-center" },
            { data: 'orderTotal', "width": "10%", className: "text-start text-center" },

            {

                data: 'id',
                "render": function (data) {
                    return `<div class="w-100 btn-group" role="group"><a href="/admin/order/details?id=${data}" class="btn btn-outline-info mx-2" > <i class="bi bi-pen-fill"></i> </a > 
                   `;
                },
                "width": "25%"
            }


        ]
    });
}

