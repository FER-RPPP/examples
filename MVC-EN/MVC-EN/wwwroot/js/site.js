function clearOldMessage() {
  $("#tempmessage").siblings().remove();
  $("#tempmessage").removeClass("alert-success");
  $("#tempmessage").removeClass("alert-danger");
  $("#tempmessage").html('');
}

$(function () {
  $(document).on('click', '.delete', function (event) {
    if (!confirm("Delete entry?")) {
      event.preventDefault();
    }
  });
});
