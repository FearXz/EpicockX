// Associa la chiusura del modale ai bottoni di chiusura e annulla
document.addEventListener("DOMContentLoaded", function () {
  // Chiudi il modale quando si clicca sulla X
  document
    .querySelectorAll('[data-dismiss="modal"]')
    .forEach(function (element) {
      element.addEventListener("click", function () {
        closeDeleteModal();
      });
    });

  $(".btnUpdate").click(function () {
    // Recupera i dati del prodotto da visualizzare nel form di update
    let id = $(this).data("id");
    let name = $(this).data("name");
    let description = $(this).data("description");
    let quantity = $(this).data("quantity");
    let price = $(this).data("price");
    let category = $(this).data("category");
    let brand = $(this).data("brand");

    editProduct(id, name, description, quantity, price, category, brand);
  });

  $(".btnDelete").click(function () {
    // Recupera l'ID del prodotto da eliminare
    let productId = $(this).data("id");

    confirmDelete(productId);
  });

  function editProduct(
    id,
    name,
    description,
    quantity,
    price,
    category,
    brand
  ) {
    // Nascondi il form di creazione e mostra quello di update
    $("#createProduct").addClass("hidden");
    $("#updateProduct").removeClass("hidden");

    // Chiudi tutti gli accordion aperti
    $(".accordion-button").addClass("collapsed");
    $(".accordion-collapse").removeClass("show");

    // Apri l'accordion di modifica
    let collapseThree = new bootstrap.Collapse(
      document.getElementById("collapseThree"),
      {
        toggle: true,
      }
    );

    // Popola i campi del form di update con i dati del prodotto
    var updateForm = document.getElementById("updateProductForm");
    updateForm.elements["ProductId"].value = id;
    updateForm.elements["ProductName"].value = name;
    updateForm.elements["ProductDescription"].value = description;
    updateForm.elements["ProductQuantity"].value = quantity;
    updateForm.elements["ProductPrice"].value = price;
    updateForm.elements["ProductCategory"].value = category;
    updateForm.elements["ProductBrand"].value = brand;

    // Scroll verso l'accordion del form
    document
      .getElementById("collapseThree")
      .scrollIntoView({ behavior: "smooth", block: "start" });
  }

  function confirmDelete(productId) {
    // Imposta l'ID del prodotto nel campo nascosto del form
    $("#deleteProductId").val(productId);
    // Mostra il modale
    $("#deleteModal").modal("show");
  }

  function closeDeleteModal() {
    $("#deleteModal").modal("hide");
  }

  // Chiudi il modale quando si clicca su Annulla
  $("#cancelDelete").click(function () {
    closeDeleteModal();
  });
});
