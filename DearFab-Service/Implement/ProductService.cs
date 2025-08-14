using DearFab_Model.Entity;
using DearFab_Model.Paginate;
using DearFab_Model.Payload.Request.Product;
using DearFab_Model.Payload.Response;
using DearFab_Model.Payload.Response.Product;
using DearFab_Model.Payload.Response.ProductSize;
using DearFab_Model.Utils;
using DearFab_Repository.Interface;
using DearFab_Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DearFab_Service.Implement;

public class ProductService : BaseService<ProductService>, IProductService
{
    private readonly IUploadService _uploadService;
    public ProductService(IUnitOfWork<DearFabContext> unitOfWork, ILogger<ProductService> logger, IHttpContextAccessor httpContextAccessor, IUploadService uploadService) : base(unitOfWork, logger, httpContextAccessor)
    {
        _uploadService = uploadService;
    }

    public async Task<BaseResponse<CreateProductResponse>> CreateProduct(CreateProductRequest request)
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            Image = await _uploadService.UploadImage(request.Image),
            IsNew = true,
            IsActive = true,
            CreateAt = TimeUtil.GetCurrentSEATime()
        };
        
        await _unitOfWork.GetRepository<Product>().InsertAsync(product);
        
        List<ProductSize> productSizes = new List<ProductSize>();
        
        foreach (var size in request.Sizes)
        {
            var sizeExist = await _unitOfWork.GetRepository<Size>().SingleOrDefaultAsync(
                predicate: s => s.Id.Equals(size.SizeId) && s.IsActive == true);

            if (sizeExist == null)
            {
                return new BaseResponse<CreateProductResponse>()
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = "Kích thước không tồn tại",
                    Data = null
                };
            }

            var productSize = new ProductSize
            {
                Id = Guid.NewGuid(),
                ProductId = product.Id,
                SizeId = sizeExist.Id,
                Price = size.Price,
                Quantity = size.Quantity,
                IsActive = true
            };
            productSizes.Add(productSize);
        }

        await _unitOfWork.GetRepository<ProductSize>().InsertRangeAsync(productSizes);
        
        var isSuccess = await _unitOfWork.CommitAsync() > 0;
        
        if (!isSuccess)
        {
            throw new Exception("Một lỗi đã xảy ra trong quá trình tạo sản phẩm");
        }

        return new BaseResponse<CreateProductResponse>()
        {
            Status = StatusCodes.Status200OK,
            Message = "Tạo sản phẩm thành công",
            Data = new CreateProductResponse()
            {
                Name = product.Name,
                Description = product.Description,
                Image = product.Image
            }
        };
    }

    public async Task<BaseResponse<IPaginate<GetProductResponse>>> GetAllProduct(int page, int size)
    {
        var products = await _unitOfWork.GetRepository<Product>().GetPagingListAsync(
            selector: p => new GetProductResponse()
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Image = p.Image,
                Price = p.ProductSizes.Where(ps => ps.Price.HasValue && ps.IsActive == true).Select(ps => ps.Price.Value).Min(),
                Quantity = p.ProductSizes.Where(ps => ps.Quantity.HasValue && ps.IsActive == true).Select(ps => ps.Quantity.Value).Sum(),
                Rating = p.Reviews.Average(r => (double?)r.Rating) ?? 0
            },
            predicate: p => p.IsActive == true,
            include: p => p.Include(p => p.ProductSizes).Include(p => p.Reviews),
            page: page,
            size: size);

        return new BaseResponse<IPaginate<GetProductResponse>>()
        {
            Status = StatusCodes.Status200OK,
            Message = "Lấy danh sách sản phẩm thành công",
            Data = products
        };
    }

    public async Task<BaseResponse<GetProductDetailResponse>> GetProductById(Guid id)
    {
        var product = await _unitOfWork.GetRepository<Product>().SingleOrDefaultAsync(
            selector: p => new GetProductDetailResponse()
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Image = p.Image,
                productSizes = p.ProductSizes.
                    Where(ps => ps.IsActive == true).
                    Select(ps => new GetProductSizeResponse()
                    {
                        Id = ps.Id,
                        Price = ps.Price,
                        Quantity = ps.Quantity,
                        Size = ps.Size.Label
                    }).ToList(),
            },
            predicate: p => p.Id.Equals(id) && p.IsActive == true,
            include: p => p.Include(p => p.ProductSizes).ThenInclude(ps => ps.Size)
            );

        if (product == null)
        {
            new BaseResponse<GetProductDetailResponse>()
            {
                Status = StatusCodes.Status404NotFound,
                Message = "Không tìm thấy sản phẩm",
                Data = null
            };
        }

        return new BaseResponse<GetProductDetailResponse>()
        {
            Status = StatusCodes.Status200OK,
            Message = "Lấy thông tin sản phẩm thành công",
            Data = product
        };
    }

    public async Task<BaseResponse<UpdateProductResponse>> UpdateProduct(Guid id, UpdateProductRequest request)
    {
        var product = await _unitOfWork.GetRepository<Product>().SingleOrDefaultAsync(
            predicate: p => p.Id.Equals(id) && p.IsActive == true);

        if (product == null)
        {
            return new BaseResponse<UpdateProductResponse>()
            {
                Status = StatusCodes.Status404NotFound,
                Message = "Không tìm thấy sản phẩm",
                Data = null
            };
        }
        
        string? uploadedImage = null;
        if (request.Image != null)
        {
            uploadedImage = await _uploadService.UploadImage(request.Image);
        }
        
        product.Name = request.Name ?? product.Name;
        product.Description = request.Description ?? product.Description;
        product.Image = uploadedImage ?? product.Image;
        
        _unitOfWork.GetRepository<Product>().UpdateAsync(product);
        var isSuccess = await _unitOfWork.CommitAsync() > 0;
        
        if (!isSuccess)
        {
            throw new Exception("Một lỗi đã xảy ra trong quá trình cập nhật sản phẩm");
        }

        return new BaseResponse<UpdateProductResponse>()
        {
            Status = StatusCodes.Status200OK,
            Message = "Cập nhật sản phẩm thành công",
            Data = new UpdateProductResponse()
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Image = product.Image,
            }
        };
    }

    public async Task<BaseResponse<bool>> DeleteProduct(Guid id)
    {
        var product = await _unitOfWork.GetRepository<Product>().SingleOrDefaultAsync(
            predicate: p => p.Id.Equals(id) && p.IsActive == true);

        if (product == null)
        {
            return new BaseResponse<bool>()
            {
                Status = StatusCodes.Status404NotFound,
                Message = "Không tìm thấy sản phẩm",
                Data = false
            };
        }
        
        product.IsActive = false;
        product.DeleteAt = TimeUtil.GetCurrentSEATime();
        _unitOfWork.GetRepository<Product>().UpdateAsync(product);

        var productSizes = await _unitOfWork.GetRepository<ProductSize>().GetListAsync(
            predicate: ps => ps.ProductId.Equals(id) && ps.IsActive == true);

        foreach (var productSize in productSizes)
        {
            productSize.IsActive = false;
        }
        _unitOfWork.GetRepository<ProductSize>().UpdateRange(productSizes);
        
        var isSuccess = await _unitOfWork.CommitAsync() > 0;
        
        if (!isSuccess)
        {
            throw new Exception("Một lỗi đã xảy ra trong quá trình xóa sản phẩm");
        }

        return new BaseResponse<bool>()
        {
            Status = StatusCodes.Status200OK,
            Message = "Xóa sản phẩm thành công",
            Data = true
        };
    }
}