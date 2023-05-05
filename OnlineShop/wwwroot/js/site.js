@if (ViewBag.Message != null) {
    <script>Swal.fire({
        title: '@ViewBag.Message',
        timer: 1500,
        showConfirmButton: false,
        icon: 'success',
        position: 'top-center'
       });</script>
}

@if (ViewBag.ErrorMessage != null) {
    <script>Swal.fire({
        title: '@ViewBag.ErrorMessage',
        timer: 1500,
        showConfirmButton: false,
        icon: 'warning',
        color: '#000000',
        position: 'top'
       });</script>
}