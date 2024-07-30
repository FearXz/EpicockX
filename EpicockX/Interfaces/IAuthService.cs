using EpicockX.Models;
using Stripe.Checkout;

namespace EpicockX.Services
{
    public interface IAuthService
    {
        /// <summary>
        /// Effettua il login dell'utente.
        /// </summary>
        /// <param name="user">L'utente da autenticare.</param>
        /// <returns>Task rappresentante l'operazione asincrona.</returns>
        Task Login(User user);

        /// <summary>
        /// Effettua il logout dell'utente.
        /// </summary>
        /// <returns>Task rappresentante l'operazione asincrona.</returns>
        Task Logout();
    }

    public interface ICartService
    {
        /// <summary>
        /// Crea una sessione di pagamento tramite Stripe.
        /// </summary>
        /// <param name="cart">Lista dei prodotti nel carrello.</param>
        /// <returns>Oggetto Session rappresentante la sessione di pagamento.</returns>
        Session CreateCheckoutSession(List<Product> cart);

        /// <summary>
        /// Ottiene il carrello dell'utente.
        /// </summary>
        /// <returns>Lista di prodotti presenti nel carrello.</returns>
        List<Product> GetCart();

        /// <summary>
        /// Salva il carrello dell'utente.
        /// </summary>
        /// <param name="cart">Lista di prodotti da salvare nel carrello.</param>
        void SaveCart(List<Product> cart);

        /// <summary>
        /// Svuota il carrello dell'utente.
        /// </summary>
        void ClearCart();

        /// <summary>
        /// Invia un ordine per l'utente.
        /// </summary>
        /// <param name="order">Oggetto rappresentante l'ordine.</param>
        /// <param name="userId">ID dell'utente.</param>
        /// <param name="products">Lista di prodotti da includere nell'ordine.</param>
        /// <returns>ID dell'ordine creato.</returns>
        int SubmitOrder(Order order, int userId, List<Product> products);

        /// <summary>
        /// Ottiene il risultato dell'ordine tramite l'ID della sessione.
        /// </summary>
        /// <param name="sessionId">ID della sessione di pagamento.</param>
        /// <returns>Oggetto ResultOrderDto rappresentante il risultato dell'ordine.</returns>
        ResultOrderDto GetResultOrder(string sessionId);
    }

    public interface IImageService
    {
        /// <summary>
        /// Ottiene tutte le immagini dei prodotti.
        /// </summary>
        /// <returns>Lista di immagini dei prodotti.</returns>
        List<ProductImage> GetImages();

        /// <summary>
        /// Ottiene un'immagine di un prodotto tramite il suo ID.
        /// </summary>
        /// <param name="id">ID dell'immagine del prodotto.</param>
        /// <returns>Oggetto ProductImage rappresentante l'immagine del prodotto.</returns>
        ProductImage GetImageById(int id);

        /// <summary>
        /// Aggiunge una nuova immagine di un prodotto.
        /// </summary>
        /// <param name="image">Oggetto ProductImage rappresentante l'immagine da aggiungere.</param>
        void AddImage(ProductImage image);

        /// <summary>
        /// Aggiorna le immagini di un prodotto.
        /// </summary>
        /// <param name="productId">ID del prodotto.</param>
        /// <param name="newImages">Lista di nuovi file immagine.</param>
        void UpdateImage(int productId, List<IFormFile> newImages);

        /// <summary>
        /// Elimina un'immagine di un prodotto tramite il suo ID.
        /// </summary>
        /// <param name="id">ID dell'immagine del prodotto da eliminare.</param>
        void DeleteImage(int id);
    }

    public interface IProductService
    {
        /// <summary>
        /// Ottiene la lista di tutti i prodotti.
        /// </summary>
        /// <returns>Lista di prodotti.</returns>
        List<Product> GetProducts();

        /// <summary>
        /// Ottiene un prodotto tramite il suo ID.
        /// </summary>
        /// <param name="id">ID del prodotto.</param>
        /// <returns>Oggetto Product rappresentante il prodotto.</returns>
        Product GetProductById(int id);

        /// <summary>
        /// Aggiunge un nuovo prodotto.
        /// </summary>
        /// <param name="product">Oggetto Product rappresentante il prodotto da aggiungere.</param>
        void AddProduct(Product product);

        /// <summary>
        /// Aggiorna un prodotto esistente.
        /// </summary>
        /// <param name="product">Oggetto Product rappresentante il prodotto da aggiornare.</param>
        void UpdateProduct(Product product);

        /// <summary>
        /// Elimina un prodotto tramite il suo ID.
        /// </summary>
        /// <param name="id">ID del prodotto da eliminare.</param>
        void DeleteProduct(int id);

        /// <summary>
        /// Ottiene le immagini di un prodotto tramite il suo ID.
        /// </summary>
        /// <param name="productId">ID del prodotto.</param>
        /// <returns>Lista di immagini del prodotto.</returns>
        List<ProductImage> GetProductImages(int productId);

        /// <summary>
        /// Aggiunge un'immagine a un prodotto.
        /// </summary>
        /// <param name="image">Oggetto ProductImage rappresentante l'immagine da aggiungere.</param>
        void AddProductImage(ProductImage image);

        /// <summary>
        /// Elimina un'immagine di un prodotto tramite il suo ID.
        /// </summary>
        /// <param name="id">ID dell'immagine del prodotto da eliminare.</param>
        void DeleteProductImage(int id);
    }

    public interface IUserService
    {
        /// <summary>
        /// Ottiene un utente tramite le credenziali di accesso.
        /// </summary>
        /// <param name="loginDto">Oggetto LoginDto rappresentante le credenziali di accesso.</param>
        /// <returns>Oggetto User rappresentante l'utente.</returns>
        User GetUser(LoginDto loginDto);
    }
}
