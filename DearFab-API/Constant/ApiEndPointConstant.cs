namespace DearFab.Constant;

public static class ApiEndPointConstant
{
    public const string RootEndPoint = "/api";
    public const string ApiVersion = "/v1";
    public const string ApiEndpoint = RootEndPoint + ApiVersion;
    
    public static class Account
    {
        public const string AccountEndPoint = ApiEndpoint + "/account";
        public const string RegisterAccount    = AccountEndPoint;
        public const string GetAccounts    = AccountEndPoint;
        public const string GetAccount    = AccountEndPoint + "/{id}";
        public const string UpdateAccount    = AccountEndPoint;
        public const string DeleteAccount    = AccountEndPoint + "/{id}";
    }
    
    public static class Authentication
    {
        public const string AuthenticationEndPoint = ApiEndpoint + "/auth";
        public const string Authenticate = AuthenticationEndPoint;
    }
    
    public static class Product
    {
        public const string ProductEndPoint = ApiEndpoint + "/product";
        public const string CreateProduct = ProductEndPoint;
        public const string GetAllProduct = ProductEndPoint;
        public const string GetProductById = ProductEndPoint + "/{id}";
        public const string UpdateProduct = ProductEndPoint + "/{id}";
        public const string DeleteProduct = ProductEndPoint + "/{id}";
        public const string CreateReview = ProductEndPoint + "/{id}/review";
        public const string GetAllReview = ProductEndPoint + "/{id}/review";
    }
    
    public static class Size
    {
        public const string SizeEndPoint = ApiEndpoint + "/size";
        public const string CreateSize = SizeEndPoint;
        public const string GetSizes = SizeEndPoint;
        public const string GetSize = SizeEndPoint + "/{id}";
    }

    public static class Order
    {
        public const string OrderEndPoint = ApiEndpoint + "/order";
        public const string CreateOrder = OrderEndPoint;
        public const string GetOrders = OrderEndPoint;
        public const string GetOrder = OrderEndPoint + "/{id}";
        public const string ChangeStatus = OrderEndPoint + "/{id}/status";
    }
    
    public static class Payment
    {
        public const string PaymentEndPoint = ApiEndpoint + "/payment";
        public const string CreatePayment = PaymentEndPoint;
        public const string HandlePayment = PaymentEndPoint + "/handle";
        public const string ReturnUrlFail = PaymentEndPoint + "/failed";
    }
    
    public static class ProductSize
    {
        public const string ProductSizeEndPoint = ApiEndpoint + "/product-size";
        public const string DeleteProductSize = ProductSizeEndPoint + "/{id}";
        public const string UpdateProductSize = ProductSizeEndPoint + "/{id}";
    }

    public static class Review
    {
        public const string ReviewEndPoint = ApiEndpoint + "/review";
        public const string DeleteReview = ReviewEndPoint + "/{id}";
    }
}