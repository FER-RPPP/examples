$(function () {
  $(document).on('click', '.delete', function (event) {
    if (!confirm("Delete entry?")) {
      event.preventDefault();
    }
  });
});
