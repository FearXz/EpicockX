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
    let id = $(this).data("ProductId");
    let name = $(this).data("ProductName");
    let description = $(this).data("ProductDescription");
    let quantity = $(this).data("ProductQuantity");
    let price = $(this).data("ProductPrice");
    let category = $(this).data("ProductCategory");
    let brand = $(this).data("ProductBrand");

    editProduct(id, name, description, quantity, price, category, brand);
  });

  $(".btnDelete").click(function () {
    // Recupera l'ID del prodotto da eliminare
    let productId = $(this).data("ProductId");

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
    document.getElementById("createProduct").classList.add("hidden");
    document.getElementById("updateProduct").classList.remove("hidden");

    // Seleziona il form di update tramite il suo ID
    var updateForm = document.getElementById("updateProductForm");

    // Popola i campi del form di update con i dati del prodotto
    document.getElementById("ProductId").value = id;
    document.getElementById("ProductName").value = name;
    document.getElementById("ProductDescription").value = description;
    document.getElementById("ProductQuantity").value = quantity;
    document.getElementById("ProductPrice").value = price;
    document.getElementById("ProductCategory").value = category;
    document.getElementById("ProductBrand").value = brand;

    // Gestione dell'apertura e chiusura degli accordion
    let collapseOne = new bootstrap.Collapse(
      document.getElementById("collapseOne"),
      { toggle: false }
    );
    let collapseThree = new bootstrap.Collapse(
      document.getElementById("collapseThree"),
      { toggle: true }
    );

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
