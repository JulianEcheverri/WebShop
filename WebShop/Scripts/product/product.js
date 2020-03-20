// Init for sweetalert2 library: Animated messages
const Toast = Swal.mixin({
    toast: true,
    position: 'top-end',
    showConfirmButton: false,
    timer: 5000,
    timerProgressBar: true,
    onOpen: (toast) => {
        toast.addEventListener('mouseenter', Swal.stopTimer);
        toast.addEventListener('mouseleave', Swal.resumeTimer);
    }
});

$(function () {
    $('#CreateProduct').on('click', function () {
        $.ajax({
            type: 'POST',
            url: $('#ProductsTable').data('create'),
            success: function (view) {
                showModalProductManage(view);
            },
            error: function (xhr) {
                console.log(xhr.responseText);
            }
        });
    });

    $(document).on('click', '.editProduct', function () {
        let $this = $(this);
        $.ajax({
            type: 'POST',
            url: $('#ProductsTable').data('edit'),
            data: {
                Id: $this.data('id'),
                SaveInMemory: $this.data('inmemory')
            },
            success: function (view) {
                showModalProductManage(view, false);
            },
            error: function (xhr) {
                console.log(xhr.responseText);
            }
        });
    });

    // Form registration in the dom for jquery validate
    $("#ModalProductsManage").on('shown.bs.modal', function () {
        $.validator.unobtrusive.parse('#FormCreateOrEditProduct');
    });

    $("#UdpateProduct").on('click', function () {
        let $ModalProductsManage = $('#ModalProductsManage');
        let $form = $ModalProductsManage.find('.modal-body').find('form');
        if ($form.length <= 0) {
            $ModalProductsManage.modal('hide');
            return;
        }
        UpdateProduct($form);
    });
});

// Shows the modal
function showModalProductManage(view, create = true) {
    let $ModalProductsManage = $('#ModalProductsManage');
    $ModalProductsManage.find('.modal-body').html(view);
    $ModalProductsManage.find('.modal-title').text(create === true ? 'Product Creation' : 'Product Editing');
    $ModalProductsManage.modal();
}

function UpdateProduct($form) {
    if ($form.valid()) {
        // Messages located in the form
        let successMessage = $form.data('success');
        let errorMessage = $form.data('failure');
        let $saveInMemoryObject = $('#SaveInMemory');
        // Radio button for data storage choosing. Only when the user is creating can the storage be chosen
        let $memoryStorageRadioButton = $('#MemoryStorageRadioButton');
        let saveInMemory = $memoryStorageRadioButton.length > 0 ? $memoryStorageRadioButton.is(':checked') : $saveInMemoryObject.val();
        // Data storage assignment
        $saveInMemoryObject.val(saveInMemory);

        $.ajax({
            type: 'POST',
            url: $form.attr('action'),
            data: $form.serialize(),
            success: function (resp) {
                if (!resp.Success) {
                    // Show the message
                    showMessages(resp.ValidationCode === 0 ? 'There is a product with the same title in the database' : errorMessage, 'error');
                    return;
                }
                let $ProductsTable = $('#ProductsTable').find('tbody');
                let number = $('#Number').val();
                let titile = $('#Title').val();
                let price = $('#Price').val();
                let $tdEmtyTable = $('.tdEmtyTable');
                if ($tdEmtyTable.length > 0) $tdEmtyTable.remove();

                // If the current message from the form doesn't have 'edit' word, means that a new record is being created
                if (successMessage.indexOf('edit') <= '0') {
                    $ProductsTable.append(
                        $('<tr>', { "class": 'text-left' }).append(
                            $('<td>', { "class": 'number', html: number }),
                            $('<td>', { "class": 'title', html: titile }),
                            $('<td>', { "class": 'price', html: price }),
                            $('<td>').append(
                                $('<button>',
                                    {
                                        "class": 'btn btn-outline-success btn-sm editProduct btn-block',
                                        text: 'Edit',
                                        'data-id': resp.ProductId,
                                        'data-inmemory': resp.SaveInMemory
                                    }
                                ),
                            )
                        )
                    );
                }
                // In the editing part, we look for the button corresponding to ProductId for replacing the new values that the user modified
                else {
                    let $ProductToEdit = $ProductsTable.find(`button[data-id=${resp.ProductId}]`).parents('tr');
                    $ProductToEdit.find('.number').html(number);
                    $ProductToEdit.find('.title').html(titile);
                    $ProductToEdit.find('.price').html(price);
                }
                showMessages(successMessage, 'success');
                $('#ModalProductsManage').modal('hide');
            },
            error: function () { onFailure(errorMessage); }
        });
    }

    // For showing messages with sweetalert2
    function showMessages(message, icon) {
        Toast.fire({
            icon: icon,
            title: message
        });
    }
}