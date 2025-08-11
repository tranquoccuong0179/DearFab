using DearFab_Model.Paginate;
using DearFab_Model.Payload.Request.Product;
using DearFab_Model.Payload.Response;
using DearFab_Model.Payload.Response.Product;

namespace DearFab_Service.Interface;

public interface IProductService
{
    Task<BaseResponse<CreateProductResponse>> CreateProduct(CreateProductRequest request);

    Task<BaseResponse<IPaginate<GetProductResponse>>> GetAllProduct(int page, int size);
    
    Task<BaseResponse<GetProductDetailResponse>> GetProductById(Guid id);
    
    Task<BaseResponse<UpdateProductResponse>> UpdateProduct(Guid id, UpdateProductRequest request);
    
    Task<BaseResponse<bool>> DeleteProduct(Guid id);
}