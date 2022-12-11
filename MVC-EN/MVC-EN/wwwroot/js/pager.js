$(function () {
  $('.pagebox').click(function () {
    $(this).select();
  });

  $('.pagebox').keyup(function (event) {
    const keycode = event.which;
    const pageBox = $(this);
    if (keycode === 13) {      
      if (validRange(pageBox.val(), pageBox.data("min"), pageBox.data("max"))) {
        let link = pageBox.data('url');
        link = link.replace('-1', pageBox.val());
        window.location = link;
      }
    }
    else if (keycode === 27) {      
      pageBox.val(pageBox.data('current'));
    }
  });
});

function validRange(str, min, max) {
  const intRegex = /^\d+$/;
  if (intRegex.test(str)) {//is it number?
    const num = parseInt(str);
    return num >= min && num <= max;
  }
  else {
    return false;
  }
}