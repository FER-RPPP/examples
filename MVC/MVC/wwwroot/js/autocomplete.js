$(function () {

  $("[data-autocomplete]").each(function (index, element) {
    const action = $(element).data('autocomplete');
    let resultplaceholder = $(element).data('autocomplete-placeholder-name');
    if (resultplaceholder === undefined)
      resultplaceholder = action;

    $(element).change(function () {
      const dest = $(`[data-autocomplete-placeholder='${resultplaceholder}']`);
      const text = $(element).val();
      if (text.length === 0 || text !== $(dest).data('selected-label')) {
        $(dest).val('');
      }
    });

    $(element).autocomplete({
      source: window.applicationBaseUrl + "autocomplete/" + action,
      autoFocus: true,
      minLength: 1,
      select: function (event, ui) {
        $(element).val(ui.item.label);
        const dest = $(`[data-autocomplete-placeholder='${resultplaceholder}']`);
        $(dest).val(ui.item.id);
        $(dest).data('selected-label', ui.item.label);

        //za artikle...
        const dest_cijena = $(`[data-autocomplete-placeholder-cijena='${resultplaceholder}']`);
        if (dest_cijena !== undefined) {
          $(dest_cijena).val(ui.item.cijena);
        }
      }
    });
  });
});