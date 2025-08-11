using System.Security.Cryptography;
using System.Text;
using DearFab_Model.Entity;
using DearFab_Model.Enum;
using DearFab_Model.Payload.Request.Payment;
using DearFab_Model.Payload.Response;
using DearFab_Model.Payload.Settings;
using DearFab_Model.Utils;
using DearFab_Repository.Interface;
using DearFab_Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Net.payOS;
using Net.payOS.Types;
using Transaction = DearFab_Model.Entity.Transaction;

namespace DearFab_Service.Implement;

public class PaymentService : BaseService<PaymentService>, IPaymentService
{
    private readonly PayOS _payOS;
    private readonly PayOSSettings _payOSSettings;
    private readonly HttpClient _client;


    public PaymentService(IUnitOfWork<DearFabContext> unitOfWork, ILogger<PaymentService> logger, IHttpContextAccessor httpContextAccessor, PayOS payOs, IOptions<PayOSSettings> payOsSettings, HttpClient client) : base(unitOfWork, logger, httpContextAccessor)
    {
        _payOS = payOs;
        _payOSSettings = payOsSettings.Value;
        _client = client;
    }

    public async Task<BaseResponse<CreatePaymentResult>> CreatePaymentUrlRegisterCreator(CreatePaymentRequest request)
    {
        Guid? accountId = UserUtil.GetAccountId(_httpContextAccessor.HttpContext);

        var account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
            predicate: a => a.Id.Equals(accountId));

        if (account == null)
        {
            return new BaseResponse<CreatePaymentResult>
            {
                Status = StatusCodes.Status404NotFound,
                Message = "Không tìm thấy tài khoản người dùng",
                Data = null
            };
        }

        string buyerName = account.FullName;
        string buyerPhone = account.Phone;
        string buyerEmail = account.Email;

        var order = await _unitOfWork.GetRepository<Order>().SingleOrDefaultAsync(
            predicate: o => o.Id.Equals(request.OrderId) && o.AccountId.Equals(accountId) && o.IsActive == true,
            include: o => o.Include(o => o.OrderItems)
                                                .ThenInclude(oi => oi.ProductSize)
                                                    .ThenInclude(ps => ps.Product)
                                            .Include(o => o.OrderItems)
                                                .ThenInclude(oi => oi.ProductSize)
                                                    .ThenInclude(ps => ps.Size));

        if (order == null)
        {
            return new BaseResponse<CreatePaymentResult>
            {
                Status = StatusCodes.Status404NotFound,
                Message = "Không tìm thấy đơn",
                Data = null
            };
        }
        
        var items = new List<ItemData>();
        foreach (var orderItem in order.OrderItems)
        {
            var itemData = new ItemData(orderItem.ProductSize.Product.Name + " " + orderItem.ProductSize.Size.Label, orderItem.Quantity,
                (int)(orderItem.ProductSize.Price.Value * orderItem.Quantity));
            items.Add(itemData);
        }

        Random random = new Random();
        /*long orderCode = long.Parse(
            DateTime.Now.ToString("yyMMddHHmmss") + random.Next(10, 99)
        );*/
        long orderCode = DateTime.Now.Ticks % 10000000000000L * 10 + random.Next(0, 10);
        var description = "Thanh toán " + orderCode;
        var signatureData = new Dictionary<string, object>
        {
            { "amount", order.TotalPrice },
            { "cancelUrl", _payOSSettings.ReturnUrlFail },
            { "description", description },
            { "expiredAt", DateTimeOffset.Now.AddMinutes(10).ToUnixTimeSeconds() },
            { "orderCode", orderCode },
            { "returnUrl", _payOSSettings.ReturnUrl }
        };

        var sortedSignatureData = new SortedDictionary<string, object>(signatureData);
        var dataForSignature = string.Join("&", sortedSignatureData.Select(p => $"{p.Key}={p.Value}"));
        var signature = ComputeHmacSha256(dataForSignature, _payOSSettings.ChecksumKey);

        DateTimeOffset expiredAt = DateTimeOffset.Now.AddMinutes(10);

        var paymentData = new PaymentData(
            orderCode: orderCode,
            amount: (int)order.TotalPrice,
            description: description,
            items: items,
            cancelUrl: _payOSSettings.ReturnUrlFail,
            returnUrl: _payOSSettings.ReturnUrl,
            signature: signature,
            buyerName: buyerName,
            buyerPhone: buyerPhone,
            buyerEmail: buyerEmail,
            buyerAddress: account.Address,
            expiredAt: (int)expiredAt.ToUnixTimeSeconds()
        );


        var paymentResult = await _payOS.createPaymentLink(paymentData);

        var addNewTransaction = new DearFab_Model.Entity.Transaction()
        {
            Id = Guid.NewGuid(),
            OrderId = order.Id,
            OrderCode = orderCode,
            Status = StatusTransactionEnum.Pending.GetDescriptionFromEnum(),
            CreateAt = TimeUtil.GetCurrentSEATime()
        };

        await _unitOfWork.GetRepository<DearFab_Model.Entity.Transaction>().InsertAsync(addNewTransaction);
        await _unitOfWork.CommitAsync();

        return new BaseResponse<CreatePaymentResult>
        {
            Status = StatusCodes.Status200OK,
            Message = "Tạo link thành công",
            Data = paymentResult,
        };
    }
    private string? ComputeHmacSha256(string data, string checksumKey)
    {
        using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(checksumKey)))
        {
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }
}
