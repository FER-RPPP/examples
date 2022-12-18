$(document).on('click', '.deleterow', function () {
  event.preventDefault();
  const tr = $(this).parents("tr");
  tr.remove();
});

$(function () {
  $(".form-control").bind('keydown', function (event) {
    if (event.which === 13) {
      event.preventDefault();
    }
  });

  $("#product-quantity, #product-discount").bind('keydown', function (event) {
    if (event.which === 13) {
      event.preventDefault();
      addProduct();
    }
  });


  $("#product-add").click(function () {
    event.preventDefault();
    addProduct();
  });
});

function addProduct() {
  const productNumber = $("#product-number").val();
  if (productNumber !== '') {
    if ($("[name='Items[" + productNumber + "].ProductNumber'").length > 0) {
      alert('Product is already included in the document.');
      return;
    }

    let quantity = parseFloat($("#product-quantity").val().replace(',', '.')); //must be dot, and not comma za parseFloat
    if (isNaN(quantity))
      quantity = 1;

    let discount = parseFloat($("#product-discount").val().replace(',', '.')); 
    if (isNaN(discount))
      discount = 0;

    const price = parseFloat($("#product-price").val());
    const price_formatted = price.toFixed(2).replace('.', ',').replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$1.") + ' kn'

    let template = $('#template').html();
    const name = $("#product-name").val();
    let amount = quantity * price * (1 - discount);
    amount = amount.toFixed(2).replace('.', ',').replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$1.") + ' kn';
    
    //Some alternative related to decimal comma and dot //http://haacked.com/archive/2011/03/19/fixing-binding-to-decimals.aspx/
    //ili ovo http://intellitect.com/custom-model-binding-in-asp-net-core-1-0/

    template = template.replace(/--number--/g, productNumber)
      .replace(/--quantity--/g, quantity.toFixed(2).replace('.', ','))
      .replace(/--price--/g, price)
      .replace(/--price_formatted--/g, price_formatted)
      .replace(/--name--/g, name)
      .replace(/--amount--/g, amount)
      .replace(/--discount--/g, discount.toFixed(2).replace('.', ','));
    $(template).find('tr').insertBefore($("#table-items").find('tr').last());

    $("#product-number").val('');
    $("#product-quantity").val('');
    $("#product-discount").val('');
    $("#product-price").val('');
    $("#product-name").val('');
  }
}